using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Leaderboard
   /// </summary>
   public class LeaderboardUIView : BaseSceneUIView
   {
      //  Properties -----------------------------------
      public Button BackButton { get { return _backButton; } }

      //  Fields ---------------------------------------
      [Header("Child Properties")]
      
      [SerializeField]
      private Button _backButton = null;
      
      //  Unity Methods   ------------------------------
      protected override void Start()
      {
         base.Start();
        
      }

      //  Other Methods   ------------------------------
      
      
      //  Event Handlers  ------------------------------

   }
}