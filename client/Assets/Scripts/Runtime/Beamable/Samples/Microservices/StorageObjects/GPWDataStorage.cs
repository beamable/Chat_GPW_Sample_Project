using System.Collections.Generic;
using Beamable.Samples.GPW.Content;
using MongoDB.Bson;

namespace Beamable.Server
{
    [StorageObject("GPWDataStorage")]
    public class GPWDataStorage : MongoStorageObject
    {

    }
    
    public class LocationContentViewsWrapper
    {
        public ObjectId Id;
        public List<LocationContentView> LocationContentViews;
    }
}
