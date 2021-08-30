using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW;
using Beamable.Samples.GPW.Data;
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
        
        public bool HasCurrentDialogUI { get { return _currentDialogUI != null; }}
        
        public int DelayBeforeHideDialogBox { get { return _delayBeforeHideDialogBox; } set { _delayBeforeHideDialogBox = value; }}
    
        
        //  Fields ---------------------------------------
        [SerializeField] 
        private GameObject _dialogParent = null;
        
        [SerializeField] 
        private CanvasGroup _dialogParentCanvasGroup = null;

        [SerializeField] 
        private DialogUI _dialogUIPrefab = null;

        [SerializeField] 
        private Configuration _configuration = null;

        
        private DialogUI _currentDialogUI = null;
        private int _delayBeforeHideDialogBox = 0;

        //  Constructor ---------------------------------------
        
        //  Other Methods ---------------------------------------
        public T ShowDialogBox<T>(T dialogUIPrefab, 
            string titleText,
            string bodyText,
            List<DialogButtonData> dialogButtonDatas) where  T : DialogUI
        {
            
            TweenHelper.CanvasGroupDoFade(_dialogParentCanvasGroup, 
                0, 1, _configuration.DelayFadeInUI, 0);

            _currentDialogUI = GameObject.Instantiate<T>(dialogUIPrefab, _dialogParent.transform);
            _currentDialogUI.DialogButtonDatas = dialogButtonDatas;
            _currentDialogUI.TitleText.text = titleText;
            _currentDialogUI.BodyText.text = bodyText;
            return _currentDialogUI as T;
        }

        public async Task<EmptyResponse> HideDialogBox()
        {
            if (_currentDialogUI != null)
            {
                // For cosmetics have a short delay
                await Task.Delay(_delayBeforeHideDialogBox);

                // For cosmetics have a short delay
                TweenHelper.CanvasGroupDoFade(_dialogParentCanvasGroup,
                    1, 0, _configuration.DelayFadeInUI, 0).onComplete = () =>
                {
                    GameObject.Destroy(_currentDialogUI.gameObject);
                    _currentDialogUI = null;
                };

            }
            return new EmptyResponse();
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
                        GPWHelper.PlayAudioClipSecondaryClick();
                        HideDialogBox();
                    })
                });
        }
        
        /// <summary>
        /// Show "Loading..."
        /// </summary>
        /// <param name="areaToLoadTitle"></param>
        /// <returns></returns>
        public DialogUI ShowDialogBoxLoading(string areaToLoadTitle)
        {
            DialogUI dialogUI = ShowDialogBox<DialogUI>(
                DialogUIPrefab,
                string.Format(_configuration.DialogBoxLoadingText, areaToLoadTitle),
                "",
                new List<DialogButtonData>());
            
            dialogUI.RectTransform.sizeDelta = new Vector2(dialogUI.RectTransform.sizeDelta.x, 150);
            
            return dialogUI;
        }
    }
}