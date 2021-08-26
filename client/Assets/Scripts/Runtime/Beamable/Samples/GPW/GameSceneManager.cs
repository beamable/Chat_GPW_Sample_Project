using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI.DialogSystem;
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

      //  Fields ---------------------------------------
      [SerializeField]
      private GameUIView _gameUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.ProductContentList.CanvasGroup.alpha = 0;
         
         _gameUIView.TravelButton.onClick.AddListener(TravelButton_OnClicked);
         _gameUIView.BankButton.onClick.AddListener(BankButton_OnClicked);
         _gameUIView.DebtButton.onClick.AddListener(DebtButton_OnClicked);
         //
         _gameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
         _gameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _gameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         
         //
         SetupBeamable();
         
      }
      

      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         // Setup List
         _gameUIView.ProductContentList.OnInitialized.AddListener(ProductContentList_OnInitialized);
         
         // Setup Storage
         GameController.Instance.PersistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
         GameController.Instance.RuntimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
         
         // Every scene initializes as needed (Max 1 time per session)
         if (!GameController.Instance.IsInitialized)
         {
            await GameController.Instance.Initialize(_gameUIView.Configuration);
         }
         else
         {
            GameController.Instance.PersistentDataStorage.ForceRefresh();
            GameController.Instance.RuntimeDataStorage.ForceRefresh();
            GameController.Instance.GameServices.ForceRefresh();
         }
      }

      
      //  Event Handlers -------------------------------
      
      private void TravelButton_OnClicked()
      {
         GameController.Instance.GoToLocation();
  }
      
      private void BankButton_OnClicked()
      {
         _gameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _gameUIView.DialogSystem.DialogUIPrefab,
            "Bank",
            "Transfer from CASH → BANK?",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GameController.Instance.TransferCashToBank(100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GameController.Instance.TransferCashToBank(-100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  _gameUIView.DialogSystem.HideDialogBox();
               })
            });
      }


      private void DebtButton_OnClicked()
      {
         _gameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _gameUIView.DialogSystem.DialogUIPrefab,
            "Transfer from CASH → DEBT?",
            $"DEBT increases by " +
            $"{GameController.Instance.RuntimeDataStorage.RuntimeData.DebtInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GameController.Instance.TransferCashToDebt(100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GameController.Instance.TransferCashToDebt(-100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  _gameUIView.DialogSystem.HideDialogBox();
               })
            });
         
      }
      
      private void ChatButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _gameUIView.Configuration.ChatSceneName,
            _gameUIView.Configuration.DelayBeforeLoadScene));
      }

      
      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _gameUIView.Configuration.LeaderboardSceneName,
            _gameUIView.Configuration.DelayBeforeLoadScene));
      }
      
      
      private void QuitButton_OnClicked()
      {
         _gameUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               StartCoroutine(GPWHelper.LoadScene_Coroutine(
                  _gameUIView.Configuration.IntroSceneName,
                  _gameUIView.Configuration.DelayBeforeLoadScene));
            });
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
      
      private async void PersistentDataStorage_OnChanged(SubStorage subStorage)
      {
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         _gameUIView.PersistentData = persistentDataStorage.PersistentData;

         ProductContentListRefresh();
   
      }

      private async void ProductContentListRefresh()
      {
         List<ProductContentView> list = GameController.Instance.PersistentDataStorage.
            PersistentData.LocationContentViewCurrent.ProductContentViews;
         
         // Render list
         await _gameUIView.ProductContentList.InitializeOnDelay(list, 100);
         _gameUIView.ProductContentList.Refresh();
         

         if (_gameUIView.ProductContentList.CanvasGroup.alpha == 0)
         {
            _gameUIView.ProductContentList.CanvasGroup.alpha = 1;
         }
      }


      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _gameUIView.RuntimeData = runtimeDataStorage.RuntimeData;
      }
      
      
      private void ProductContentList_OnInitialized(CircularScrollingList circularScrollingList)
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