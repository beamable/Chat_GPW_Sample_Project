using System;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using UnityEngine;

namespace Beamable.Samples.Core.UI.ScrollingList
{
    [Serializable]
    public class ColorString
    {
        public Color color;
        public string name;
    }

    /// <summary>s
    /// PersistentData provider of ScrollingList's <see cref="BaseListBank"/>.
    /// </summary>
    [Serializable]
    public class ColorStringListBank : ListBank
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; }}

        //  Fields  --------------------------------------
        private bool _isInitialized = false;
        private List<ColorString> _contents;

        //  Other Methods  --------------------------------
        public void Initialize (List<ColorString> contents)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _contents = contents;
            _isInitialized = true;
        }
        
        public override object GetListContent(int index)
        {
            if (!_isInitialized)
            {
                throw new Exception("Must Be Initialized");
            }
            
            return _contents[index];
        }

        public override int GetListLength()
        {
            if (!_isInitialized)
            {
                throw new Exception("Must Be Initialized");
            }
            
            return _contents.Count;
        }
        
        //  Event Handlers  -------------------------------
    }
}
