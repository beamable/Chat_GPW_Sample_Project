using System.Collections;
using Beamable.Api;
using Beamable.Common;
using Beamable.Service;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beamable.Samples.Core.Utilities
{ 
    /// <summary>
    /// These are hacks and other shortcuts used to
    /// assist in the examples.
    ///
    /// HACK: These hacks are not recommended for production usage
    /// 
    /// </summary>
    public static class ExampleProjectHacks
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Clears all data related to the active runtime user(s)
        /// </summary>
        public static void ClearDeviceUsersAndReloadScene()
        {
            Beam.ClearAndStopAllContexts()
                .FlatMap(_ => Beam.ResetToScene(SceneManager.GetSceneByBuildIndex(0).name));
        }
    }
}