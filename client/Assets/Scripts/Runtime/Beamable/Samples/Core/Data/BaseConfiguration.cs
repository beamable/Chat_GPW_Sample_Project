using Beamable.Samples.Core.Debugging;
using UnityEngine;

namespace Beamable.Samples.Core.Data
{
    public class BaseConfiguration : ScriptableObject
    {
        //  Properties -----------------------------------
        public DebugLogLevel DebugLogLevel
        {
            get { return _debugLogLevel; }
        }
        
        //  Fields ----------------------------------------
        [Header("Base Fields")]
        [SerializeField] 
        private DebugLogLevel _debugLogLevel = DebugLogLevel.Disabled;

        //  Unity Methods ---------------------------------
        protected virtual void OnEnable()
        {
            SetInstance();
        }

        protected virtual void OnDisable()
        {
            ClearInstance();
        }

        protected virtual void OnValidate()
        {
            SetInstance();
        }
        
        //  Subsystem -------------------------------------

        #region DebugLog
        // KEEP AS PRIVATE : Not intended as full Singleton implementation.
        // This is just to ease global use of "Configuration.Debugger.Log("Hello World");"
        private static BaseConfiguration _instance = null;

        /// <summary>
        /// Allow easy access to calls of "Configuration.Debugger.Log("Hello World");".
        /// </summary>
        public static Debugger Debugger { get { return _debugger; } }

        private static Debugger _debugger = null;

        private void SetInstance()
        {
            // When OnEnable (Always) and also...
            // When OnValidate (only when app is running)
            // Do update the debugging
            if (!Application.isPlaying)
            {
                //return;
            }

            _instance = this;
            _debugger = new Debugger(_instance.DebugLogLevel);
        }

        private void ClearInstance()
        {
            _instance = null;
            _debugger = null;
        }

        #endregion
    }
}