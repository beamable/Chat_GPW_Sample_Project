using UnityEngine;
using UnityEngine.UI;

namespace AirFishLab.ScrollingList.Demo
{
    public class DisplayAndSelectExample : MonoBehaviour
    {
        [SerializeField]
        private CircularScrollingList _list;
        [SerializeField]
        private Text _displayText;
        [SerializeField]
        private Text _centeredContentText;

        public void DisplayCenteredContent()
        {
            var contentID = _list.GetCenteredContentID();
            var centeredContent = (int) _list.ListBank.GetListContent(contentID);
            _displayText.text = "Centered content: " + centeredContent;
        }

        public void GetSelectedContentID(int selectedContentID)
        {
            Debug.Log("Selected content ID: " + selectedContentID +
                      ", Content: " +
                      (int) _list.ListBank.GetListContent(selectedContentID));
        }

        public void OnListCenteredContentChanged(int centeredContentID)
        {
            var content = (int) _list.ListBank.GetListContent(centeredContentID);
            _centeredContentText.text = "(Auto updated)\nCentered content: " + content;
        }

        public void OnMovementEnd()
        {
            Debug.Log("Movement Ends");
        }
    }
}
