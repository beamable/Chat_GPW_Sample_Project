using System.Collections.Generic;
using System.Text;
using AirFishLab.ScrollingList;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
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
        private bool _isReadyProductContentListFirstTime = false;
        private bool _isReadyInventoryView = false;
      
        //  Unity Methods   ------------------------------
        protected void Start()
        {
         
            // Clear UI
            _scene02GameUIView.ProductContentList.IsVisible = false;

            // Top Navigation
            _scene02GameUIView.TravelButton.onClick.AddListener(TravelButton_OnClicked);
            _scene02GameUIView.BankButton.onClick.AddListener(BankButton_OnClicked);
            _scene02GameUIView.DebtButton.onClick.AddListener(DebtButton_OnClicked);
         
            // Bottom Navigation
            _scene02GameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
            _scene02GameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
            _scene02GameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         
            // Load
            _scene02GameUIView.DialogSystem.DelayBeforeHideDialogBox =
                (int)_scene02GameUIView.Configuration.DelayAfterDataLoading * 1000;
            _scene02GameUIView.DialogSystem.ShowDialogBoxLoading(GPWHelper.Game);
            SetupBeamable();
         
        }
      

        //  Other Methods  -----------------------------
        private async void SetupBeamable()
        {
            // Setup List
            _scene02GameUIView.ProductContentList.OnInitialized.AddListener(ProductContentList_OnChanged);
         
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
            if (_isReadyRuntimeDataStorage && _isReadyInventoryView)
            {
                // Likely often. That's ok
                RefreshProductContentList();
            
                // Limit this to one time ever per session per-this-scene
                if (!_isReadyProductContentListFirstTime && _isReadyProductContentList)
                {
                    _isReadyProductContentListFirstTime = true;
               
                    // Likely once when scene starts
                    await _scene02GameUIView.DialogSystem.HideDialogBox();
                }
            
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
      
      
        private async void CheckIsGameOver()
        {
            PersistentData persistentData = GPWController.Instance.PersistentDataStorage.PersistentData;
            if (persistentData.IsGameOver)
            {
                //
                int turnCurrent = persistentData.TurnCurrent;
                int turnsTotal = persistentData.TurnsTotal;
                //
                int cashAmount = persistentData.CashAmount;
                int bankAmount = persistentData.BankAmount;
                int debtAmount = persistentData.DebtAmount;
                double score = GPWController.Instance.GetCalculatedCurrentScore();
            
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

                if (_scene02GameUIView.DialogSystem.HasCurrentDialogUI)
                {
                    await _scene02GameUIView.DialogSystem.HideDialogBoxImmediate();
                }
            
                _scene02GameUIView.DialogSystem.ShowDialogBox<DialogUI>(
                    _scene02GameUIView.DialogSystem.DialogUIPrefab,
                    $"Turn {turnCurrent}/{turnsTotal} - Game Over! ",
                    stringBuilder.ToString(),
                    new List<DialogButtonData>
                    {
                        new DialogButtonData(GPWHelper.SubmitScore, async delegate
                        {
                            GPWHelper.PlayAudioClipSecondaryClick();
                            await GPWController.Instance.GameServices.GetOrCreateAliasAndSetLeaderboardScore(score);
                            await _scene02GameUIView.DialogSystem.HideDialogBoxImmediate();
                            QuitGameSafe(false);
                        }),
                        new DialogButtonData(GPWHelper.Quit, async delegate
                        {
                            GPWHelper.PlayAudioClipSecondaryClick();
                            await _scene02GameUIView.DialogSystem.HideDialogBoxImmediate();
                            QuitGameSafe(false);
                        })
                    });
            }
        }
       

      
        //  Event Handlers -------------------------------
      
        private void TravelButton_OnClicked()
        {
            GPWHelper.PlayAudioClipSecondaryClick();

            int currentLocationIndex = GPWController.Instance.PersistentDataStorage.PersistentData.CurrentLocationIndex;
            List<LocationContentView> locationContentViews = GPWController.Instance.RuntimeDataStorage.RuntimeData.LocationContentViews;
         
            GPWHelper.ShowDialogLocation(_scene02GameUIView.DialogSystem, 
                locationContentViews,
                currentLocationIndex, 
                delegate(int nextLocationIndex)
                {
                    if (currentLocationIndex != nextLocationIndex)
                    {
                        GPWController.Instance.SetLocationIndexSafe(nextLocationIndex, true);
                    }
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
                _scene02GameUIView.Configuration.Scene05LeaderboardName,
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

            int cashTransactionMin = GPWController.Instance.RuntimeDataStorage.RuntimeData.CashTransactionMin;
         
            GPWHelper.ShowDialogBank(_scene02GameUIView.DialogSystem, cashTransactionMin, 
                delegate(int amountToAddToBank)
                {
                    GPWController.Instance.TransferCashToBank(amountToAddToBank);
                });
        }


        private void DebtButton_OnClicked()
        {
            GPWHelper.PlayAudioClipSecondaryClick();
         
            int cashTransactionMin = GPWController.Instance.RuntimeDataStorage.RuntimeData.CashTransactionMin;
         
            GPWHelper.ShowDialogDebt(_scene02GameUIView.DialogSystem, cashTransactionMin, 
                delegate(int amountToPayoffDeb)
                {
                    GPWController.Instance.TransferCashToDebt(amountToPayoffDeb);
                });
        }


        private void ProductContentListItem_OnBuy(ProductContentView productContentView)
        {
            GPWHelper.ShowDialogBoxBuy(
                _scene02GameUIView.DialogSystem, 
                productContentView, 
                async delegate (int updatedAmount)
                {
                    GPWHelper.PlayAudioClipSecondaryClick();

                    if (updatedAmount > 0)
                    {
                        _scene02GameUIView.DialogSystem.CurrentDialogUI.IsInteractable = false;
                        bool isSuccessful = await GPWController.Instance.BuyItem(productContentView, updatedAmount);
                    }
                    await _scene02GameUIView.DialogSystem.HideDialogBoxImmediate();
                });
        }
      
      
        private void ProductContentListItem_OnSell(ProductContentView productContentView)
        {
            GPWHelper.ShowDialogBoxSell(
                _scene02GameUIView.DialogSystem, 
                productContentView, 
                async delegate (int updatedAmount)
                {
                    GPWHelper.PlayAudioClipSecondaryClick();

                    if (updatedAmount > 0)
                    {
                        _scene02GameUIView.DialogSystem.CurrentDialogUI.IsInteractable = false;
                        bool isSuccessful = await GPWController.Instance.SellItem(productContentView, updatedAmount);
                    }
               
                    await _scene02GameUIView.DialogSystem.HideDialogBoxImmediate();
                });
        }
      
      
        private void PersistentDataStorage_OnChanged(SubStorage subStorage)
        {
            PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
            _scene02GameUIView.PersistentData = persistentDataStorage.PersistentData;
            _scene02GameUIView.LocationContentView = GPWController.Instance.LocationContentViewCurrent;
         
            // When changes occur (e.g. CashAmount), refresh
            // list so buy/sell interactable can toggle properly
            RefreshProductContentList();
         
            // When PersistentData changes, check "is last turn?"
            CheckIsGameOver();
        }
     

        private async void RefreshProductContentList()
        {
            if (GPWController.Instance.HasLocationContentViewCurrent)
            {
                GPWController.Instance.RefreshCurrentProductContentViews();
            
                List<ProductContentView> list = GPWController.Instance.LocationContentViewCurrent.ProductContentViewCollection.ProductContentViews;
         
                // This rebuilds the list...
                // 1. Keeps vertical list scroll. Good!
                // 2. but refreshes the contents based on the ProductContentViews. Good!
                await _scene02GameUIView.ProductContentList.InitializeOnDelay(list, 100);
                _scene02GameUIView.ProductContentList.Refresh();
         
                if (!_scene02GameUIView.ProductContentList.IsVisible)
                {
                    _scene02GameUIView.ProductContentList.IsVisible = true;
                }
            }
        }


        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
            _scene02GameUIView.RuntimeData = runtimeDataStorage.RuntimeData;

            _isReadyRuntimeDataStorage = true;
            CheckIsSceneReady();
        }

      
        private void ProductContentList_OnChanged(ScrollingList scrollingList)
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