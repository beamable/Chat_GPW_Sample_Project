using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AirFishLab.ScrollingList;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.UI.ScrollingList;
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
      private bool _isReadyRuntimeDataStorage = false;
      private bool _isReadyProductContentList = false;
      private bool _isReadyInventoryView = false;
      
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
         
         // Wait
         _scene02GameUIView.DialogSystem.ShowDialogBoxLoading();
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
         GPWController.Instance.GameServices.OnInventoryViewChanged.AddListener(GameServices_OnInventoryViewChanged);
         
         
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
      
      private async void CheckIsSceneReady()
      {
         if (_isReadyRuntimeDataStorage && _isReadyProductContentList && _isReadyInventoryView)
         {
            await Task.Delay((int)_scene02GameUIView.Configuration.DelayAfterDataLoading*1000);
            _scene02GameUIView.DialogSystem.HideDialogBox();
         }
      }

      private void QuitGameSafe(bool willConfirm)
      {
         if (willConfirm)
         {
            _scene02GameUIView.DialogSystem.ShowDialogBoxConfirmation(
               delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  StartCoroutine(GPWHelper.LoadScene_Coroutine(
                     _scene02GameUIView.Configuration.Scene01IntroName,
                     _scene02GameUIView.Configuration.DelayBeforeLoadScene));
               });
         }
         else
         {
            StartCoroutine(GPWHelper.LoadScene_Coroutine(
               _scene02GameUIView.Configuration.Scene01IntroName,
               _scene02GameUIView.Configuration.DelayBeforeLoadScene));
         }
 
      }
      
      //  Event Handlers -------------------------------
      
      private void TravelButton_OnClicked()
      {
         GPWHelper.PlayAudioClipSecondaryClick();
         GPWController.Instance.GoToLocation();
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
         GPWHelper.PlayAudioClipSecondaryClick();
         QuitGameSafe(true);
      }


      private void BankButton_OnClicked()
      {
         
         GPWHelper.PlayAudioClipSecondaryClick();
         
         
         _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _scene02GameUIView.DialogSystem.DialogUIPrefab,
            $"Transfer from CASH → BANK",
            "Positive BANK will help your final score.\n\n\nBANK increases by " +
            $"{GPWController.Instance.RuntimeDataStorage.RuntimeData.BankInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  GPWController.Instance.TransferCashToBank(100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  GPWController.Instance.TransferCashToBank(-100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  _scene02GameUIView.DialogSystem.HideDialogBox();
               })
            });
      }


      private void DebtButton_OnClicked()
      {
         
         GPWHelper.PlayAudioClipSecondaryClick();
         
         
         _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
            _scene02GameUIView.DialogSystem.DialogUIPrefab,
            "Transfer from CASH → DEBT?",
            $"Positive DEBT will hurt your final score.\n\n\nDEBT increases by " +
            $"{GPWController.Instance.RuntimeDataStorage.RuntimeData.DebtInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData("+100", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  GPWController.Instance.TransferCashToDebt(-100);
               }),
               new DialogButtonData("-100", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  GPWController.Instance.TransferCashToDebt(100);
               }),
               new DialogButtonData("Ok", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  _scene02GameUIView.DialogSystem.HideDialogBox();
               })
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
         
         if (persistentDataStorage.PersistentData.IsGameOver)
         {
            //
            int turnCurrent = GPWController.Instance.PersistentDataStorage.PersistentData.TurnCurrent;
            int turnsTotal = GPWController.Instance.PersistentDataStorage.PersistentData.TurnsTotal;
            //
            int cashAmount = GPWController.Instance.PersistentDataStorage.PersistentData.CashAmount;
            int bankAmount = GPWController.Instance.PersistentDataStorage.PersistentData.BankAmount;
            int debtAmount = GPWController.Instance.PersistentDataStorage.PersistentData.DebitAmount;
            double score = GPWController.Instance.CalculatedCurrentScore();
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"CASH + BANK - DEBT = FINAL SCORE");
            stringBuilder.AppendLine($"{cashAmount} + {bankAmount} - {debtAmount} = {score}");
            stringBuilder.AppendLine();
            
            int itemsCurrent = GPWController.Instance.RuntimeDataStorage.RuntimeData.ItemsCurrent;
            if (itemsCurrent > 0)
            {
               stringBuilder.AppendLine($"TIP: Sell all items before turn {turnsTotal}.");
            }
            
            GPWHelper.PlayAudioClipSecondaryClick();
            
            _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
               _scene02GameUIView.DialogSystem.DialogUIPrefab,
               $"Turn {turnCurrent}/{turnsTotal} - Game Over! ",
               stringBuilder.ToString(),
               new List<DialogButtonData>
               {
                  new DialogButtonData($"Submit Score", async delegate
                  {
                     GPWHelper.PlayAudioClipSecondaryClick();
                     await GPWController.Instance.GameServices.SetLeaderboardScoreAndWriteAlias(score);
                     _scene02GameUIView.DialogSystem.HideDialogBox();
                     QuitGameSafe(false);
                  }),
                  new DialogButtonData("Quit", delegate
                  {
                     GPWHelper.PlayAudioClipSecondaryClick();
                     _scene02GameUIView.DialogSystem.HideDialogBox();
                     QuitGameSafe(false);
                  })
               });
         }
   
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

         _isReadyRuntimeDataStorage = true;
         CheckIsSceneReady();
      }

      private void ProductContentList_OnInitialized(ScrollingList scrollingList)
      {
         foreach (ListBox listBox in scrollingList.ListBoxes)
         {
            ProductContentListItem productContentListItem = listBox as ProductContentListItem;
            
            productContentListItem.OnBuy.RemoveAllListeners();
            productContentListItem.OnSell.RemoveAllListeners();
            //
            productContentListItem.OnBuy.AddListener(ProductContentListItem_OnBuy);
            productContentListItem.OnSell.AddListener(ProductContentListItem_OnSell);
         }
         
         _isReadyProductContentList = true;
         CheckIsSceneReady();
      }
      
      private void GameServices_OnInventoryViewChanged(InventoryView inventoryView)
      {
         _isReadyInventoryView = true;
         CheckIsSceneReady();
         
      }
      

   }
}