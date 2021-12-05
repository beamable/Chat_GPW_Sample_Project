using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Leaderboard
   /// </summary>
   public class Scene05LeaderboardManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private Scene05LeaderboardUIView _scene05LeaderboardUIView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         // Bottom Navigation
         _scene05LeaderboardUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         // Load
         SetupBeamable();
      }
      
      
      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         // Every scene initializes as needed (Max 1 time per session)
         if (!GPWController.Instance.IsInitialized)
         {
            await GPWController.Instance.Initialize(_scene05LeaderboardUIView.Configuration);
         }
      }

      
      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         _scene05LeaderboardUIView.BackButton.interactable = false;
         
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            GPWController.Instance.RuntimeDataStorage.RuntimeData.PreviousSceneName,
            _scene05LeaderboardUIView.Configuration.DelayBeforeLoadScene));
      }
   }
}