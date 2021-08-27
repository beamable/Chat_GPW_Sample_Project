using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI
{
   /// <summary>
   /// Wraps a <see cref="TMP_Text"/> for input text
   /// and submit, and clear.
   ///
   /// Automatically toggles the interactable for buttons
   /// </summary>
   public class TMP_ChatInputUI : MonoBehaviour
   {
      public InputField.SubmitEvent OnValueSubmitted = new InputField.SubmitEvent();
      public InputField.OnChangeEvent OnValueChanged = new InputField.OnChangeEvent();
      public UnityEvent OnValueCleared = new UnityEvent();

      //  Properties --------------------------------------
      public TMP_InputField InputField { get { return _inputField;}}
      public Button ChatInputSubmitButton { get { return _chatInputSubmitButton;}}
      public Button ChatInputClearButton { get { return _chatInputClearButton;}}

      //  Fields ---------------------------------------
      [SerializeField] 
      private TMP_InputField _inputField = null;
      
      [SerializeField] 
      private Button _chatInputSubmitButton = null;

      [SerializeField] 
      private Button _chatInputClearButton = null;

      [SerializeField] 
      private bool _willAutoClear = true;
      
      [SerializeField] 
      private bool _willAutoSelect = true;

      [SerializeField] 
      private bool _willSubmitViaKeyCode = true;

      [SerializeField] 
      private KeyCode _keyCode = KeyCode.Return;
      
      //  Unity Methods   ------------------------------
      protected void Awake()
      {
         _inputField.onValueChanged.AddListener(ChatInput_OnValueChanged);
         _chatInputSubmitButton.onClick.AddListener(ChatInputSubmitButton_OnClicked);
         _chatInputClearButton.onClick.AddListener(ChatInputClearButton_OnClicked);
         Refresh();
      }
      
      protected void Update()
      {
         if (_willSubmitViaKeyCode)
         {
            if (Input.GetKeyDown(_keyCode))
            {
               ChatInputSubmitButton_OnClicked();
               Refresh();
            }
         }
      }
      
      //  Other Methods   ------------------------------
      private void Refresh()
      {
         _chatInputSubmitButton.interactable = _inputField.text.Length > 0;
         _chatInputClearButton.interactable = _inputField.text.Length > 0;
         
         if (_willAutoSelect)
         {
            Select();
         }
      }

      public void Select()
      {
         _inputField.Select();
      }
      
      //  Event Handlers   ------------------------------
      private void ChatInput_OnValueChanged(string message)
      {
         OnValueChanged.Invoke(message);
         Refresh();
      }
      
      private async void ChatInputSubmitButton_OnClicked()
      {
         if (string.IsNullOrEmpty(_inputField.text))
         {
            return;
         }
         
         OnValueSubmitted.Invoke(_inputField.text);

         if (_willAutoClear)
         {
            ChatInputClearButton_OnClicked();
         }
         
         Refresh();
      }
      
      private void ChatInputClearButton_OnClicked()
      {
         OnValueCleared.Invoke();
         
         _inputField.text = "";
         ChatInput_OnValueChanged(_inputField.text);
      }
   }
}