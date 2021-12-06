using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Intro
   /// </summary>
   public class Scene04SettingsManager : MonoBehaviour
   {
      //  Fields ---------------------------------------

      [SerializeField]
      private Scene04SettingsUIView _scene04SettingsUIView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         // Clear UI
         _scene04SettingsUIView.TitleText = GPWHelper.SettingsTitle;
         _scene04SettingsUIView.BodyText = GPWHelper.GetSettingsBodyText();
         
         // Bottom Navigation
         _scene04SettingsUIView.ResetPlayerButton.onClick.AddListener(ResetPlayerButton_OnClicked);
         _scene04SettingsUIView.ResetGameDataButton.onClick.AddListener(ResetGameDataButton_OnClicked);
         _scene04SettingsUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         _scene04SettingsUIView.DialogSystem.ShowDialogBoxLoading(GPWHelper.Settings);
         
         SetupBeamable();

      }
      
      private async void SetupBeamable()
      {
         // Every scene initializes as needed (Max 1 time per session)
         if (!GPWController.Instance.IsInitialized)
         {
            await GPWController.Instance.Initialize(_scene04SettingsUIView.Configuration);
         }
         else
         {
            GPWController.Instance.PersistentDataStorage.ForceRefresh();
            GPWController.Instance.RuntimeDataStorage.ForceRefresh();
            GPWController.Instance.GameServices.ForceRefresh();
         }

         await _scene04SettingsUIView.DialogSystem.HideDialogBox();
      }

      //  Event Handlers -------------------------------
      private void ResetPlayerButton_OnClicked()
      {
         GPWHelper.PlayAudioClipSecondaryClick();
         
         _scene04SettingsUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               GPWController.Instance.ResetPlayerData();
            });
      }
      
      private void ResetGameDataButton_OnClicked()
      {
         GPWHelper.PlayAudioClipSecondaryClick();
         
         _scene04SettingsUIView.DialogSystem.ShowDialogBoxConfirmation(
            delegate
            {
               // Reset and return to intro scene
               GPWController.Instance.ResetGameDataViaDataFactory();
               BackButton_OnClicked();
            });
      }

      private void BackButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            _scene04SettingsUIView.Configuration.Scene01IntroName,
            _scene04SettingsUIView.Configuration.DelayBeforeLoadScene));
      }
   }
}