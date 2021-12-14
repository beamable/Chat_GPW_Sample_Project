using System;
using Beamable.Samples.GPW.Data;
using MongoDB.Bson;

namespace Beamable.Server
{
    [StorageObject("GPWDataStorage")]
    public class GPWDataStorage : MongoStorageObject
    {
    }
    
    /// <summary>
    /// This class wraps BOTH:
    /// * the concepts of MongoDB (ex. ObjectID) which is required
    /// * and the concepts of the game's custom datatypes (ex. LocationContentView)
    /// </summary>
    [Serializable]
    public class LocationContentViewsWrapper
    {
        public ObjectId Id;
        public LocationContentViewCollection LocationContentViewCollection;
    }
}
