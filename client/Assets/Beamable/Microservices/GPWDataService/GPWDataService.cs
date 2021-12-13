using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
using Beamable.Samples.GPW.Data.Factories;
using MongoDB.Driver;
using UnityEngine;

namespace Beamable.Server
{
    [Microservice("GPWDataService")]
    public class GPWDataService : Microservice
    {
        
        [ClientCallable]
        public async Task<bool> GetTestBool()
        {
            return true;
        }
        
        [ClientCallable]
        public async Task<bool> HasLocationContentViews()
        {
            return false;
           // var locationContentViews = await GetLocationContentViews();
            //return locationContentViews != null && locationContentViews.LocationContentViews.Count > 0;
        }


        // /// <summary>
        // /// TODO: Make a comment here about AdminCallable vs ClientCallable
        // /// </summary>
        // [ClientCallable]
        // public async Task<bool> CreateLocationContentViews(
        //     List<LocationData> locationDatas, List<ProductData> productDatas)
        // {
        //     //TODO: MAKE SURE it does not create If THERE IS ALREADY A DB


        //     IDataFactory dataFactory = new GPWBasicDataFactory();
        //
        //     List<LocationContentView> locationContentViews =
        //         await dataFactory.CreateLocationContentViews(locationDatas, productDatas);
        //
        //     bool isSuccess = false;
        //     try
        //     {
        //         var db = Storage.GetDatabase<GPWDataStorage>();
        //         var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");
        //
        //         collection.InsertOne(new LocationContentViewsWrapper()
        //         {
        //             LocationContentViews = locationContentViews
        //         });
        //
        //         isSuccess = true;
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError(e.Message);
        //     }
        //
        //     return isSuccess;
        // }
        //
        // [ClientCallable]
        // public async Task<LocationContentViewCollection> GetLocationContentViews()
        // {
        //     List<LocationContentView> locationContentViews = null;
        //
        //     try
        //     {
        //         var db = Storage.GetDatabase<GPWDataStorage>();
        //         var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");
        //
        //         //Find the ONE element. TODO: Refactor?
        //         var x = collection
        //             .Find(e => e.LocationContentViews != null)
        //             .ToList()[0];
        //
        //         locationContentViews = x.LocationContentViews;
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError(e.Message);
        //     }
        //
        //     Debug.Log($"GetLocationContentViews() end with Count= {locationContentViews.Count}");
        //     Debug.Log($"GetLocationContentViews() end with type [0] of {locationContentViews[0].GetType().Name}");
        //     
        //     LocationContentViewCollection locationContentViewCollection = new LocationContentViewCollection();
        //     locationContentViewCollection.LocationContentViews = locationContentViews;
        //     return locationContentViewCollection;
        // }

        [ClientCallable]
        public async Task<LocationContentViewCollection> GetLocationContentViewsWithoutDB(
            List<LocationData> locationDatas,
            List<ProductData> productDatas)
        {
            List<LocationContentView> locationContentViews = null;
        
            IDataFactory dataFactory = new GPWBasicDataFactory();
        
            locationContentViews =
                await dataFactory.CreateLocationContentViews(locationDatas, productDatas);
        
            Debug.Log($"GetTestWithoutDB() end with Count= {locationContentViews.Count}");
            Debug.Log($"GetTestWithoutDB() end with type [0] of {locationContentViews[0].GetType().Name}");
        
            LocationContentViewCollection locationContentViewCollection = new LocationContentViewCollection();
            locationContentViewCollection.LocationContentViews = locationContentViews;
            return locationContentViewCollection;
        }
    }
}