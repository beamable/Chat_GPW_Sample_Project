using System.Collections.Generic;
using Beamable.Samples.Core.Animation;
using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Chat
   /// </summary>
   public class ChatUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public Button BackButton { get { return _backButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public TMP_Text RoundText { get { return _roundText; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      private TMP_Text _roundText = null;

      [SerializeField]
      private Button _backButton = null;

      [Header("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }
   }
}