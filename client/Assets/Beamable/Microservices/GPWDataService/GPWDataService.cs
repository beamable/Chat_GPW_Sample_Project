using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Factories;
using MongoDB.Driver;
using UnityEngine;

namespace Beamable.Server
{
    [Microservice("GPWDataService")]
    public class GPWDataService : Microservice
    {
        private const string CollectionName = "location_content_views_wrapper";
            
        [ClientCallable]
        public bool IsMicroServiceReady()
        {
            return true;
        }
        
        [ClientCallable]
        public bool IsMicroStorageReady()
        {
            return Storage != null;
        }
        
        /// <summary>
        /// Determines if data yet exists in the database
        /// </summary>
        [ClientCallable]
        public async Task<bool> HasLocationContentViews()
        {
            var locationContentViews = await GetLocationContentViews_Internal(false);
            return locationContentViews != null &&
                   locationContentViews.LocationContentViews != null
                   && locationContentViews.LocationContentViews.Count > 0;
        }

        /// <summary>
        /// Get the data from the database
        /// </summary>
        [ClientCallable]
        public async Task<LocationContentViewCollection> GetLocationContentViews()
        {
            return await GetLocationContentViews_Internal(false);
        }

        private async Task<LocationContentViewCollection> GetLocationContentViews_Internal(bool isSuppressErrors)
        {
            List<LocationContentView> locationContentViews = null;
        
            try
            {
                var mongoDatabase = Storage.GetDatabase<GPWDataStorage>();
                var mongoCollection = mongoDatabase.GetCollection<LocationContentViewsWrapper>(CollectionName);
                var result = await mongoCollection.FindAsync(_ => true);
                var locationContentViewsWrappers = result.ToList();

                if (locationContentViewsWrappers.Count == 1)
                {
                    var locationContentViewsWrapper = locationContentViewsWrappers[0];
                
                    if (locationContentViewsWrapper.LocationContentViewCollection != null)
                    {
                        locationContentViews = locationContentViewsWrappers[0].LocationContentViewCollection
                            .LocationContentViews;
                    }
                    else
                    {
                        Debug.LogError($"GetLocationContentViews_Internal() failed.");
                    }
                }
                else
                {
                    Debug.LogError($"GetLocationContentViews_Internal() failed. " +
                                   $"Count = {locationContentViewsWrappers.Count}");
                }
      
            }
            catch (Exception e)
            {
                // Do nothing. This means the LocationContentViewsWrapper does not yet exist in the db.
                // That is sometimes expected (e.g. first run) and is ok.
                if (!isSuppressErrors)
                {
                    Debug.Log($"GetLocationContentViews () failed. Error={e.Message}");
                }
            }
        
            Debug.Log("so... 4");
            
            LocationContentViewCollection locationContentViewCollection = new LocationContentViewCollection();
            locationContentViewCollection.LocationContentViews = locationContentViews;
            return locationContentViewCollection;
        }

        /// <summary>
        /// Create the data in the database.
        /// 
        /// In production it is recommended...
        /// * 1. Use the attribute [AdminOnlyCallable] is recommended to protect
        /// vital methods like this. Such a method requires a user with Admin privileges as the caller.
        /// * 2. Don't allow GetLocationContentViews() if there is already data in the database. 
        /// 
        /// However, in development here...
        /// * 1. For ease-of-use in the sample project [ClientCallable] is used to allow
        /// any users with any privileges to call.
        /// * 2. We allow creation of data regardless if data is present in the database
        /// 
        /// </summary>
        [ClientCallable]
        public async Task<bool> CreateLocationContentViews(
            List<LocationData> locationDatas, List<ProductData> productDatas)
        {
            //TODO: MAKE SURE it does not create If THERE IS ALREADY A DB

            IDataFactory dataFactory = new GPWBasicDataFactory();
        
            List<LocationContentView> locationContentViews =
                await dataFactory.GetLocationContentViews(locationDatas, productDatas);
        
            bool isSuccess = false;
            try
            {
                var db = Storage.GetDatabase<GPWDataStorage>();
                
                // This section is verbose but...
                
                //The wrapper helps the DB
                var collection = db.GetCollection<LocationContentViewsWrapper>(CollectionName);

                // Delete any/all previous data
                await collection.DeleteManyAsync(_ => true);
                
                //And this custom collection helps some method-return-value serialization
                LocationContentViewCollection locationContentViewCollection = new LocationContentViewCollection();
                locationContentViewCollection.LocationContentViews = locationContentViews;
                
                // Insert the one new value
                collection.InsertOne(new LocationContentViewsWrapper()
                {
                    LocationContentViewCollection = locationContentViewCollection
                });
        
                isSuccess = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        
            return isSuccess;
        }
    }
}