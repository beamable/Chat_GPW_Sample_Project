using Beamable.Samples.GPW.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Leaderboard
   /// </summary>
   public class LeaderboardSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private Button _backButton = null;

      [SerializeField]
      private Configuration _configuration = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _backButton.onClick.AddListener(BackButton_OnClicked);
         
         //
         SetupBeamable();
      }
      
      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         // Every scene initializes as needed (Max 1 time per session)
         if (!GameController.Instance.IsInitialized)
         {
            await GameController.Instance.Initialize(_configuration);
         }
      }

      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         _backButton.interactable = false;

         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}