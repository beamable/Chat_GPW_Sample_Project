using System.Collections.Generic;
using System.Text;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Chat
   /// </summary>
   public class ChatSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public ChatUIView ChatUIView { get { return _chatUIView; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Configuration _configuration = null;
      
      [SerializeField]
      private ChatUIView _chatUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _chatUIView.ScrollingText.SetText("");
         
         //
         _chatUIView.GlobalChatButton.onClick.AddListener(GlobalChatButton_OnClicked);
         _chatUIView.LocationChatButton.onClick.AddListener(LocationChatButton_OnClicked);
         _chatUIView.DirectChatButton.onClick.AddListener(DirectChatButton_OnClicked);
         //
         _chatUIView.TransactionButton.onClick.AddListener(TransactionButton_OnClicked);
         _chatUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         //
         SetupBeamable();
      }
      

      //  Other Methods  -----------------------------

      private async void SetupBeamable()
      {
         // Setup Storage
         GameController.Instance.PersistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
         GameController.Instance.RuntimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
         GameController.Instance.GameServices.OnChatViewChanged.AddListener(GameServices_OnChatViewChanged);
         
         // Every scene initializes as needed (Max 1 time per session)
         if (!GameController.Instance.IsInitialized)
         {
            await GameController.Instance.Initialize(_configuration);
         }
         else
         {
            GameController.Instance.PersistentDataStorage.ForceRefresh();
            GameController.Instance.RuntimeDataStorage.ForceRefresh();
         }
      }



      //  Event Handlers -------------------------------

      
      
      private void GlobalChatButton_OnClicked()
      {
         GameController.Instance.RuntimeDataStorage.RuntimeData.ChatMode = ChatMode.Global;
         GameController.Instance.RuntimeDataStorage.ForceRefresh();
      }
      
      private void LocationChatButton_OnClicked()
      {
         GameController.Instance.RuntimeDataStorage.RuntimeData.ChatMode = ChatMode.Location;
         GameController.Instance.RuntimeDataStorage.ForceRefresh();
      }
      
      private void DirectChatButton_OnClicked()
      {
         GameController.Instance.RuntimeDataStorage.RuntimeData.ChatMode = ChatMode.Direct;
         GameController.Instance.RuntimeDataStorage.ForceRefresh();
      }
      
      private void TransactionButton_OnClicked()
      {
        
      }
      
      private void BackButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void PersistentDataStorage_OnChanged(SubStorage subStorage)
      {
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         _chatUIView.PersistentData = persistentDataStorage.PersistentData;
         Chat();
      }
      
      
      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _chatUIView.RuntimeData = runtimeDataStorage.RuntimeData;
         Chat();
      }
      
      private void GameServices_OnChatViewChanged(ChatView chatView)
      {
         Chat();
      }

      private void Chat()
      {
         if (!GameController.Instance.GameServices.HasChatView)
         {
            return;
         }
         RoomHandle roomHandle = GameController.Instance.GetCurrentRoomHandle();
         

         Debug.Log("2 Rooms -----------------------------");
         foreach (var x in GameController.Instance.GameServices.ChatView.roomHandles)
         {
            Debug.Log("\nNAME: " + x.Name + " #" + x.Players.Count);
         }
         
         Debug.Log("3 Guild -----------------------------");
         foreach (var x in GameController.Instance.GameServices.ChatView.GuildRooms)
         {
            Debug.Log("\nNAME: " + x.Name + " #" + x.Players.Count);
         }
         
         Debug.Log("4 Direct -----------------------------");
         foreach (var x in GameController.Instance.GameServices.ChatView.DirectMessageRooms)
         {
            Debug.Log("\nNAME: " + x.Name + " #" + x.Players.Count);
         }
         
         Debug.Log("5 So -----------------------------");
         Debug.Log("you looking in room: " + roomHandle.Name);

         StringBuilder stringBuilder = new StringBuilder();
         for (int i = 0; i < 200; i++)
         {
            stringBuilder.AppendLine($"[Alias01]: " + i);
         }

         _chatUIView.ScrollingText.SetText(stringBuilder.ToString());
      }
      
   }
}