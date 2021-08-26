using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.Samples.GPW.UI.ScrollingList;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Game
   /// </summary>
   public class Scene02GameManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public Scene02GameUIView Scene02GameUIView { get { return _scene02GameUIView; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Scene02GameUIView _scene02GameUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _scene02GameUIView.ProductContentList.CanvasGroup.alpha = 0;
         
         _scene02GameUIView.TravelButton.onClick.AddListener(TravelButton_OnClicked);
         _scene02GameUIView.BankButton.onClick.AddListener(BankButton_OnClicked);
         _scene02GameUIView.DebtButton.onClick.AddListener(DebtButton_OnClicked);
         //
         _scene02GameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
         _scene02GameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _scene02GameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         
         //
         SetupBeamable();
         
      }
      

      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         // Setup List
         _scene02GameUIView.ProductContentList.OnInitialized.AddListener(ProductContentList_OnInitialized);
         
         // Setup Storage
         GPWController.Instance.PersistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
         GPWController.Instance.RuntimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
         
         // Every scene initializes as needed (Max 1 time per session)
         if (!GPWController.Instance.IsInitialized)
         {
            await GPWController.Instance.Initialize(_scene02GameUIView.Configuration);
         }
         else
         {
            GPWController.Instance.PersistentDataStorage.ForceRefresh();
            GPWController.Instance.RuntimeDataStorage.ForceRefresh();
            GPWController.Instance.GameServices.ForceRefresh();
         }
      }

      
      //  Event Handlers -------------------------------
      
      private void TravelButton_OnClicked()
      {
         GPWController.Instance.GoToLocation();
  }
      
      private void BankButton_OnClicked()
      {
         _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _scene02GameUIView.DialogSystem.DialogUIPrefab,
            "Bank",
            "Transfer from CASH → BANK?",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GPWController.Instance.TransferCashToBank(100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GPWController.Instance.TransferCashToBank(-100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  _scene02GameUIView.DialogSystem.HideDialogBox();
               })
            });
      }


      private void DebtButton_OnClicked()
      {
         _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _scene02GameUIView.DialogSystem.DialogUIPrefab,
            "Transfer from CASH → DEBT?",
            $"DEBT increases by " +
            $"{GPWController.Instance.RuntimeDataStorage.RuntimeData.DebtInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GPWController.Instance.TransferCashToDebt(100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GPWController.Instance.TransferCashToDebt(-100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  _scene02GameUIView.DialogSystem.HideDialogBox();
               })
            });
         
      }
      
      private void ChatButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene02GameUIView.Configuration.Scene03ChatName,
            _scene02GameUIView.Configuration.DelayBeforeLoadScene));
      }

      
      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene02GameUIView.Configuration.Scene04LeaderboardName,
            _scene02GameUIView.Configuration.DelayBeforeLoadScene));
      }
      
      
      private void QuitButton_OnClicked()
      {
         _scene02GameUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               StartCoroutine(GPWHelper.LoadScene_Coroutine(
                  _scene02GameUIView.Configuration.Scene01IntroName,
                  _scene02GameUIView.Configuration.DelayBeforeLoadScene));
            });
      }
      
      
      private async void ProductContentListItem_OnBuy(ProductContentView productContentView)
      {
         var canBuyItem = await GPWController.Instance.GameServices.
            CanBuyItem(productContentView.ProductContent.Id, 1);
         
         bool isSuccessful = false;
         if (canBuyItem)
         {
            isSuccessful = await GPWController.Instance.GameServices.
               BuyItem(productContentView.ProductContent.Id, 1);
         }
         
         Debug.Log($"ProductContentListItem_OnBuy() canBuyItem = {canBuyItem}, " +
                   $"isSuccessful = {isSuccessful}");
      }
      
      
      private async void ProductContentListItem_OnSell(ProductContentView productContentView)
      {
         bool canSellItem = await GPWController.Instance.GameServices.
            CanSellItem(productContentView.ProductContent.Id, 1);

         bool isSuccessful = false;
         if (canSellItem)
         {
            isSuccessful = await GPWController.Instance.GameServices.
               SellItem(productContentView.ProductContent.Id, 1);
         }

         Debug.Log($"ProductContentListItem_OnSell() canSellItem = {canSellItem}, " +
                   $"isSuccessful = {isSuccessful}");
      }
      
      private async void PersistentDataStorage_OnChanged(SubStorage subStorage)
      {
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         _scene02GameUIView.PersistentData = persistentDataStorage.PersistentData;

         ProductContentListRefresh();
   
      }

      private async void ProductContentListRefresh()
      {
         List<ProductContentView> list = GPWController.Instance.PersistentDataStorage.
            PersistentData.LocationContentViewCurrent.ProductContentViews;
         
         // Render list
         await _scene02GameUIView.ProductContentList.InitializeOnDelay(list, 100);
         _scene02GameUIView.ProductContentList.Refresh();
         

         if (_scene02GameUIView.ProductContentList.CanvasGroup.alpha == 0)
         {
            _scene02GameUIView.ProductContentList.CanvasGroup.alpha = 1;
         }
      }


      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _scene02GameUIView.RuntimeData = runtimeDataStorage.RuntimeData;
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