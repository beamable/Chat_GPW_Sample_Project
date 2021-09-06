using System.Collections;
using Beamable.Api;
using Beamable.Service;
using UnityEngine;

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
            // Reset the system. Then reload the current scene
            PlatformService platformService = ServiceManager.Resolve<PlatformService>();
            platformService.ClearDeviceUsers();
            ServiceManager.OnTeardown();
        }
    }
}