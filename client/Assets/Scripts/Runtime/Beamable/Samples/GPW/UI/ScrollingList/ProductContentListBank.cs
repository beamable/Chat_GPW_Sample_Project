using System;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.Core.UI.ScrollingList
{
    /// <summary>s
    /// Data provider of ScrollingList's <see cref="ListBank"/>.
    /// </summary>
    [Serializable]
    public class ProductContentListBank : ListBank
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; }}

        //  Fields  --------------------------------------
        private bool _isInitialized = false;
        private List<ProductContent> _contents;

        //  Other Methods  --------------------------------
        public void Initialize (List<ProductContent> contents)
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
