using System.Collections.Generic;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class IntroUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public string TitleText { set { _titleText.text = value; } }
      public string BodyText { set { _bodyText.text = value; } }
      public Button StartGameButton { get { return _startGameButton; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }
      
      public DialogSystem DialogSystem { get { return _dialogSystem; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private Button _startGameButton = null;

      [SerializeField]
      private Button _leaderboardButton = null;

      [SerializeField]
      private Button _quitButton = null;

      [SerializeField]
      private TMP_Text _titleText = null;
      
      [SerializeField]
      private TMP_Text _bodyText = null;

      [SerializeField]
      private CanvasGroup _buttonsCanvasGroup = null;

      [SerializeField]
      private DialogSystem _dialogSystem = null;
      
      [Header ("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }
   }
}