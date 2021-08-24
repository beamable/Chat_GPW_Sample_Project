using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Data.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class GameUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public TMP_Text CashText { get { return _cashText; } }
      public TMP_Text ItemsText { get { return _itemsText; } }
      public TMP_Text TurnText { get { return _turnText; } }

      public Button TravelButton { get { return _travelButton; } }
      public Button BankButton { get { return _bankButton; } }
      public Button DebtButton { get { return _debtButton; } }
      
      public Button ChatButton { get { return _chatButton; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button QuitButton { get { return _quitButton; } }

      public CircularScrollingList ProductContentList { get { return _productContentList; } }
      public CanvasGroup ProductContentListCanvasGroup { get { return _productContentListCanvasGroup; } }
      public PersistentData PersistentData { get { return _persistentData; } set { _persistentData = value; OnRefresh(); } }
      public RuntimeData RuntimeData { get { return _runtimeData; } set { _runtimeData = value; OnRefresh(); } }


      //  Fields ---------------------------------------
      private PersistentData _persistentData = null;
      private RuntimeData _runtimeData = null;

      [Header("Child Properties")]
      [SerializeField]
      private CircularScrollingList _productContentList = null;
      
      [SerializeField]
      private CanvasGroup _productContentListCanvasGroup = null;
      
      [Header("Top Text")]
      [SerializeField]
      private TMP_Text _cashText = null;

      [SerializeField]
      private TMP_Text _itemsText = null;

      [SerializeField]
      private TMP_Text _turnText = null;

      [Header("Top Buttons")]

      [SerializeField]
      private Button _bankButton = null;

      [SerializeField]
      private Button _travelButton = null;

      [SerializeField]
      private Button _debtButton = null;
      
      [Header("Bottom Buttons")]
      [SerializeField]
      private Button _chatButton = null;

      [SerializeField]
      private Button _leaderboardButton = null;

      [SerializeField]
      private Button _quitButton = null;

      //  Unity Methods   ------------------------------
      protected override void Start()
      {
         base.Start(); 
      }
      
      private void OnRefresh()
      {
         if (_persistentData != null)
         {
            _cashText.text = $"Cash: ${_persistentData.CashAmount}";
            _turnText.text = $"Turn: {_persistentData.TurnCurrent}/{_persistentData.TurnsTotal}";
            GPWHelper.SetButtonText(_travelButton, "Travel", 
               _persistentData.LocationContentViewCurrent.LocationContent.Title);
            GPWHelper.SetButtonText(_bankButton, "Bank", $"${_persistentData.BankAmount}");
            GPWHelper.SetButtonText(_debtButton, "Debt", $"${_persistentData.DebitAmount}");
         }

         if (_runtimeData != null)
         {
            _itemsText.text = $"Items: {_runtimeData.ItemsCurrent}/{_runtimeData.ItemsMax}";
         }
      }
   }
}