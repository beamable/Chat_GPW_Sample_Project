using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Components;
using Beamable.Samples.Core.Debugging;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
using Beamable.Samples.GPW.Data.Factories;
using Beamable.Samples.GPW.Data.Storage;
using UnityEngine;

namespace Beamable.Samples.GPW
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class GPWController : SingletonMonobehavior<GPWController>
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; } protected set { _isInitialized = value; } }
        public RuntimeDataStorage RuntimeDataStorage { get { return _runtimeDataStorage; } }
        public PersistentDataStorage PersistentDataStorage { get { return _persistentDataStorage; } }
        public GameServices GameServices { get { return _gameServices; } }
        public bool HasCurrentRoomHandle {  get  {  return GetCurrentRoomHandle() != null;  } }

        public LocationContentView LocationContentViewCurrent
        {
            get
            {
                if (_persistentDataStorage.PersistentData.CurrentLocationIndex == GPWHelper.UnsetValue)
                {
                    return null;
                }
                return _runtimeDataStorage.RuntimeData.LocationContentViews[_persistentDataStorage.PersistentData.CurrentLocationIndex];
            }
        }
        
        public bool HasLocationContentViewCurrent
        {
            get
            {
                try
                {
                    // May fail during startup. That's ok.
                    return LocationContentViewCurrent != null;
                }
                catch 
                {
                    return false;
                }
            }
        }

        //  Fields  --------------------------------------
        private RuntimeDataStorage _runtimeDataStorage = new RuntimeDataStorage();
        private PersistentDataStorage _persistentDataStorage = new PersistentDataStorage();
        private GameServices _gameServices = new GameServices();
        private Configuration _configuration = null;
        private bool _isInitialized = false;

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration2)
        {
            if (!IsInitialized)
            {
                /////////////////////////////
                // GameServices
                /////////////////////////////
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                _configuration = configuration2;
                _gameServices.OnInventoryViewChanged.AddListener(InventoryService_OnChanged);
                _runtimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
                _persistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
                await ResetController();
                IsInitialized = true;
            }
        }

        
        public void SetLocationIndexSafe(int nextIndex, bool willIncrementTurn)
        {
            if (_persistentDataStorage.PersistentData.IsGameOver)
            {
                return;
            }
            
            // Too high? Wrap low
            if (nextIndex > RuntimeDataStorage.RuntimeData.LocationContentViews.Count - 1)
            {
                nextIndex = 0;
            }
            
            // Too low? Wrap high
            if (nextIndex < 0)
            {
                nextIndex = RuntimeDataStorage.RuntimeData.LocationContentViews.Count - 1;
            }

            // Progress Turn / Bank / Debt
            if (willIncrementTurn)
            {
                GoToNextTurn();
            }
            
            
            // Change Location
            PersistentDataStorage.PersistentData.CurrentLocationIndex = nextIndex;
            PersistentDataStorage.ForceRefresh();
            GameServices.ForceRefresh();
        }


        public double GetCalculatedCurrentScore()
        {
            int cashAmount = _persistentDataStorage.PersistentData.CashAmount;
            int bankAmount = _persistentDataStorage.PersistentData.BankAmount;
            int debtAmount = _persistentDataStorage.PersistentData.DebtAmount;

            return cashAmount + bankAmount - debtAmount;
        }

        
        private void GoToNextTurn()
        {
            if (_persistentDataStorage.PersistentData.IsGameOver)
            {
                return;
            }

            // Calculate interest - BankAmount 
            float bankAmount = PersistentDataStorage.PersistentData.BankAmount +
                (PersistentDataStorage.PersistentData.BankAmount * RuntimeDataStorage.RuntimeData.BankInterestCurrent);
            PersistentDataStorage.PersistentData.BankAmount = (int)bankAmount;
            
            // Calculate interest - BankAmount 
            float debtAmount = PersistentDataStorage.PersistentData.DebtAmount +
                               (PersistentDataStorage.PersistentData.DebtAmount * RuntimeDataStorage.RuntimeData.DebtInterestCurrent);
            PersistentDataStorage.PersistentData.DebtAmount = (int)debtAmount;
            
            // Advance the turn counter
            PersistentDataStorage.PersistentData.TurnCurrent++;

        }
        
        
        public void TransferCashToBank(int amountToAddToBank)
        {
            // The amount may be positive or negative
            // Check that both balances will be above zero
            if (_persistentDataStorage.PersistentData.CashAmount - amountToAddToBank >= 0 && 
                _persistentDataStorage.PersistentData.BankAmount + amountToAddToBank >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount -= amountToAddToBank;
                _persistentDataStorage.PersistentData.BankAmount += amountToAddToBank;
                _persistentDataStorage.ForceRefresh();
            }
        }
      
        public void TransferCashToDebt(int amountToPayoffDebt)
        {
            // The amount may be positive or negative
            // Check that both balances will be above zeros
            
            // NEGATIVE amountToPayoffDebt = INCREASE DEBT
            int nextCashAmount = _persistentDataStorage.PersistentData.CashAmount + amountToPayoffDebt;
            int nextDebtAmount = _persistentDataStorage.PersistentData.DebtAmount + amountToPayoffDebt;
            
            if (nextCashAmount >= 0)
            {
                // You can add debt but only up to the original debt amount. You can't add extra.
                // You can remove debt but only up to 0 debt. You can't remove extra.
                if (nextDebtAmount >= 0 && 
                    (nextDebtAmount <= _runtimeDataStorage.RuntimeData.RemoteConfiguration.DebtAmountInitial ||
                     nextDebtAmount <= _persistentDataStorage.PersistentData.DebtAmount))
                {
                    _persistentDataStorage.PersistentData.CashAmount = nextCashAmount;
                    _persistentDataStorage.PersistentData.DebtAmount = nextDebtAmount;
                    _persistentDataStorage.ForceRefresh();
                }
            }
        }
        
        public void TransferCashToBuyItem(int itemPurchasePrice)
        {
            if (_persistentDataStorage.PersistentData.CashAmount - itemPurchasePrice >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount -= itemPurchasePrice;
                _persistentDataStorage.ForceRefresh();
            }
        }
        
        public void TransferCashToSellItem(int ItemSalePrice)
        {
            if (_persistentDataStorage.PersistentData.CashAmount + ItemSalePrice >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount += ItemSalePrice;
                _persistentDataStorage.ForceRefresh();
            }
        }
        
        public bool CanBuyItem(ProductContentView productContentView, int amount)
        {
            int totalQuantity = productContentView.MarketGoods.Quantity;
            bool isQuanity = totalQuantity >= amount;
            
            int totalPrice = productContentView.MarketGoods.Price * amount;
            bool isPrice = _persistentDataStorage.PersistentData.CashAmount >= totalPrice;

            return isPrice && isQuanity;
        }

        public async Task<bool> BuyItem(ProductContentView productContentView, int amount)
        {
            if (CanBuyItem(productContentView, amount))
            {
                TransferCashToBuyItem(productContentView.MarketGoods.Price * amount);
                return await _gameServices.BuyItemInternal(productContentView, amount);
            }

            return false;
        }

        
        public bool CanSellItem(ProductContentView productContentView, int amount)
        {
            int totalQuantity = productContentView.OwnedGoods.Quantity;
            bool isQuanity = totalQuantity >= amount;
            
            return isQuanity;
        }

        
        public async Task<bool> SellItem(ProductContentView productContentView, int amount)
        {
            if (CanSellItem(productContentView, amount))
            {
                TransferCashToSellItem(productContentView.MarketGoods.Price * amount);
                return await _gameServices.SellItemInternal(productContentView, amount);
            }

            return false;
        }
        
        public RoomHandle GetCurrentRoomHandle()
        {
            return GetRoomHandleForChatMode(_runtimeDataStorage.RuntimeData.ChatMode);
        }

        public RoomHandle GetRoomHandleForChatMode(ChatMode chatMode)
        {
            string currentRoomName = "";
			
            switch (chatMode )
            {
                case ChatMode.Global:
                    currentRoomName = GPWHelper.ChatRoomNameGlobal;
                    break;
                case ChatMode.Location:
                    currentRoomName = GPWHelper.GetChatRoomNameLocation(
                        LocationContentViewCurrent.LocationData);
                    break;
                case ChatMode.Direct:
                    currentRoomName = GPWHelper.GetChatRoomNameDirect();
                    break;
                default:
                    //Throw no exception, returning null is ok
                    break;
            }

            if (_gameServices == null)
            {
                return null;
            }
            
            if (!_gameServices.HasRoom(currentRoomName))
            {
                return null;
            }
            
            return _gameServices.GetRoom(currentRoomName);
        }
        
        /// <summary>
        /// Recreates the user account thus behaving like a first
        /// time player
        /// </summary>
        public void ResetPlayerData()
        {
            //NOTE: This is a bit hacky and it resets heavy-handed
            GPWHelper.PlayAudioClipSecondaryClick();
            GameObject.Destroy(GPWController.Instance.gameObject);
            ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
        }
        
        /// <summary>
        /// Recreates the location/product/price data
        /// </summary>
        public async void ResetGameDataViaDataFactory()
        {
            ///////////////////////
            // FACTORY: Populate Locations, each with products
            ///////////////////////
            await _runtimeDataStorage.ResetGameDataViaDataFactory();
        }
        
        private async Task<EmptyResponse> ResetController()
        {
            /////////////////////////////
            // DataFactoryType
            /////////////////////////////
            IDataFactory _dataFactory = null;
            
            // There are multiple repos to show various features
            switch (_configuration.DataFactoryType)
            {
                case DataFactoryType.BasicDataFactory:
                    
                    //TODO: Remove this
                    _dataFactory = new GPWBasicDataFactory();
                    break;
                    
                    //TODO: Throw error instead
                    // throw new Exception(
                    //     "The project (https://github.com/beamable/Chat_GPW_2_With_MicroStorage_Sample_Project ) " +
                    //     "does not support Beamable BasicDataFactory. " +
                    //     
                    //     "Instead see project " +
                    //     "(https://github.com/beamable/Chat_GPW_Sample_Project).");
                case DataFactoryType.MicroStorage:
                    // Use this as the required value for this project
                    _dataFactory = new GPWMicroStorageDataFactory();
                    break;
                default:
                    // Must set this value properly via Configuration via inspector
                    throw new SwitchDefaultException(_configuration.DataFactoryType);
            }
            
            /////////////////////////////
            // Systems
            /////////////////////////////
            await _gameServices.Initialize(_configuration);
            await _runtimeDataStorage.Initialize(_configuration, _dataFactory);
            await _persistentDataStorage.Initialize(_configuration);
        
            /////////////////////////////
            // Money
            /////////////////////////////
            _persistentDataStorage.PersistentData.BankAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.BankAmountInitial;
            
            _persistentDataStorage.PersistentData.CashAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.CashAmountInitial;
            
            _persistentDataStorage.PersistentData.DebtAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.DebtAmountInitial;
            
            /////////////////////////////
            // Chat
            /////////////////////////////
            while (!_gameServices.HasChatView)
            {
                await Task.Delay(100);
            }
            
            // Chat - Global Room
            await _gameServices.CreateRoomSafe(GPWHelper.ChatRoomNameGlobal);
            
            // Chat - Direct Room 
            // available on the chatView instance?
            await _gameServices.CreateRoomSafe(GPWHelper.GetChatRoomNameDirect());
            
            // Chat - Location Room(s)
            foreach (LocationContentView locationContentView in _runtimeDataStorage.RuntimeData.LocationContentViews)
            {
                await _gameServices.CreateRoomSafe(
                    GPWHelper.GetChatRoomNameLocation(locationContentView.LocationData));
            }
            
            // When running this scene directly, go back to intro scene
            // When running in production, go to the previous scene
            _runtimeDataStorage.RuntimeData.PreviousSceneName = _configuration.Scene01IntroName;
            
            // Default to global chat
            _runtimeDataStorage.RuntimeData.ChatMode = ChatMode.Global;

            /////////////////////////////
            // Turns
            /////////////////////////////
            _persistentDataStorage.PersistentData.TurnCurrent = 1;
            _persistentDataStorage.PersistentData.TurnsTotal = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.TurnsTotal;
            _persistentDataStorage.OnChanged.Invoke(_persistentDataStorage);

            return new EmptyResponse();
        }
        
        
        public void RefreshCurrentProductContentViews()
        {
            if (HasLocationContentViewCurrent)
            {
                List<ProductContentView> list = LocationContentViewCurrent.ProductContentViews;

                Configuration.Debugger.Log("RefreshCurrentProductContentViews()", DebugLogLevel.Verbose);
            
                // Refresh the UI buttons for buy/sell
                // Can we buy/sell at least one quantity?
                foreach (ProductContentView productContentView in list)
                {
                    productContentView.CanBuy = CanBuyItem(productContentView, 1);
                    productContentView.CanSell = CanSellItem(productContentView, 1);
                
                    string contentId = productContentView.ProductData.Id;
                    productContentView.OwnedGoods.Quantity = _gameServices.GetOwnedItemQuantity(contentId);
                    productContentView.OwnedGoods.Price = _gameServices.GetOwnedItemAveragePrice(contentId);
                }
            }

        }
        
        
        //  Event Handlers  -------------------------------
        private void InventoryService_OnChanged(InventoryView inventoryView)
        {
            Configuration.Debugger.Log("GPWController.InventoryService_OnChanged()", 
                DebugLogLevel.Verbose);
            
            _runtimeDataStorage.RuntimeData.InventoryView = inventoryView;
            _runtimeDataStorage.ForceRefresh();
        }
        
        
        private void PersistentDataStorage_OnChanged(SubStorage subStorage)
        {
            Configuration.Debugger.Log("GPWController.PersistentDataStorage_OnChanged()", 
                DebugLogLevel.Verbose);
            
            PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
            
            if (!HasLocationContentViewCurrent)
            {
                SetLocationIndexSafe(0, false);
            }
        }
      
      
        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            Configuration.Debugger.Log("GPWController.RuntimeDataStorage_OnChanged()", 
                DebugLogLevel.Verbose);
            
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
        }
    }
}
