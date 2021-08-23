using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api.Inventory;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.GPW.Content;
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
	        List<ItemView> itemViews = await GetItemViews(contentId);
	        return itemViews.Count >= amount;
        }

        public async Task<bool> SellItem(string contentId, int amount)
        {
            List<ItemView> itemViews = await GetItemViews(contentId);

            if (itemViews.Count < amount)
            {
                return false;
            }

            InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
            inventoryUpdateBuilder.deleteItems.Clear();

            int deletedAlready = 0;
            foreach (ItemView itemView in itemViews)
            {
	            if (deletedAlready++ > amount)
	            {
		            break;
	            }
	            Debug.Log($"DeleteItem() contentId={contentId}, itemView.id={itemView.id}");
	           inventoryUpdateBuilder.DeleteItem(contentId, itemView.id);
            }
            
            await _inventoryService.Update(inventoryUpdateBuilder);
            return true;
        }
        
        private async Task<List<ItemView>> GetItemViews(string contentId)
        {
            foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
            {
                string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
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
