using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api.Inventory;
using Beamable.Common.Api.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Samples.GPW.Data
{
	public class GameServicesEvent : UnityEvent<GameServices>{}
	public class InventoryViewEvent : UnityEvent<InventoryView>{}
	
	/// <summary>
	/// Game-specific wrapper for calling Beamable online services
	/// </summary>
	public class GameServices 
	{
		//  Events  --------------------------------------
		public GameServicesEvent OnGameServicesChanged = new GameServicesEvent();
		public InventoryViewEvent OnInventoryViewChanged = new InventoryViewEvent();
		
		//  Properties  ----------------------------------
		public InventoryService InventoryService { get { return _inventoryService; }}
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }

		//  Fields  --------------------------------------
		private IBeamableAPI _beamableAPI = null;
		private InventoryService _inventoryService = null;
		private InventoryView _inventoryView = null;
		private bool _isInitialized = false;
		private const string ContentType = "items.product";
		private long _localPlayerDbid;

		//  Unity Methods  --------------------------------

		//  Other Methods  --------------------------------
		public async Task Initialize(Configuration configuration)
		{
			if (!_isInitialized)
			{
				_beamableAPI = await Beamable.API.Instance;
				_localPlayerDbid = _beamableAPI.User.id;
				
				_inventoryService = _beamableAPI.InventoryService;
				_inventoryService.Subscribe(ContentType, InventoryService_OnChanged);

				_isInitialized = true;
			}

			OnGameServicesChanged.Invoke(this);
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
            _inventoryService.Update(inventoryUpdateBuilder);

            return true;
        }

        public async Task<bool> CanSellItem(string contentId, int amount)
        {
	        List<ItemView> itemViews = await GetItems(contentId);
	        return itemViews.Count >= amount;
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
            
            await _inventoryService.Update(inventoryUpdateBuilder);
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
            OnInventoryViewChanged.Invoke(inventoryView);
        }
	}
}
