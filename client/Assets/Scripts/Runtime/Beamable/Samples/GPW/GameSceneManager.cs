﻿using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.Samples.GPW.UI.ScrollingList;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Game
   /// </summary>
   public class GameSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public GameUIView GameUIView { get { return _gameUIView; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Configuration _configuration = null;
      
      [SerializeField]
      private GameUIView _gameUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.ProductContentListCanvasGroup.alpha = 0;
         
         _gameUIView.TravelButton.onClick.AddListener(TravelButton_OnClicked);
         _gameUIView.BankButton.onClick.AddListener(BankButton_OnClicked);
         _gameUIView.DebtButton.onClick.AddListener(DebtButton_OnClicked);
         //
         _gameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
         _gameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _gameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         SetupBeamable();
         
      }
      

      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         _beamableAPI = await Beamable.API.Instance;
         
         if (!GameController.Instance.IsInitialized)
         {
            await GameController.Instance.Initialize(_configuration);
         }

         // ProductContentList
         _gameUIView.ProductContentList.OnInitialized.AddListener(CircularScrollingList_OnInitialized);
         ProductContentListBank listBank =  _gameUIView.ProductContentList.gameObject.AddComponent<ProductContentListBank>();
         _gameUIView.ProductContentList.ListBank = listBank;
         
         // Storage
         GameController.Instance.PersistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
         GameController.Instance.RuntimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
         GameController.Instance.PersistentDataStorage.ForceRefresh();
         GameController.Instance.RuntimeDataStorage.ForceRefresh();
      }

      
      //  Event Handlers -------------------------------
      
      private void TravelButton_OnClicked()
      {
         GameController.Instance.UpdateLocationTo();
  }
      
      private void BankButton_OnClicked()
      {
         GameController.Instance.UpdateBankTo();
      }
      
      private void DebtButton_OnClicked()
      {
         GameController.Instance.UpdateDebtTo();
      }
      
      private void ChatButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.ChatSceneName,
            _configuration.DelayBeforeLoadScene));
      }

      
      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.LeaderboardSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      
      private void QuitButton_OnClicked()
      {
         Debug.Log("as are you sure? This blanks progress");
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      
      private async void ProductContentListItem_OnBuy(ProductContentView productContentView)
      {
         var canBuyItem = await GameController.Instance.GameServices.
            CanBuyItem(productContentView.ProductContent.Id, 1);
         
         bool isSuccessful = false;
         if (canBuyItem)
         {
            isSuccessful = await GameController.Instance.GameServices.
               BuyItem(productContentView.ProductContent.Id, 1);
         }
         
         Debug.Log($"ProductContentListItem_OnBuy() canBuyItem = {canBuyItem}, " +
                   $"isSuccessful = {isSuccessful}");
      }
      
      
      private async void ProductContentListItem_OnSell(ProductContentView productContentView)
      {
         bool canSellItem = await GameController.Instance.GameServices.
            CanSellItem(productContentView.ProductContent.Id, 1);

         bool isSuccessful = false;
         if (canSellItem)
         {
            isSuccessful = await GameController.Instance.GameServices.
               SellItem(productContentView.ProductContent.Id, 1);
         }

         Debug.Log($"ProductContentListItem_OnSell() canSellItem = {canSellItem}, " +
                   $"isSuccessful = {isSuccessful}");
      }
      
      private void PersistentDataStorage_OnChanged(SubStorage subStorage)
      {
         Debug.Log("PersistentDataStorage_OnChanged: -------");
         
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         _gameUIView.PersistentData = persistentDataStorage.PersistentData;
         
         List<ProductContentView> list = persistentDataStorage.PersistentData.LocationContentViewCurrent.ProductContentViews;

         // Prepare list
         ProductContentListBank listBank = _gameUIView.ProductContentList.ListBank as ProductContentListBank;
         listBank.SetContents(list);
         
         // Render list
         _gameUIView.ProductContentList.ListBank = listBank;
         _gameUIView.ProductContentList.Initialize();
         _gameUIView.ProductContentList.Refresh();

         if (_gameUIView.ProductContentListCanvasGroup.alpha == 0)
         {
            _gameUIView.ProductContentListCanvasGroup.alpha = 1;
         }
      }
      
      
      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _gameUIView.RuntimeData = runtimeDataStorage.RuntimeData;
      }
      
      
      private void CircularScrollingList_OnInitialized(CircularScrollingList circularScrollingList)
      {
         foreach (ListBox listBox in circularScrollingList.ListBoxes)
         {
            ProductContentListItem productContentListItem = listBox as ProductContentListItem;
            
            productContentListItem.OnBuy.RemoveAllListeners();
            productContentListItem.OnSell.RemoveAllListeners();
            //
            productContentListItem.OnBuy.AddListener(ProductContentListItem_OnBuy);
            productContentListItem.OnSell.AddListener(ProductContentListItem_OnSell);
         }
      }
      
   }
}