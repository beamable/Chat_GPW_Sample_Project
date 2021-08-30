using System;
using Beamable.Samples.Core.Utilities;
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

      private IBeamableAPI _beamableAPI = null;
      private bool _isConnected = false;
      private bool _isBeamableSDKInstalled = true;
      private string _isBeamableSDKInstalledErrorMessage = "";

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         // Clear UI
         _scene01IntroUIView.TitleText = "Beamable\nGlobal Price Wars";
         _scene01IntroUIView.BodyText = "";
         
         // Bottom Navigation
         _scene01IntroUIView.StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
         _scene01IntroUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _scene01IntroUIView.ResetButton.onClick.AddListener(ResetButton_OnClicked);
         _scene01IntroUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         
         // Load
         SetupBeamable();
      }


      protected void OnDestroy()
      {
         Beamable.API.Instance.Then(de =>
         {
            _beamableAPI = null;
            de.ConnectivityService.OnConnectivityChanged -= ConnectivityService_OnConnectivityChanged;
         });
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
         
            _beamableAPI = await Beamable.API.Instance;
            
            // Handle any changes to the internet connectivity
            _beamableAPI.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
            ConnectivityService_OnConnectivityChanged(_beamableAPI.ConnectivityService.HasConnectivity);
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
            dbid = _beamableAPI.User.id;
         }

         string bodyText = GPWHelper.GetIntroAboutBodyText(
            _isConnected, 
            _isBeamableSDKInstalled, 
            dbid, _isBeamableSDKInstalledErrorMessage);
         
         _scene01IntroUIView.BodyText = bodyText;
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = _isConnected;
      }

      

      //  Event Handlers -------------------------------
      private void ConnectivityService_OnConnectivityChanged(bool isConnected)
      {
         _isConnected = isConnected;
         RenderUI();
      }


      private async void StartGameButton_OnClicked()
      {
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;

         // Wait
         _scene01IntroUIView.DialogSystem.ShowDialogBoxLoading(GPWConstants.Intro);
         await GPWController.Instance.ResetGame();
         
         // Load Scene
         await _scene01IntroUIView.DialogSystem.HideDialogBox();
        
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene01IntroUIView.Configuration.Scene02GameName,
            _scene01IntroUIView.Configuration.DelayBeforeLoadScene));
      }


      private void LeaderboardButton_OnClicked()
      {
         _scene01IntroUIView.ButtonsCanvasGroup.interactable = false;
         
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene01IntroUIView.Configuration.Scene04LeaderboardName,
            _scene01IntroUIView.Configuration.DelayBeforeLoadScene));
      }
      
      private void ResetButton_OnClicked()
      {
         GPWHelper.PlayAudioClipPrimaryClick();
         
         _scene01IntroUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               GPWHelper.PlayAudioClipSecondaryClick();
               ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
            });
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