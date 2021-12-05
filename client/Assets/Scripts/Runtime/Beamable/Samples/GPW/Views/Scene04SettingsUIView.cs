using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class Scene04SettingsUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public string TitleText { set { _titleText.text = value; } }
      public string BodyText { set { _bodyText.text = value; } }
      public Button ResetGameDataButton { get { return _resetGameDataButton; } }
      public Button ResetPlayerButton { get { return _resetPlayerButton; } }
      
      public Button BackButton { get { return _backButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

      //  Fields ---------------------------------------
      [Header("Child Properties")]
      
      [SerializeField]
      private Button _resetGameDataButton = null;

      [SerializeField]
      private Button _resetPlayerButton = null;

      [SerializeField]
      private Button _backButton = null;

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