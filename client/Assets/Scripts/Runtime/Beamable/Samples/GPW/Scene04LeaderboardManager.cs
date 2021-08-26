using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Leaderboard
   /// </summary>
   public class Scene04LeaderboardManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private Scene04LeaderboardUIView _scene04LeaderboardUIView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _scene04LeaderboardUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         //
         SetupBeamable();
      }
      
      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         // Every scene initializes as needed (Max 1 time per session)
         if (!GPWController.Instance.IsInitialized)
         {
            await GPWController.Instance.Initialize(_scene04LeaderboardUIView.Configuration);
         }
      }

      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         _scene04LeaderboardUIView.BackButton.interactable = false;
         
         StartCoroutine(GPWHelper.LoadScene_Coroutine(
            GPWController.Instance.RuntimeDataStorage.RuntimeData.PreviousSceneName,
            _scene04LeaderboardUIView.Configuration.DelayBeforeLoadScene));
      }
   }
}