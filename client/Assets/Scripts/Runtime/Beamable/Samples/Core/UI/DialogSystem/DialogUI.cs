using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Renders one dialog
    /// </summary>
    public class DialogUI : MonoBehaviour
    {
        //  Properties -----------------------------------
        public List<DialogButtonData> DialogButtonDatas { get { return _dialogButtonDatas; } set { _dialogButtonDatas = value; Render(); }}
        public bool IsInteractable { get { return _canvasGroup.interactable; } set { _canvasGroup.interactable = value; }}
        
        public TMP_Text TitleText { get { return _titleText; } }
        public TMP_Text BodyText { get { return _bodyText; } }
        public DialogButtonUI DialogButtonUIPrefab { get { return _dialogButtonUIPrefab; }}
        
        //  Fields ---------------------------------------
        private List<DialogButtonData> _dialogButtonDatas = null;
        
        [Header("Base Fields")]
        [SerializeField]
        private DialogButtonUI _dialogButtonUIPrefab;
                
        [SerializeField]
        private Image _backgroundImage = null;

        [SerializeField]
        private TMP_Text _titleText = null;

        [SerializeField]
        private TMP_Text _bodyText = null;

        [SerializeField]
        private HorizontalLayoutGroup _buttonsHorizontalLayoutGroup = null;

        [SerializeField]
        private RectTransform _resizableRectTransform = null;
        
        [SerializeField]
        private CanvasGroup _canvasGroup = null;

        [Header("Cosmetics")]
        [SerializeField]
        private Color _backgroundColor = new Color(0, 0, 0, 20);
        

        //  Unity Methods --------------------------------
        private void OnValidate()
        {
            if (_backgroundImage == null)
            {
                return;
            }

            _backgroundImage.color = _backgroundColor;
        }

        //  Other Methods --------------------------------
        public void SetHeight(float h)
        {
            _resizableRectTransform.sizeDelta = new Vector2(_resizableRectTransform.sizeDelta.x, h);
        }
        
        private void Render()
        {
            if ((_dialogButtonDatas == null || _dialogButtonDatas.Count == 0) &&
                _dialogButtonUIPrefab == null)
            {
                Debug.LogError($"Render() failed. Arguments invalid.");
                return;
            }

            // There may be some some buttons remaining for readability at edit time
            // Clear them out
            _buttonsHorizontalLayoutGroup.transform.ClearChildren();
            
            //Attach only children based on the prefab chosen and the data present
            foreach (DialogButtonData dialogButtonData in _dialogButtonDatas)
            {
                DialogButtonUI dialogButtonUI = Instantiate(_dialogButtonUIPrefab, _buttonsHorizontalLayoutGroup.transform);
                dialogButtonUI.transform.SetAsLastSibling();
                dialogButtonUI.Button.onClick.AddListener(dialogButtonData.OnButtonClicked);
                dialogButtonUI.Text.text = dialogButtonData.Text;
            }
        }
    }
}


//When upgrading from Beamable 0.15.0 to 0.17.4 the "ClearChildren" extension could not be found
//so here is a local copy. The project works well now. - srivello
public static class TransformExtensions2
{
    public static void ClearChildren(this Transform trans)
    {
        while (trans.childCount > 0)
        {
            Transform child = trans.GetChild(0);
            child.SetParent((Transform) null);
            Object.DestroyImmediate((Object) child.gameObject);
        }
    }
}
