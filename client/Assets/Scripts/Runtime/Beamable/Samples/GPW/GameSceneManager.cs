using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{

   /// <summary>
   /// Handles the main scene logic: Game
   /// </summary>
   public class GameSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public GameUIView GameUIView { get { return _gameUIView; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Configuration _configuration = null;
      
      [SerializeField]
      private GameUIView _gameUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
         _gameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _gameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         SetupBeamable();
      }
      

      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         _beamableAPI = await Beamable.API.Instance;
         
         if (!RuntimeDataStorage.Instance.IsInitialized)
         {
            await RuntimeDataStorage.Instance.Initialize(_configuration);
         }
         else
         {
         }

         Debug.Log("Products.Count: " + 
                   RuntimeDataStorage.Instance.GameService.RemoteConfiguration.Products.Count);
         
         Debug.Log("Locations.Count: " + 
                   RuntimeDataStorage.Instance.GameService.RemoteConfiguration.Locations.Count);
         
         Debug.Log("LocationCurrent: " + 
                   RuntimeDataStorage.Instance.GameService.LocationCurrent.Title);
      }

      /// <summary>
      /// Render UI text
      /// </summary>
      /// <param name="message"></param>
      /// <param name="statusTextMode"></param>
      public void SetStatusText(string message, TMP_BufferedText.BufferedTextMode statusTextMode)
      {
         _gameUIView.BufferedText.SetText(message, statusTextMode);
      }

      //  Event Handlers -------------------------------
      private void ChatButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.ChatSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.LeaderboardSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void QuitButton_OnClicked()
      {
         Debug.Log("as are you sure? This blanks progress");
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}