using System.Collections.Generic;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
using MongoDB.Bson;

namespace Beamable.Server
{
    [StorageObject("GPWDataStorage")]
    public class GPWDataStorage : MongoStorageObject
    {

    }
    
    /// <summary>
    /// This class wraps concepts of MongoDB (ex. ObjectID)
    /// and concepts of the games custom datatypes (ex. LocationContentView)
    /// </summary>
    public class LocationContentViewsWrapper
    {
        public ObjectId Id;
        public List<LocationContentView> LocationContentViews;
    }
}
