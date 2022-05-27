using System.ComponentModel;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
using Beamable.Samples.GPW.Data.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class Scene02GameUIView : BaseSceneUIView
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

      public ScrollingList ProductContentList { get { return _productContentList; } }

      public PersistentData PersistentData { get { return _persistentData; } set { _persistentData = value; Refresh(); } }
      public RuntimeData RuntimeData { get { return _runtimeData; } set { _runtimeData = value; Refresh(); } }
      public LocationContentView LocationContentView { get { return _locationContentView; } set { _locationContentView = value; Refresh(); } }


      //  Fields ---------------------------------------
      private PersistentData _persistentData = null;
      private RuntimeData _runtimeData = null;
      private LocationContentView _locationContentView;

      [Header("Child Properties")]
      [SerializeField]
      private ScrollingList _productContentList = null;
      

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
         Refresh();
      }
      
      public void Refresh()
      {
         _bankButton.interactable = _persistentData != null && !_persistentData.IsGameOver;
         _travelButton.interactable = _persistentData != null && !_persistentData.IsGameOver;
         _debtButton.interactable = _persistentData != null && !_persistentData.IsGameOver;

         if (_locationContentView != null)
         {
            GPWHelper.SetChildTMPText(_travelButton, "Travel", _locationContentView.LocationData.Title);
         }
         else
         {
            GPWHelper.SetChildTMPText(_travelButton, "Travel");
         }
         
         if (_persistentData != null)
         {
            _cashText.text = $"Cash: ${_persistentData.CashAmount}";
            _turnText.text = $"Turn: {_persistentData.TurnCurrent}/{_persistentData.TurnsTotal}";
           
            GPWHelper.SetChildTMPText(_bankButton, "Bank", $"${_persistentData.BankAmount}");
            GPWHelper.SetChildTMPText(_debtButton, "Debt", $"${_persistentData.DebtAmount}");
         }

         if (_runtimeData != null)
         {
            _itemsText.text = $"Items: {_runtimeData.ItemsCurrent}/{_runtimeData.ItemsMax}";
         }
      }
   }
}