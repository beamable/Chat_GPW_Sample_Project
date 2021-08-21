using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class GameUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public Button ChatButton { get { return _chatButton; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public Button TravelButton { get { return _travelButton; } }
      public Button BankButton { get { return _bankButton; } }
      public Button DebtButton { get { return _debtButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public TMP_Text RoundText { get { return _roundText; } }
      public CircularScrollingList CircularScrollingList { get { return _circularScrollingList; } }
      
      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      private TMP_Text _roundText = null;

      [Header("Scrolling List")]
      [SerializeField]
      private CircularScrollingList _circularScrollingList = null;
      
      [Header("Top Buttons")]
      [SerializeField]
      private Button _travelButton = null;

      [SerializeField]
      private Button _bankButton = null;

      [SerializeField]
      private Button _debtButton = null;
      
      [Header("Bottom Buttons")]
      [SerializeField]
      private Button _chatButton = null;

      [SerializeField]
      private Button _leaderboardButton = null;

      [SerializeField]
      private Button _quitButton = null;

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