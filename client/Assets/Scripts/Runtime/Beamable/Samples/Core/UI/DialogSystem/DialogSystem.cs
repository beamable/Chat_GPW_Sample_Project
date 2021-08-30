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
        private static string ConfirmationText = "Confirmation";
        private static string AreYouSureText = "Are you sure?";
        private static string OkText = "Ok";
        private static string CancelText = "Cancel";

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
            return await HideDialogBoxInternal(_delayBeforeHideDialogBox);
        }
        public async Task<EmptyResponse> HideDialogBoxImmediate()
        {
            return await HideDialogBoxInternal(0);
        }
        private async Task<EmptyResponse> HideDialogBoxInternal(int durationFadeIn)
        {
            if (_currentDialogUI != null)
            {
                // For cosmetics have a short durationFadeIn
                await Task.Delay(durationFadeIn);

                // For cosmetics have a short durationFadeIn
                TweenHelper.CanvasGroupDoFade(_dialogParentCanvasGroup,
                    1, 0, _configuration.DelayFadeInUI , 0).onComplete = () =>
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
                ConfirmationText,
                AreYouSureText,
                new List<DialogButtonData>
                {
                    new DialogButtonData(OkText, async delegate
                    {
                        action.Invoke();
                        await HideDialogBoxImmediate();
                    }),
                    new DialogButtonData(CancelText, async delegate
                    {
                        GPWHelper.PlayAudioClipSecondaryClick();
                        await HideDialogBoxImmediate();
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