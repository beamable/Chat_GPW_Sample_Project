using System;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.Core.UI.ScrollingList
{
    /// <summary>s
    /// PersistentData provider of ScrollingList's <see cref="ListBank"/>.
    /// </summary>
    [Serializable]
    public class ProductContentListBank : ListBank
    {
        //  Properties  ----------------------------------

        //  Fields  --------------------------------------
        private List<ProductContent> _contents;

        //  Other Methods  --------------------------------
        public void SetContents (List<ProductContent> contents)
        {
            _contents = contents;
        }
        
        public override object GetListContent(int index)
        {
            return _contents[index];
        }

        public override int GetListLength()
        {
            return _contents.Count;
        }
        
        //  Event Handlers  -------------------------------
    }
}
