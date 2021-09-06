using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Chat
   /// </summary>
   public class Scene03ChatUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public TMP_Text CashText { get { return _cashText; } }
      public TMP_Text ItemsText { get { return _itemsText; } }
      public TMP_Text TurnText { get { return _turnText; } }
      
      public TMP_ScrollingText ScrollingText { get { return _scrollingText; } }
      public TMP_ChatInputUI ChatInputUI { get { return _chatInputUI ;}}

      public ToggleGroup ToggleGroup { get { return _toggleGroup; } }
      public Toggle GlobalChatToggle { get { return _globalChatToggle; } }
      public Toggle LocationChatToggle { get { return _locationChatToggle; } }
      public Toggle DirectChatToggle { get { return _directChatToggle; } } 
      
      public Button BackButton { get { return _backButton; } }
      
      public PersistentData PersistentData { get { return _persistentData; } set { _persistentData = value; Refresh(); } }
      public RuntimeData RuntimeData { get { return _runtimeData; } set { _runtimeData = value; Refresh(); } }
      public LocationContentView LocationContentView { get { return _locationContentView; } set { _locationContentView = value; Refresh(); } }
      

      //  Fields ---------------------------------------
      [Header("Child Properties")]
      
      [SerializeField]
      private TMP_Text _cashText = null;

      [SerializeField]
      private TMP_Text _itemsText = null;

      [SerializeField]
      private TMP_Text _turnText = null;

      [SerializeField]
      private TMP_ScrollingText _scrollingText = null;
      
      [SerializeField]
      private TMP_ChatInputUI _chatInputUI = null;
      
      [SerializeField]
      private ToggleGroup _toggleGroup = null;

      [SerializeField]
      private Toggle _globalChatToggle = null;
      
      [SerializeField]
      private Toggle _locationChatToggle = null;
      
      [SerializeField]
      private Toggle _directChatToggle = null;
      
      [SerializeField]
      private Button _backButton = null;
      
      private PersistentData _persistentData = null;
      private RuntimeData _runtimeData = null;
      private LocationContentView _locationContentView;
      
      //  Unity Methods   ------------------------------
      protected override void Start()
      {
         base.Start();
         Refresh();
      }
      
      //  Other Methods   ------------------------------
      public void Refresh()
      {
         GPWHelper.SetChildTMPText(_globalChatToggle, "Global", "Chat");
         GPWHelper.SetChildTMPText(_directChatToggle, "Direct", $"Chat");
         GPWHelper.SetChildTMPText(_backButton, "Back");
         
         string title = "Location";
         if (_locationContentView != null)
         {
            title = _locationContentView.LocationContent.Title;
         }
         
         //Adjust spacing due to long names
         GPWHelper.SetChildTMPText(_locationChatToggle, 
            $"<size=20>{title}</size>",
            "Chat", 12);
         
         
         if (_persistentData != null)
         {
            _cashText.text = $"Cash: ${_persistentData.CashAmount}";
            _turnText.text = $"Turn: {_persistentData.TurnCurrent}/{_persistentData.TurnsTotal}";
         }

         if (_runtimeData != null)
         {
            _itemsText.text = $"Items: {_runtimeData.ItemsCurrent}/{_runtimeData.ItemsMax}";

            switch (_runtimeData.ChatMode)
            {
               case ChatMode.Global:
                  _globalChatToggle.Select();
                  break;
               case ChatMode.Location:
                  _locationChatToggle.Select();
                  break;
               case ChatMode.Direct:
                  _directChatToggle.Select();
                  break;
               default:
                  //allow default
                  break;
            }

         }
      }
   }
}