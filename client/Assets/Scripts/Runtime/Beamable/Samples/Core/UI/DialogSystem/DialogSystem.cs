using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Manages the dialogs
    ///
    /// NOTE: This allows for only 0...1 dialog at a time. 
    /// </summary>
    [Serializable]
    public class DialogSystem
    {
        //  Properties ---------------------------------------
        public DialogUI DialogUIPrefab { get { return _dialogUIPrefab; }}
        
        //  Fields ---------------------------------------
        [SerializeField] 
        private GameObject _dialogParent = null;
        
        [SerializeField] 
        private DialogUI _dialogUIPrefab = null;
       
        private DialogUI _currentDialogUI = null;

        //  Constructor ---------------------------------------
        
        //  Other Methods ---------------------------------------
        public T ShowDialogBox<T>(T dialogUIPrefab, 
            string titleText,
            string bodyText,
            List<DialogButtonData> dialogButtonDatas) where  T : DialogUI
        {
            _currentDialogUI = GameObject.Instantiate<T>(dialogUIPrefab, _dialogParent.transform);
            _currentDialogUI.DialogButtonDatas = dialogButtonDatas;
            _currentDialogUI.TitleText.text = titleText;
            _currentDialogUI.BodyText.text = bodyText;
            return _currentDialogUI as T;
        }

        public void HideDialogBox()
        {
            if (_currentDialogUI == null)
            {
                return;
            }
            
            GameObject.Destroy(_currentDialogUI.gameObject);
            _currentDialogUI = null;
        }

        /// <summary>
        /// Show "Are you sure?"
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DialogUI ShowDialogBoxConfirmation(Action action)
        {
            return ShowDialogBox<DialogUI>(
                DialogUIPrefab,
                "Confirmation",
                "Are you sure?",
                new List<DialogButtonData>
                {
                    new DialogButtonData("Ok", delegate
                    {
                        action.Invoke();
                        HideDialogBox();
                    }),
                    new DialogButtonData("Cancel", delegate
                    {
                        HideDialogBox();
                    })
                });
        }
        
        /// <summary>
        /// Show "Loading..."
        /// </summary>
        /// <param name="bodyText"></param>
        /// <returns></returns>
        public DialogUI ShowDialogBoxLoading(string bodyText = "")
        {
            DialogUI dialogUI = ShowDialogBox<DialogUI>(
                DialogUIPrefab,
                "Loading...",
                bodyText,
                new List<DialogButtonData>());
            
            dialogUI.RectTransform.sizeDelta = new Vector2(dialogUI.RectTransform.sizeDelta.x, 220);
            
            return dialogUI;
        }
    }
}