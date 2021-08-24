using System;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Intro
   /// </summary>
   public class IntroSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------

      [SerializeField]
      private IntroUIView _introUIView = null;

      [SerializeField]
      private Configuration _configuration = null;

      private IBeamableAPI _beamableAPI = null;
      private bool _isConnected = false;
      private bool _isBeamableSDKInstalled = true;
      private string _isBeamableSDKInstalledErrorMessage = "";

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _introUIView.TitleText = "Beamable\nGlobal Price Wars";
         _introUIView.BodyText = "";
         _introUIView.StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
         _introUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _introUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         
         //
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
         _beamableAPI = await Beamable.API.Instance;
         
         try
         {
            // Handle any changes to the internet connectivity
            _beamableAPI.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
            ConnectivityService_OnConnectivityChanged(_beamableAPI.ConnectivityService.HasConnectivity);
         
            // Every scene initializes as needed (Max 1 time per session)
            if (!GameController.Instance.IsInitialized)
            {
               await GameController.Instance.Initialize(_configuration);
            }
            
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
         
         _introUIView.BodyText = bodyText;
         _introUIView.ButtonsCanvasGroup.interactable = _isConnected;
      }

      

      //  Event Handlers -------------------------------
      private void ConnectivityService_OnConnectivityChanged(bool isConnected)
      {
         _isConnected = isConnected;
         RenderUI();
      }


      private void StartGameButton_OnClicked()
      {
         _introUIView.ButtonsCanvasGroup.interactable = false;

         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.LeaderboardSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void QuitButton_OnClicked()
      {
         _introUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               GPWHelper.QuitSafe();
            });
      }
   }
}