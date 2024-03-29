﻿using System;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;

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
        private List<ProductContentView> _contents;

        //  Other Methods  --------------------------------
        public void SetContents (List<ProductContentView> contents)
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
