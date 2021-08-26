using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Leaderboard
   /// </summary>
   public class LeaderboardSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private LeaderboardUIView _leaderboardUIView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _leaderboardUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         //
         SetupBeamable();
      }
      
      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         // Every scene initializes as needed (Max 1 time per session)
         if (!GameController.Instance.IsInitialized)
         {
            await GameController.Instance.Initialize(_leaderboardUIView.Configuration);
         }
      }

      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         _leaderboardUIView.BackButton.interactable = false;
         
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            GameController.Instance.RuntimeDataStorage.RuntimeData.PreviousSceneName,
            _leaderboardUIView.Configuration.DelayBeforeLoadScene));
      }
   }
}