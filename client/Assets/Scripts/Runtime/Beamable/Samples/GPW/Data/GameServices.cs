using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api.Inventory;
using Beamable.Common.Api.Inventory;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Storage;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Samples.GPW.Data
{
	public class InventoryViewEvent : UnityEvent<InventoryView>{}
	public class ChatViewEvent : UnityEvent<ChatView>{}
	
	/// <summary>
	/// Game-specific wrapper for calling Beamable online services
	/// </summary>
	public class GameServices 
	{
		//  Events  --------------------------------------
		public InventoryViewEvent OnInventoryViewChanged = new InventoryViewEvent();
		public ChatViewEvent OnChatViewChanged = new ChatViewEvent();
		
		//  Properties  ----------------------------------
		public InventoryService InventoryService { get { return _inventoryService; }}
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }
		public bool HasChatView { get { return ChatView != null; }}
		public ChatView ChatView { get { return _chatView; }}
		
		//  Fields  --------------------------------------
		private ChatService _chatService = null;
		private ChatView _chatView = null;
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
				
				// InventoryService
				_inventoryService = _beamableAPI.InventoryService;
				_inventoryService.Subscribe(ContentType, InventoryService_OnChanged);
				
				// ChatService
				_chatService = _beamableAPI.Experimental.ChatService;
				_chatService.Subscribe(ChatService_OnChanged);
				
				_isInitialized = true;
			}
		}

		#region ChatService


		
		private bool HasRoom(string roomName)
		{
			foreach (RoomHandle roomHandle in _chatView.roomHandles)
			{
				Debug.Log("Check: " + roomHandle.Name);
				if (roomHandle.Name == roomName)
				{
					return true;
				}
			}

			return false;
		}
		
		private bool IsLocalPlayerInRoom(string roomName)
		{
			foreach (RoomHandle roomHandle in _chatView.roomHandles)
			{
				if (roomHandle.Players.Contains(_localPlayerDbid)) ;
				{
					return true;
				}
			}
			return false;
		}

		public RoomHandle GetRoomHandle(string roomName)
		{
			foreach (RoomHandle roomHandle in _chatView.roomHandles)
			{
				if (roomHandle.Name == roomName)
				{
					return roomHandle;
				}
			}

			throw new Exception("can't find id for roomName: " + roomName);
		}

		public async Task<bool> CreateRoomSafe(string roomName)
		{
			bool wasSuccesfullyCreated = false;
			
			if (_chatView == null)
			{
				throw new Exception("CreateRoomSafe() failed. _chatView must be not null.");
			}

			bool alreadyHasRoom = HasRoom(roomName);
			if (!alreadyHasRoom)
			{
				RoomInfo roomInfo = await _chatService.CreateRoom(roomName, 
					true, 
					new List<long> {_localPlayerDbid});

				wasSuccesfullyCreated = roomInfo.name == roomName;
				
				Debug.Log("88CREATED: " + roomName + " succ: " + wasSuccesfullyCreated);
			}
			else
			{
				Debug.Log("88FOUND: " + roomName + " succ: " + alreadyHasRoom);
			}

			return alreadyHasRoom || wasSuccesfullyCreated;
		}

		public async Task<bool> JoinRoom(string roomName)
		{
			if (!HasRoom(roomName))
			{
				Debug.LogError("Room does not exist");
				return false;
			}
			
			if (IsLocalPlayerInRoom(roomName))
			{
				Debug.LogError("Local player is not in this room");
				return true;
			}
			else
			{
				RoomHandle roomHandle = GetRoomHandle(roomName);
				List<long> players = roomHandle.Players;
				players.Add(_localPlayerDbid);
				await _chatService.CreateRoom(roomHandle.Id, true, players);
			}

			return true;
		}
		
		public async Task<bool> LeaveRoom(string roomName)
		{
			if (!HasRoom(roomName))
			{
				Debug.LogError("Room does not exist");
				return false;
			}
			
			if (!IsLocalPlayerInRoom(roomName))
			{
				Debug.LogError("Local player is not in this room");
				return false;
			}

			await _chatService.LeaveRoom(GetRoomHandle(roomName).Id);

			return true;
		}

		public async Task<bool> SendMessage(string roomName, string messageToSend)
		{
			if (!HasRoom(roomName))
			{
				Debug.LogError("Room does not exist");
				return false;
			}
			
			if (!IsLocalPlayerInRoom(roomName))
			{
				Debug.LogError("Local player is not in this room");
				return false;
			}
			
			bool isProfanity  = await IsProfaneMessage(messageToSend);

			if (isProfanity)
			{
				// Disallow (or prompt Player to resubmit)
				messageToSend = "[Message Not Allowed]";
			}
            
			await GetRoomHandle(roomName).SendMessage(messageToSend);
			return true;
		}
		
		public List<Message> GetRoomMessages(string roomName)
		{
			if (!HasRoom(roomName))
			{
				Debug.LogError("Room does not exist");
			}
			
			if (!IsLocalPlayerInRoom(roomName))
			{
				Debug.LogError("Local player is not in this room");
			}
			
			return GetRoomHandle(roomName).Messages;;
		}
		
		/// <summary>
		/// Optional: Filter out words which are not appropriate for the
		/// audience.
		/// </summary>
		public async Task<bool> IsProfaneMessage(string text)
		{
			bool isProfanityText = true;
			try
			{
				var result = await _beamableAPI.Experimental.ChatService.ProfanityAssert(text);
				isProfanityText = false;
			} catch{}

			return isProfanityText;
		}

		#endregion

		#region InventoryService

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
        
		#endregion
        
        //  Event Handlers  -------------------------------
        private void InventoryService_OnChanged(InventoryView inventoryView)
        {
            _inventoryView = inventoryView;
            OnInventoryViewChanged.Invoke(inventoryView);
        }
        
        private void ChatService_OnChanged(ChatView chatView)
        {
	        _chatView = chatView;
	        OnChatViewChanged.Invoke(_chatView);
        }
	}
}
