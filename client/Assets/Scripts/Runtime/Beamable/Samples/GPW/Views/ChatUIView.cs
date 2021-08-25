using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.Core.UI;
using Beamable.Samples.GPW.Data.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Chat
   /// </summary>
   public class ChatUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public TMP_Text CashText { get { return _cashText; } }
      public TMP_Text ItemsText { get { return _itemsText; } }
      public TMP_Text TurnText { get { return _turnText; } }
      
      public TMP_ScrollingText ScrollingText { get { return _scrollingText; } }

      public Button GlobalChatButton { get { return _globalChatButton; } }
      public Button LocationChatButton { get { return _locationChatButton; } }
      public Button DirectChatButton { get { return _directChatButton; } }
      
      public Button TransactionButton { get { return _transactionButton; } }
      public Button BackButton { get { return _backButton; } }
      
      public PersistentData PersistentData { get { return _persistentData; } set { _persistentData = value; Refresh(); } }
      public RuntimeData RuntimeData { get { return _runtimeData; } set { _runtimeData = value; Refresh(); } }


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
      private Button _globalChatButton = null;
      
      [SerializeField]
      private Button _locationChatButton = null;
      
      [SerializeField]
      private Button _directChatButton = null;
      

      [SerializeField]
      private Button _transactionButton = null;
      
      [SerializeField]
      private Button _backButton = null;
      
      private PersistentData _persistentData = null;
      private RuntimeData _runtimeData = null;

      //  Unity Methods   ------------------------------
      protected override void Start()
      {
         base.Start();
      }
      
      //  Other Methods   ------------------------------
      public void Refresh()
      {
         GPWHelper.SetButtonText(_globalChatButton, "Global", "Chat");
         GPWHelper.SetButtonText(_directChatButton, "Direct", $"Chat");
         GPWHelper.SetButtonText(_transactionButton, "Direct", $"Buy/Sell");
         GPWHelper.SetButtonText(_backButton, "Back");
         
         if (_persistentData != null)
         {
            _cashText.text = $"Cash: ${_persistentData.CashAmount}";
            _turnText.text = $"Turn: {_persistentData.TurnCurrent}/{_persistentData.TurnsTotal}";


            string title = _persistentData?.LocationContentViewCurrent?.LocationContent?.Title;
            //Adjust spacing due to long names
            GPWHelper.SetButtonText(_locationChatButton, 
               $"<size=20>{title}</size>",
               "Chat", 12);
         }

         if (_runtimeData != null)
         {
            _itemsText.text = $"Items: {_runtimeData.ItemsCurrent}/{_runtimeData.ItemsMax}";

            switch (_runtimeData.ChatMode)
            {
               case ChatMode.Global:
                  _globalChatButton.Select();
                  break;
               case ChatMode.Location:
                  _locationChatButton.Select();
                  break;
               case ChatMode.Direct:
                  _directChatButton.Select();
                  break;
                  SwitchDefaultException.Throw(_runtimeData.ChatMode);
                  break;
            }

            _transactionButton.interactable = _runtimeData.ChatMode == ChatMode.Direct;
         }
      }
   }
}