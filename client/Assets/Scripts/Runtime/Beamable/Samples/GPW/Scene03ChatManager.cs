using System;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Data;
using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Chat 
   /// </summary>
   public class Scene03ChatManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public Scene03ChatUIView Scene03ChatUIView { get { return _scene03ChatUIView; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Scene03ChatUIView _scene03ChatUIView = null;
      
      //  Unity Methods   ------------------------------
      protected async void Start()
      {
         
         // Clear UI
         _scene03ChatUIView.ScrollingText.SetText("");
         _scene03ChatUIView.ScrollingText.HyperlinkHandler.OnLinkClicked.AddListener(HyperlinkHandler_OnLinkClicked);
         
         // Top Navigation
         _scene03ChatUIView.GlobalChatToggle.onValueChanged.AddListener(GlobalChatButton_OnClicked);
         _scene03ChatUIView.LocationChatToggle.onValueChanged.AddListener(LocationChatButton_OnClicked);
         _scene03ChatUIView.DirectChatToggle.onValueChanged.AddListener(DirectChatButton_OnClicked);
         SetChatMode(ChatMode.Global);
         
         // Input
         _scene03ChatUIView.ChatInputUI.OnValueSubmitted.AddListener(ChatInputUI_OnValueSubmitted);
         _scene03ChatUIView.ChatInputUI.OnValueCleared.AddListener(ChatInputUI_OnValueCleared);

         // Bottom Navigation
         _scene03ChatUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         // Load
         _scene03ChatUIView.DialogSystem.DelayBeforeHideDialogBox =
            (int)_scene03ChatUIView.Configuration.DelayAfterDataLoading * 1000;
         
         await ShowDialogBoxLoadingSafe();
         SetupBeamable();
      }


      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         // Setup Storage
         GPWController.Instance.PersistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
         GPWController.Instance.RuntimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
         GPWController.Instance.GameServices.OnChatViewChanged.AddListener(GameServices_OnChatViewChanged);
         
         // Every scene initializes as needed (Max 1 time per session)
         if (!GPWController.Instance.IsInitialized)
         {
            await GPWController.Instance.Initialize(_scene03ChatUIView.Configuration);
         }
         else
         {
            GPWController.Instance.PersistentDataStorage.ForceRefresh();
            GPWController.Instance.RuntimeDataStorage.ForceRefresh();
         }
      }

      
      private async Task<EmptyResponse> ShowDialogBoxLoadingSafe()
      {
         // Get roomname, and fallback to blank
         string roomName = "";
         if (!_scene03ChatUIView.DialogSystem.HasCurrentDialogUI && GPWController.Instance.HasCurrentRoomHandle)
         {
            RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();
            roomName = roomHandle.Name;
         }

         if (_scene03ChatUIView.DialogSystem.HasCurrentDialogUI)
         {
            await _scene03ChatUIView.DialogSystem.HideDialogBoxImmediate();
         }
         _scene03ChatUIView.DialogSystem.ShowDialogBoxLoading(roomName);
         return new EmptyResponse();
      }
      
      
      private async void SetChatMode(ChatMode chatMode)
      {
         // Change mode
         GPWController.Instance.RuntimeDataStorage.RuntimeData.ChatMode = chatMode;

         // THis mode can be reached by clicking chat text too
         // so update the button to look selected
         if (chatMode == ChatMode.Direct)
         {
            _scene03ChatUIView.DirectChatToggle.isOn = true;
         }
         
         // Show mode specific prompt
         await ShowDialogBoxLoadingSafe();
         
         // Update
         GPWHelper.PlayAudioClipSecondaryClick();
         GPWController.Instance.RuntimeDataStorage.ForceRefresh();
      }
      
      
      private async void RenderChatOutput()
      {
         if (!GPWController.Instance.GameServices.HasChatView)
         {
            return;
         }
         
         if (!GPWController.Instance.HasCurrentRoomHandle)
         {
            return;
         }
         
         RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();

         ChatMode chatMode = GPWController.Instance.RuntimeDataStorage.RuntimeData.ChatMode;
         StringBuilder stringBuilder = new StringBuilder();
         stringBuilder.AppendLine("---- RenderChatOutput Room ----");
         stringBuilder.AppendLine($"Title: {roomHandle.Name}");
         stringBuilder.AppendLine($"Topic: {GPWHelper.GetChatRoomTopic (chatMode)}");
         stringBuilder.AppendLine($"Players: {roomHandle.Players.Count}");
         stringBuilder.AppendLine($"Messages: {roomHandle.Messages.Count}");
         stringBuilder.AppendLine().AppendLine();
         
         foreach (Message message in roomHandle.Messages)
         {
            long playerDbid = message.gamerTag;
            string alias = "";
            try
            {
               alias = await GPWController.Instance.GameServices.GetOrCreateAlias(playerDbid);
            }
            catch (Exception e)
            {
               Debug.Log("E: " + e.Message);
            }
            
            //Temporarily override alias to reduce confusion. Its by the local player but 
            //from a previous account. 
            if (!GPWController.Instance.GameServices.IsLocalPlayerDbid(playerDbid) &&
                alias == GPWHelper.DefaultLocalAlias)
            {
               alias = MockDataCreator.CreateNewRandomAlias(GPWHelper.DefaultRemoteAliasPrefix);
            }
                         
            if (GPWController.Instance.RuntimeDataStorage.RuntimeData.ChatMode == ChatMode.Direct ||
                GPWController.Instance.GameServices.IsLocalPlayerDbid(playerDbid))
            {
               stringBuilder.AppendLine($"[{alias}]: " + message.content);
            }
            else
            {
               // When NOT in direct chat, and NOT the local player, renders clickable text
               // Clicks are handled above by "HyperlinkHandler_OnLinkClicked"
               stringBuilder.AppendLine($"[{TMP_HyperlinkHandler.WrapTextWithLink(alias, playerDbid.ToString())}]: " + message.content);
            }
         }
         
         _scene03ChatUIView.ScrollingText.SetText(stringBuilder.ToString());
         
         await _scene03ChatUIView.DialogSystem.HideDialogBox();
         _scene03ChatUIView.ChatInputUI.Select();
      }
      
      
      //  Event Handlers -------------------------------
      private async void HyperlinkHandler_OnLinkClicked(string href)
      {
         if (GPWController.Instance.RuntimeDataStorage.RuntimeData.ChatMode == ChatMode.Direct)
         {
            throw new Exception("HyperlinkHandler_OnLinkClicked() ChatMode cannot be ChatMode.Direct. ");
         }

         SetChatMode(ChatMode.Direct);
         
         RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();
         long dbid1 = GPWController.Instance.GameServices.LocalPlayerDbid;
         long dbid2 = long.Parse(href);
         await GPWController.Instance.GameServices.JoinDirectRoomWithOnly2Players(dbid1, dbid2);
      }
      
      
      private async void ChatInputUI_OnValueSubmitted(string message)
      {
         RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();
         await GPWController.Instance.GameServices.SendMessage(roomHandle.Name, message);
         _scene03ChatUIView.ChatInputUI.Select();
      }


      private void ChatInputUI_OnValueCleared()
      {
         GPWHelper.PlayAudioClipSecondaryClick();
      }
      
         
      private void GlobalChatButton_OnClicked(bool isOn)
      {
         if (isOn)
         {
            SetChatMode(ChatMode.Global);
         }
      }
      
      
      private void LocationChatButton_OnClicked(bool isOn)
      {
         if (isOn)
         {
            SetChatMode(ChatMode.Location);
         }
      }
      
      
      private void DirectChatButton_OnClicked(bool isOn)
      {
         if (isOn)
         {
            SetChatMode(ChatMode.Direct);
         }
      }
      
      
      private void BackButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene03ChatUIView.Configuration.Scene02GameName,
            _scene03ChatUIView.Configuration.DelayBeforeLoadScene));
      }
      
      
      private void PersistentDataStorage_OnChanged(SubStorage subStorage)
      {
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         _scene03ChatUIView.PersistentData = persistentDataStorage.PersistentData;
         _scene03ChatUIView.LocationContentView = GPWController.Instance.LocationContentViewCurrent;
         RenderChatOutput();
      }
      
      
      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _scene03ChatUIView.RuntimeData = runtimeDataStorage.RuntimeData;
         RenderChatOutput();
      }
      
      
      private void GameServices_OnChatViewChanged(ChatView chatView)
      {
         RenderChatOutput();
      }
   }
}