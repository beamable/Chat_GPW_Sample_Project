using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Components;
using Beamable.Samples.Core.Debugging;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
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
                await ResetGame();
                IsInitialized = true;
            }
        }
        
        public void GoToLocation()
        {
            if (_persistentDataStorage.PersistentData.IsGameOver)
            {
                return;
            }
            
            var locationContentViewCurrent = PersistentDataStorage.PersistentData.LocationContentViewCurrent;
            var locationContentViews = RuntimeDataStorage.RuntimeData.LocationContentViews;
            int currentIndex = 0;
            foreach (LocationContentView locationContentView in locationContentViews)
            {
                if (locationContentView.LocationContent.Id == locationContentViewCurrent.LocationContent.Id)
                {
                    break;
                }
                currentIndex++;
            }

            int nextIndex = currentIndex + 1;
            if (nextIndex > locationContentViews.Count -1)
            {
                nextIndex = 0;
            }

            // Progress Turn / Bank / Debt
            GoToNextTurn();
            
            // Change Location
            PersistentDataStorage.PersistentData.LocationContentViewCurrent =
                locationContentViews[nextIndex];
            PersistentDataStorage.ForceRefresh();
        }

        
        public double CalculatedCurrentScore()
        {
            int cashAmount = GPWController.Instance.PersistentDataStorage.PersistentData.CashAmount;
            int bankAmount = GPWController.Instance.PersistentDataStorage.PersistentData.BankAmount;
            int debtAmount = GPWController.Instance.PersistentDataStorage.PersistentData.DebitAmount;

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
            float debtAmount = PersistentDataStorage.PersistentData.DebitAmount +
                               (PersistentDataStorage.PersistentData.DebitAmount * RuntimeDataStorage.RuntimeData.DebtInterestCurrent);
            PersistentDataStorage.PersistentData.DebitAmount = (int)debtAmount;
            
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
            if (_persistentDataStorage.PersistentData.CashAmount - amountToPayoffDebt >= 0 && 
                _persistentDataStorage.PersistentData.DebitAmount + amountToPayoffDebt >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount += amountToPayoffDebt;
                _persistentDataStorage.PersistentData.DebitAmount += amountToPayoffDebt;
                _persistentDataStorage.ForceRefresh();
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
                TransferCashToSellItem(productContentView.OwnedGoods.Price * amount);
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
                    LocationContent locationContent = _persistentDataStorage.
                        PersistentData.LocationContentViewCurrent.LocationContent;
                    currentRoomName = GPWHelper.GetChatRoomNameLocation(locationContent);
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
        
        public async Task<EmptyResponse> ResetGame()
        {
            /////////////////////////////
            // Systems
            /////////////////////////////
            await _gameServices.Initialize(_configuration);
            await _runtimeDataStorage.Initialize(_configuration);
            await _persistentDataStorage.Initialize(_configuration);
        
            /////////////////////////////
            // Money
            /////////////////////////////
            _persistentDataStorage.PersistentData.BankAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.BankAmountInitial;
            
            _persistentDataStorage.PersistentData.CashAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.CashAmountInitial;
            
            _persistentDataStorage.PersistentData.DebitAmount = 
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
            
            // Chat - Direct Room - TODO: Should I use the existing 'DirectRooms' 
            // available on the chatView instance?
            await _gameServices.CreateRoomSafe(GPWHelper.GetChatRoomNameDirect());
            
            // Chat - Location Rooms
            foreach (LocationContentView locationContentView in _runtimeDataStorage.RuntimeData.LocationContentViews)
            {
                await _gameServices.CreateRoomSafe(
                    GPWHelper.GetChatRoomNameLocation(locationContentView.LocationContent));
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
        
        
        public async Task<EmptyResponse> RefreshCurrentProductContentViews()
        {
            List<ProductContentView> list = _persistentDataStorage.PersistentData.
                LocationContentViewCurrent.ProductContentViews;

            Debug.Log("RefreshCurrentProductContentViews()");
            
            // Refresh the UI buttons for buy/sell
            // Can we buy/sell at least one quantity?
            foreach (ProductContentView productContentView in list)
            {
                productContentView.CanBuy = CanBuyItem(productContentView, 1);
                productContentView.CanSell = CanSellItem(productContentView, 1);
                
                string contentId = productContentView.ProductContent.Id;
                productContentView.OwnedGoods.Quantity = await _gameServices.GetOwnedItemQuantity(contentId);
                productContentView.OwnedGoods.Price = await _gameServices.GetOwnedItemAveragePrice(contentId);
            }

            return new EmptyResponse();
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
        }
      
      
        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            Configuration.Debugger.Log("GPWController.RuntimeDataStorage_OnChanged()", 
                DebugLogLevel.Verbose);
            
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
            InventoryView inventoryView = runtimeDataStorage.RuntimeData.InventoryView;

            if (_persistentDataStorage?.PersistentData?.LocationContentViewCurrent == null &&
                runtimeDataStorage?.RuntimeData?.LocationContentViews != null && 
                runtimeDataStorage?.RuntimeData?.LocationContentViews.Count > 0)
            {
                _persistentDataStorage.PersistentData.LocationContentViewCurrent = 
                    runtimeDataStorage.RuntimeData.LocationContentViews[0];
            }
        }


    }
}
