using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.UI
{
    public class RefreshEvent : UnityEvent<GameService>{}
	
    /// <summary>
    /// Renderer of ScrollingList's <see cref="ListBox"/>.
    /// </summary>
    public class ScrollingListBox : ListBox
    {
        //  Events  --------------------------------------
        public RefreshEvent OnRefresh = new RefreshEvent();
		
        //  Properties  ----------------------------------
        public Text Text { get { return _contentText; }  }

        //  Fields  --------------------------------------
        [SerializeField]
        private Image _contentImage;
        
        [SerializeField]
        private Text _contentText;
        
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

