using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Api.Inventory;
using Beamable.Api.Leaderboard;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Leaderboards;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Data;
using Beamable.Samples.Core.Debugging;
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
		public long LocalPlayerDbid { get { return _localPlayerDbid; } }
		public bool IsLocalPlayerDbid(long playerDbid) { return playerDbid == _localPlayerDbid; }
		
		public bool HasChatView { get { return ChatView != null; } }

		public ChatView ChatView { get { return _chatView; } }

		//  Fields  --------------------------------------
		private const string Price = "price";
		private const string ContentType = "items.product";
		//
		private ChatService _chatService = null;
		private ChatView _chatView = null;
		private IBeamableAPI _beamableAPI = null;
		private InventoryService _inventoryService = null;
		private LeaderboardService _leaderboardService = null;
		private LeaderboardContent _leaderboardContent = null;
		private InventoryView _inventoryView = null;
		private Dictionary<long, string> _aliasCacheDictionary = new Dictionary<long, string>();
		private bool _isInitialized = false;
		
		private long _localPlayerDbid;

		//  Unity Methods  --------------------------------

		//  Other Methods  --------------------------------
		public async Task Initialize(Configuration configuration)
		{
			if (!_isInitialized)
			{
				_beamableAPI = await Beamable.API.Instance;
				_localPlayerDbid = _beamableAPI.User.id;

				/////////////////////////////
				// ChatService
				/////////////////////////////
				_chatService = _beamableAPI.Experimental.ChatService;
				_chatService.Subscribe(ChatService_OnChanged);

				/////////////////////////////
				// InventoryService
				/////////////////////////////
				_inventoryService = _beamableAPI.InventoryService;
				_inventoryService.Subscribe(ContentType, InventoryService_OnChanged);

				/////////////////////////////
				// Leaderboard
				/////////////////////////////
				_leaderboardService = _beamableAPI.LeaderboardService;

				_leaderboardContent = await configuration.LeaderboardRef.Resolve();
				await PopulateLeaderboardWithMockData(configuration);


				_isInitialized = true;
			}
		}


		


		#region ChatService

		public bool HasRoom(string roomName)
		{
			return GetRoom(roomName) != null;
		}


		public RoomHandle GetRoom(string roomName)
		{
			if (_chatView != null)
			{
				foreach (RoomHandle roomHandle in _chatView.roomHandles)
				{
					if (roomHandle.Name == roomName)
					{
						return roomHandle;
					}
				}
			}
			return null;
		}

		private bool IsLocalPlayerInRoom(string roomName)
		{
			return IsPlayerInRoom(_localPlayerDbid, roomName);
		}

		private bool IsPlayerInRoom(long dbid, string roomName)
		{
			if (!HasRoom(roomName))
			{
				return false;
			}
			return GetRoom(roomName).Players.Contains(dbid);
		}
		
		public async Task<bool> CreateRoomSafe(string roomName)
		{
			bool wasJustCreated = false;

			if (_chatView == null)
			{
				throw new Exception("CreateRoomSafe() failed. _chatView must be not null.");
			}

			bool wasAlreadyExisting = HasRoom(roomName);
			if (!wasAlreadyExisting)
			{
				const bool keepSubscribed = true;
				List<long> players = new List<long> { _localPlayerDbid };

				RoomInfo roomInfo = await _chatService.CreateRoom(roomName,
					keepSubscribed,
					players
				);

				wasJustCreated = roomInfo.name == roomName;
			}

			bool isSuccess = wasAlreadyExisting || wasJustCreated;

			Configuration.Debugger.Log($"CreateRoomSafe() room={roomName}, " +
			                           $"wasAlreadyExisting={wasAlreadyExisting}, " +
			                           $"wasJustCreated={wasJustCreated}");
	
			return isSuccess;
		}

		public async Task<bool> JoinDirectRoomWithOnly2Players(long dbid1, long dbid2)
		{
			List<long> players = new List<long>();
			players.Add(dbid1);
			players.Add(dbid2);
			return await JoinRoom(players, GPWController.Instance.GetRoomHandleForChatMode(ChatMode.Direct).Name, true);
		}
		public async Task<bool> JoinRoom(List<long> newPlayers, string roomName, bool willRemoveOtherPlayers)
		{
			if (!HasRoom(roomName))
			{
				Debug.LogError("Room does not exist");
				return false;
			}

			//TODO: how do I join an existing room? Like this? -srivello
			RoomHandle roomHandle = GetRoom(roomName);
			List<long> players = roomHandle.Players;
			
			//Remove some?
			if (willRemoveOtherPlayers)
			{
				players.Clear();
			}
			
			// Add some
			foreach (long newPlayer in newPlayers)
			{
				players.Add(newPlayer);
			}
			
			// Automatically will Create & Join (new room) or Join (existing room)
			await _chatService.CreateRoom(roomHandle.Id, true, players);

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

			await _chatService.LeaveRoom(GetRoom(roomName).Id);

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

			bool isProfanity = await IsProfaneMessage(messageToSend);

			if (isProfanity)
			{
				// Disallow (or prompt Player to resubmit)
				messageToSend = "[Message Not Allowed]";
			}

			await GetRoom(roomName).SendMessage(messageToSend);
			return true;
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
			}
			catch
			{
			}

			return isProfanityText;
		}

		#endregion

		#region InventoryService

		/// <summary>
		/// Called manually when a scene loads and all data is already fresh
		/// and we want to manually re-invoke the events
		/// </summary>
		public void ForceRefresh()
		{
			if (_inventoryView == null)
			{
				throw new Exception("ForceRefresh() failed. _inventoryView cannot be null");
			}

			InventoryService_OnChanged(_inventoryView);
		}
		
		public async Task<bool> BuyItemInternal(ProductContentView productContentView, int amount)
		{
			string contentId = productContentView.ProductContent.Id;
			int price = productContentView.MarketGoods.Price;
			InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
			for (int i = 0; i < amount; i++)
			{
				Dictionary<string, string> properties = new Dictionary<string, string>();
				properties = SetPriceInPropertiesDictionary(properties, price);
				inventoryUpdateBuilder.AddItem(contentId, properties);
			}

			await _inventoryService.Update(inventoryUpdateBuilder);
			
			//Update Mkt
			productContentView.MarketGoods.Quantity -= amount;

			return true;
		}

		public async Task<bool> SellItemInternal(ProductContentView productContentView, int amount)
		{
			string contentId = productContentView.ProductContent.Id;
			List<ItemView> itemViews = await GetOwnedItemViews(contentId);

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
				inventoryUpdateBuilder.DeleteItem(contentId, itemView.id);
			}

			await _inventoryService.Update(inventoryUpdateBuilder);
			
			//Update Mkt
			productContentView.MarketGoods.Quantity += amount;
			
			return true;
		}

		public async Task<int> GetOwnedItemQuantity(string contentId)
		{
			foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
			{
				if (kvp.Key == contentId)
				{
					return kvp.Value.Count;
				}
			}
			return 0;
		}
		
		public async Task<int> GetOwnedItemAveragePrice(string contentId)
		{
			foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
			{
				if (kvp.Key == contentId)
				{
					int quantity = kvp.Value.Count;
					int priceTotal = 0;
					foreach (ItemView itemView in kvp.Value)
					{
						int price = GetPriceInPropertiesDictionary(itemView.properties);
						priceTotal += price;
					}

					return priceTotal / quantity;
				}
			}

			return 0;
		}

		private static int GetPriceInPropertiesDictionary(Dictionary<string, string> propertiesDictionary)
		{
			return int.Parse(MockDataCreator.GetValueInPropertiesDictionary(propertiesDictionary, Price));
		}
		
		private static Dictionary<string, string> SetPriceInPropertiesDictionary(Dictionary<string, string> propertiesDictionary, int value)
		{
			return MockDataCreator.SetValueInPropertiesDictionary(propertiesDictionary, Price, value);
		}
		
		private async Task<List<ItemView>> GetOwnedItemViews(string contentId)
		{
			foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
			{
				string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
				return kvp.Value;
			}

			return new List<ItemView>();
		}

		#endregion

		#region Leaderboards

		/// <summary>
		/// For cosmetics, set some mock scores for display
		/// </summary>
		/// <param name="configuration"></param>
		private async Task PopulateLeaderboardWithMockData(Configuration configuration)
		{
			string loggingResult = await MockDataCreator.PopulateLeaderboardWithMockData(_beamableAPI,
				_leaderboardContent,
				configuration.LeaderboardRowCountMin,
				configuration.LeaderboardScoreMin,
				configuration.LeaderboardScoreMax);
			
			Configuration.Debugger.Log(loggingResult, DebugLogLevel.Simple);
		}

		public async Task<string> GetOrCreateAlias(long dbid)
		{
			// Try in cache
			string alias = "";
			_aliasCacheDictionary.TryGetValue(dbid, out alias);

			// Try in stats
			if (string.IsNullOrEmpty(alias))
			{
				alias = await MockDataCreator.GetCurrentUserAlias(
					_beamableAPI.StatsService, dbid);
			}
				
			// Missing? Create new, and write to stats
			if (string.IsNullOrEmpty(alias))
			{
				if (dbid == _localPlayerDbid)
				{
					// Only WRITE the local player
					alias = GPWHelper.DefaultLocalAlias;
					await MockDataCreator.SetCurrentUserAlias(_beamableAPI.StatsService, alias);
				}
				else
				{
					alias = MockDataCreator.CreateNewRandomAlias(GPWHelper.DefaultRemoteAliasPrefix);
				}
			}
			
			// Store in cache
			if (_aliasCacheDictionary.ContainsKey(dbid))
			{
				_aliasCacheDictionary[dbid] = alias;
			}
			else
			{
				_aliasCacheDictionary.Add(dbid, alias);
			}

			return alias;
		}

		/// <summary>
		/// Send the users real score at the end of play
		/// </summary>
		/// <param name="score"></param>
		/// <returns></returns>
		public async Task<EmptyResponse> GetOrCreateAliasAndSetLeaderboardScore(double score)
		{
			string alias = await GetOrCreateAlias(_localPlayerDbid);

			// Write the score
			await _leaderboardService.SetScore(_leaderboardContent.Id, score);

			return new EmptyResponse();
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

			foreach (RoomHandle roomHandle in _chatView.roomHandles)
			{
				if (IsLocalPlayerInRoom(roomHandle.Name))
				{
					roomHandle.OnMessageReceived -= RoomHandle_MessageReceived;
					roomHandle.OnMessageReceived += RoomHandle_MessageReceived;
				}
			}

			OnChatViewChanged.Invoke(_chatView);
		}

		private void RoomHandle_MessageReceived(Message message)
		{
			OnChatViewChanged.Invoke(_chatView);
		}


	}

}
