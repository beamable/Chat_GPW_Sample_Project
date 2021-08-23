using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
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
      
      public TMP_Text CashText { get { return _cashText; } }
      public TMP_Text ItemsText { get { return _itemsText; } }
      public TMP_Text TurnText { get { return _turnText; } }

      public Button TravelButton { get { return _travelButton; } }
      public Button BankButton { get { return _bankButton; } }
      public Button DebtButton { get { return _debtButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public TMP_Text RoundText { get { return _roundText; } }
      public CircularScrollingList CircularScrollingList { get { return _circularScrollingList; } }
      public PersistentData PersistentData { get { return _persistentData; } set { _persistentData = value; OnRefresh(); } }


      //  Fields ---------------------------------------
      private PersistentData _persistentData = null;
      
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      private TMP_Text _roundText = null;

      [Header("Scrolling List")]
      [SerializeField]
      private CircularScrollingList _circularScrollingList = null;
      
      [Header("Top Text")]
      [SerializeField]
      private TMP_Text _cashText = null;

      [SerializeField]
      private TMP_Text _itemsText = null;

      [SerializeField]
      private TMP_Text _turnText = null;

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
      
      private void OnRefresh()
      {
         _cashText.text = $"Cash: ${_persistentData.CashAmount}";
         _itemsText.text = $"Items: {_persistentData.ItemsCurrent}/{_persistentData.ItemsMax}";
         _turnText.text = $"Turn: {_persistentData.TurnCurrent}/{_persistentData.TurnsTotal}";
         _travelButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Travel\n${_persistentData.CashAmount}";
         _bankButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Bank\n${_persistentData.BankAmount}";
         _debtButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Debt\n${_persistentData.DebitAmount}";
      }
   }
}