using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class IntroUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public string TitleText { set { _titleText.text = value; } }
      public string BodyText { set { _bodyText.text = value; } }
      public Button StartGameButton { get { return _startGameButton; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

      //  Fields ---------------------------------------
      [Header("Child Properties")]
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

      //  Unity Methods   ------------------------------
      protected override void Start()
      {
         base.Start();
      }
   }
}