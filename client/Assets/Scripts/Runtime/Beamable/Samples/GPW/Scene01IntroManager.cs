using System;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Intro
   /// </summary>
   public class Scene01IntroManager : MonoBehaviour
   {
      //  Fields ---------------------------------------

      [SerializeField]
      private Scene01IntroUIView _scene01IntroUIView = null;

      private BeamContext _beamContext;
      private bool _isConnected = false;
      private bool _isBeamableSDKInstalled = true;
      private string _isBeamableSDKInstalledErrorMessage = "";
      private InventoryView _inventoryView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         // Clear UI
         _scene01IntroUIView.TitleText = GPWHelper.GameTitle;
         _scene01IntroUIView.BodyText = "";
         
         // Bottom Navigation
         _scene01IntroUIView.StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
         _scene01IntroUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _scene01IntroUIView.SettingsButton.onClick.AddListener(SettingsButton_OnClicked);
         _scene01IntroUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;
         
         // Load
         _scene01IntroUIView.DialogSystem.ShowDialogBoxLoading(GPWHelper.Intro);
         SetupBeamable();
      }


      protected async void OnDestroy()
      {
         _beamContext.Api.ConnectivityService.OnConnectivityChanged -= ConnectivityService_OnConnectivityChanged;
         await _beamContext.ClearPlayerAndStop();
      }


      //  Other Methods --------------------------------

      /// <summary>
      /// Login with Beamable and fetch user/session information
      /// </summary>
      private async void SetupBeamable()
      {
         try
         {
            // Do this first! It sets the screenresolution
            // Every scene initializes as needed (Max 1 time per session)
            if (!GPWController.Instance.IsInitialized)
            {
               await GPWController.Instance.Initialize(_scene01IntroUIView.Configuration);
            }
         
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;

            // Observe inventory items count
            GPWController.Instance.GameServices.OnInventoryViewChanged.AddListener(InventoryService_OnChanged);
            GPWController.Instance.GameServices.ForceRefresh();
            
            // Handle any changes to the internet connectivity
            _beamContext.Api.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
            ConnectivityService_OnConnectivityChanged(_beamContext.Api.ConnectivityService.HasConnectivity);
         }
         catch (Exception e)
         {
            // Failed to connect (e.g. not logged in)
            _isBeamableSDKInstalled = false;
            _isBeamableSDKInstalledErrorMessage = e.Message;
            ConnectivityService_OnConnectivityChanged(false);
         }
      }


      /// <summary>
      /// Render the user-facing text with success or helpful errors.
      /// </summary>
      private void RenderUI()
      {
        
         long dbid = 0;
         if (_isConnected)
         {
            dbid = _beamContext.PlayerId;
         }

         string bodyText = GPWHelper.GetIntroAboutBodyText(
            _isConnected, 
            _isBeamableSDKInstalled, 
            dbid, _isBeamableSDKInstalledErrorMessage);
         
         _scene01IntroUIView.BodyText = bodyText;
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = _isConnected && _inventoryView != null;
      }

      
      private async void StartGame()
      {
         // Wait
         _scene01IntroUIView.DialogSystem.ShowDialogBoxLoading(GPWHelper.Game);
         
         // Load Scene
         await _scene01IntroUIView.DialogSystem.HideDialogBox();
        
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene01IntroUIView.Configuration.Scene02GameName,
            _scene01IntroUIView.Configuration.DelayBeforeLoadScene));
      }
      
      //  Event Handlers -------------------------------
      private async void ConnectivityService_OnConnectivityChanged(bool isConnected)
      {
         _isConnected = isConnected;
         
         RenderUI();
         await _scene01IntroUIView.DialogSystem.HideDialogBox();
      }

      
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         _inventoryView = inventoryView;
         RenderUI();
      }

      
      private void StartGameButton_OnClicked()
      {
         int itemsCount = 0;
         if (_inventoryView != null)
         {
            itemsCount = _inventoryView.items.Count;
         }
         
         if (itemsCount > 0)
         {
            _scene01IntroUIView.DialogSystem.ShowDialogBoxConfirmation(
               delegate
               {
                  GPWController.Instance.ResetGameDataViaDataFactory();
                  GPWController.Instance.ResetPlayerData();
                  _scene01IntroUIView.ButtonsCanvasGroup.interactable = true;
          
               });

            _scene01IntroUIView.DialogSystem.CurrentDialogUI.BodyText.text = 
               $"Player has {itemsCount} inventory {GPWHelper.GetPluralized("item", "items", itemsCount)}. " +
               "The game does not store 'PersistentData' between game sessions (yet). " +
               "Click 'Ok' to reset and try again.";
         }
         else
         {
            _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;
            StartGame();
         }
      }

      
      private void LeaderboardButton_OnClicked()
      {
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;
         
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene01IntroUIView.Configuration.Scene05LeaderboardName,
            _scene01IntroUIView.Configuration.DelayBeforeLoadScene));
      }
      
      
      private void SettingsButton_OnClicked()
      {
         GPWHelper.PlayAudioClipPrimaryClick();
                  
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene01IntroUIView.Configuration.Scene04SettingsName,
            _scene01IntroUIView.Configuration.DelayBeforeLoadScene));

      }
      
      
      private void QuitButton_OnClicked()
      {
         GPWHelper.PlayAudioClipPrimaryClick();
         
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;
         
         _scene01IntroUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               GPWHelper.PlayAudioClipSecondaryClick();
               GPWHelper.QuitSafe();
            });
      }
   }
}