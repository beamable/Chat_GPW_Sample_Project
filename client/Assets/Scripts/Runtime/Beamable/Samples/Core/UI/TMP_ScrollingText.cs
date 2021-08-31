using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI
{
   /// <summary>
   /// Wraps a <see cref="TMP_Text"/>. Allows game makers to 
   /// queue multiple display texts to the same UI text field 
   /// and each will be displayed in series over time.
   /// </summary>
   public class TMP_ScrollingText : MonoBehaviour
   {
      //  Properties --------------------------------------
      public TMP_HyperlinkHandler HyperlinkHandler { get { return _hyperlinkHandler;}}

      //  Fields ---------------------------------------
      [SerializeField] 
      private TMP_Text _mainText = null;
      
      [SerializeField] 
      private TMP_HyperlinkHandler _hyperlinkHandler = null;
      
      [SerializeField] 
      private ScrollRect _scrollRect = null;

      //  Unity Methods   ------------------------------

      //  Other Methods   ------------------------------
      public void SetText(string message)
      {
         _mainText.text = message;
         ScrollToBottom();
      }
      
      public void ScrollToTop()
      {
         _scrollRect.normalizedPosition = new Vector2(0, 1);
      }
      public void ScrollToBottom()
      {
         _scrollRect.normalizedPosition = new Vector2(0, 0);
      }
   }
}