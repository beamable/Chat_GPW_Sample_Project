using System.Collections.Generic;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Data;
using UnityEngine;

namespace Beamable.Samples.GPW.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Base
   /// </summary>
   public class BaseSceneUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public DialogSystem DialogSystem { get { return _dialogSystem; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      [Header ("Base Properties")]
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private DialogSystem _dialogSystem = null;
      
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected virtual void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }
   }
}