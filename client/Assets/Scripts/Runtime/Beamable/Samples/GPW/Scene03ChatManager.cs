﻿using System;
using System.Text;
using Beamable.Experimental.Api.Chat;
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
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Scene03ChatUIView _scene03ChatUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         
         // Clear UI
         
         _scene03ChatUIView.ScrollingText.SetText("");
         
         // Top Navigation
         _scene03ChatUIView.GlobalChatButton.onClick.AddListener(GlobalChatButton_OnClicked);
         _scene03ChatUIView.LocationChatButton.onClick.AddListener(LocationChatButton_OnClicked);
         _scene03ChatUIView.DirectChatButton.onClick.AddListener(DirectChatButton_OnClicked);
         
         // Input
         _scene03ChatUIView.ChatInputUI.OnValueSubmitted.AddListener(ChatInputUI_OnValueSubmitted);
         _scene03ChatUIView.ChatInputUI.OnValueCleared.AddListener(ChatInputUI_OnValueCleared);

         // Bottom Navigation
         _scene03ChatUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         // Load
         _scene03ChatUIView.DialogSystem.DelayBeforeHideDialogBox =
            (int)_scene03ChatUIView.Configuration.DelayAfterDataLoading * 1000;
         ShowDialogBoxLoadingSafe();
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

      private void ShowDialogBoxLoadingSafe()
      {
         if (!_scene03ChatUIView.DialogSystem.HasCurrentDialogUI)
         {
            RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();
            _scene03ChatUIView.DialogSystem.ShowDialogBoxLoading(roomHandle.Name);
         }
      }
      
      private void SetChatMode(ChatMode chatMode)
      {
         // Change mode
         GPWController.Instance.RuntimeDataStorage.RuntimeData.ChatMode = chatMode;
         
         // Show mode specific prompt
         ShowDialogBoxLoadingSafe();
         
         // Update
         GPWHelper.PlayAudioClipSecondaryClick();
         GPWController.Instance.RuntimeDataStorage.ForceRefresh();
      }

      //  Event Handlers -------------------------------
      
      private async void ChatInputUI_OnValueSubmitted(string message)
      {
         RoomHandle roomHandle = GPWController.Instance.GetCurrentRoomHandle();
         bool isSuccess = await GPWController.Instance.GameServices.SendMessage(roomHandle.Name, message);
         
         //The ChatInputUI selects itself upon submit,
         //but calling again here prevents a selection bug, so
         //keep this here
         _scene03ChatUIView.ChatInputUI.Select();
      }
      
      private async void ChatInputUI_OnValueCleared()
      {
         GPWHelper.PlayAudioClipSecondaryClick();
      }
      
         
      private void GlobalChatButton_OnClicked()
      {
         SetChatMode(ChatMode.Global);

      }
      
      private void LocationChatButton_OnClicked()
      {
         SetChatMode(ChatMode.Location);
;
      }
      
      private void DirectChatButton_OnClicked()
      {
         SetChatMode(ChatMode.Direct);

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
         Chat();
      }
      
      
      private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
      {
         RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
         _scene03ChatUIView.RuntimeData = runtimeDataStorage.RuntimeData;
         Chat();
      }
      
      
      private void GameServices_OnChatViewChanged(ChatView chatView)
      {
         Chat();
      }

      
      private async void Chat()
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

         StringBuilder stringBuilder = new StringBuilder();
         stringBuilder.AppendLine($"Room: {roomHandle.Name}     |     Messages: ({roomHandle.Messages.Count})").AppendLine();
         
         foreach (Message message in roomHandle.Messages)
         {
            try
            {
               long playerDbid = message.gamerTag;
               string alias = await GPWController.Instance.GameServices.GetOrCreateAlias(playerDbid);
               
               if (GPWController.Instance.GameServices.IsLocalPlayerDbid(playerDbid))
               {
                  stringBuilder.AppendLine($"[{alias}]: " + message.content);
               }
               else
               {
                  stringBuilder.AppendLine($"[CLICKABLE {alias}]: " + message.content);
               }
               
            }
            catch (Exception e)
            {
               Debug.Log("E: " + e.Message);
            }
          
         }
         
         _scene03ChatUIView.ScrollingText.SetText(stringBuilder.ToString());
         
         await _scene03ChatUIView.DialogSystem.HideDialogBox();
      }
      
   }
}