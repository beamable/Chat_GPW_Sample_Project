using System.Collections.Generic;
using System.Threading.Tasks;
using AirFishLab.ScrollingList;
using Beamable.Common.Api;
using Beamable.Samples.GPW.Content;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Samples.Core.UI.ScrollingList
{
   public class ScrollingListEvent: UnityEvent <ScrollingList>{}
   
   /// <summary>
   /// Extends <see cref="CircularScrollingList"/> for
   /// specific workflow of sample projects
   /// </summary>
   public class ScrollingList : CircularScrollingList
   {
      //  Events ---------------------------------------
      public ScrollingListEvent OnInitialized = new ScrollingListEvent();
      
      //  Properties -----------------------------------
      public CanvasGroup CanvasGroup { get { return _canvasGroup; } }

      //  Fields ---------------------------------------
      [Header("Child Properties")]
      [SerializeField]
      private CanvasGroup _canvasGroup = null;


      //  Other Methods   ------------------------------
      public async Task<EmptyResponse> InitializeOnDelay(List<ProductContentView> list, int delayMilliseconds)
      {
         // Wait one frame so rendering works in all cases
         await Task.Delay(delayMilliseconds);
         SetContents(list);
         Initialize();
         return new EmptyResponse();
      }
      public override void Initialize()
      {
         if (_isInitialized)
         {
            return;
         }
         
         base.Initialize();
        
      }

      public void SetContents (List<ProductContentView> contents)
      {
         if (ListBank == null)
         {
            ListBank = gameObject.AddComponent<ProductContentListBank>();
         }
         
         (ListBank as ProductContentListBank).SetContents(contents);
      }
      
      public override void InitializeListComponents()
      {
         base.InitializeListComponents();
         OnInitialized.Invoke(this);
      }
   }
}