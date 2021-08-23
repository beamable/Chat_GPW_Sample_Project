using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.Components;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Storage
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class GPWSingleton : SingletonMonobehavior<GPWSingleton>
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; } protected set { _isInitialized = value; } }
        public RuntimeDataStorage RuntimeDataStorage { get { return _runtimeDataStorage; } }
        public PersistentDataStorage PersistentDataStorage { get { return _persistentDataStorage; } }
        public GameServices GameServices { get { return _gameServices; } }
        
        //  Fields  --------------------------------------
        private const string ContentType = "items.product";
        private RuntimeDataStorage _runtimeDataStorage = new RuntimeDataStorage();
        private PersistentDataStorage _persistentDataStorage = new PersistentDataStorage();
        private GameServices _gameServices = new GameServices();
        private bool _isInitialized = false;
        private InventoryView _inventoryView = null;

        //  Unity Methods  --------------------------------

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration)
        {
            if (!IsInitialized)
            {
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                await _gameServices.Initialize(configuration);
                
                // Filtered items (Owned by current player)
                _gameServices.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);
                await _runtimeDataStorage.Initialize(configuration);
                await _persistentDataStorage.Initialize(configuration);

                // Money
                _persistentDataStorage.PersistentData.BankAmount =
                    _gameServices.RemoteConfiguration.BankAmountInitial;
                _persistentDataStorage.PersistentData.CashAmount =
                    _gameServices.RemoteConfiguration.CashAmountInitial;
                _persistentDataStorage.PersistentData.DebitAmount =
                    _gameServices.RemoteConfiguration.DebtAmountInitial;

                // Turns
                _persistentDataStorage.PersistentData.TurnCurrent = 0;
                _persistentDataStorage.PersistentData.TurnsTotal = 
                    _gameServices.RemoteConfiguration.TurnsTotal;
                
                // Items
                _persistentDataStorage.PersistentData.ItemsMax = 
                    _gameServices.RemoteConfiguration.ItemsMax;
                
                IsInitialized = true;
                _persistentDataStorage.OnRefreshed.Invoke(_persistentDataStorage);
            }
        }

        public async Task<bool> CanBuyItem(string contentId, int amount)
        {
            return true;
        }

        public async Task<bool> BuyItem(string contentId, int  amount)
        {
            InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
            for (int i = 0; i < amount; i++)
            {
                inventoryUpdateBuilder.AddItem(contentId);
            }
            GameServices.InventoryService.Update(inventoryUpdateBuilder);

            return true;
        }

        public async Task<bool> CanSellItem(string contentId, int amount)
        {
            return true;
        }

        public async Task<bool> SellItem(string contentId, int amount)
        {
            List<ItemView> productContents = await GetItems(contentId);

            if (productContents.Count < amount)
            {
                return false;
            }

            InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
            for (int i = 0; i < amount; i++)
            {
                Debug.Log("del: " + productContents[i].id);
                inventoryUpdateBuilder.DeleteItem(contentId, productContents[i].id); 
            }
            
            await GameServices.InventoryService.Update(inventoryUpdateBuilder);
            return true;
        }
        
        private async Task<List<ItemView>> GetItems(string contentId)
        {
            Debug.Log("contentId: " + contentId);
            foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
            {
                string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
                Debug.Log("inventoryItemName: " + inventoryItemName);
                return kvp.Value;
            }

            return new List<ItemView>();
        }
        
        //  Event Handlers  -------------------------------
        private void InventoryService_OnChanged(InventoryView inventoryView)
        {
            _inventoryView = inventoryView;
            
            Debug.Log($"#2. PLAYER - InventoryService, count = {_inventoryView.items.Count}");

            int itemsCurrent = 0;
            foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
            {
                string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
                Debug.Log("OnChanged() inventoryItemName: " + inventoryItemName);
                itemsCurrent += kvp.Value.Count;
            }
            
            _persistentDataStorage.PersistentData.ItemsCurrent = itemsCurrent;
            _persistentDataStorage.OnRefreshed.Invoke(_persistentDataStorage);
        }
    }
}
