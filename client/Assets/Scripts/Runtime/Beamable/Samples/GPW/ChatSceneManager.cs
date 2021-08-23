using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Views;
using UnityEngine;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Handles the main scene logic: Chat
   /// </summary>
   public class ChatSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public ChatUIView ChatUIView { get { return _chatUIView; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Configuration _configuration = null;
      
      [SerializeField]
      private ChatUIView _chatUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _chatUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         SetupBeamable();
      }
      

      //  Other Methods  -----------------------------
      private void DebugLog(string message)
      {
         if (GPWConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }

      private void SetupBeamable()
      {
         
      }

      /// <summary>
      /// Render UI text
      /// </summary>
      /// <param name="message"></param>
      /// <param name="statusTextMode"></param>
      public void SetStatusText(string message, TMP_BufferedText.BufferedTextMode statusTextMode)
      {
         _chatUIView.BufferedText.SetText(message, statusTextMode);
      }

      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}