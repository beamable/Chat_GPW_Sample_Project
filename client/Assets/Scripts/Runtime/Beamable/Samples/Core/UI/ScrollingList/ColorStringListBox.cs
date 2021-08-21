using AirFishLab.ScrollingList;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI.ScrollingList
{
    /// <summary>
    /// Renderer of ScrollingList's <see cref="ListBox"/>.
    /// </summary>
    public class ColorStringListBox : ListBox
    {
		
        //  Properties  ----------------------------------
        public TMP_Text Text { get { return _contentText; }  }

        //  Fields  --------------------------------------
        [SerializeField]
        private Image _contentImage;
        
        [SerializeField]
        private TMP_Text _contentText;
        
        //  Other Methods  --------------------------------
        protected override void UpdateDisplayContent(object content)
        {
            var colorString = (ColorString) content;
            _contentImage.color = colorString.color;
            _contentText.text = colorString.name;
        }
        
        //  Event Handlers  -------------------------------
    }
}

