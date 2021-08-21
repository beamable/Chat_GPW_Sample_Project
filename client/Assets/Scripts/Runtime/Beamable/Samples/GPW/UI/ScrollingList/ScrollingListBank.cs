using System;
using AirFishLab.ScrollingList;
using UnityEngine;

namespace Beamable.Samples.GPW.UI
{
    [Serializable]
    public class ColorString
    {
        public Color color;
        public string name;
    }
    
    /// <summary>
    /// Data provider of ScrollingList's <see cref="BaseListBank"/>.
    /// </summary>
    public class ScrollingListBank : BaseListBank
    {
        //  Properties  ----------------------------------
        public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }

        //  Fields  --------------------------------------
        private long _localPlayerDbid;
        
        [SerializeField]
        private ColorString[] _contents;

        //  Other Methods  --------------------------------
        public override object GetListContent(int index)
        {
            return _contents[index];
        }

        public override int GetListLength()
        {
            return _contents.Length;
        }
        
        //  Event Handlers  -------------------------------
    }
}
