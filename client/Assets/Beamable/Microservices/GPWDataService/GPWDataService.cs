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
        public async Task<bool> HasLocationContentViews()
        {
            var locationContentViews = await GetLocationContentViews();
            return locationContentViews != null && locationContentViews.Count > 0;
        }


        /// <summary>
        /// TODO: Make a comment here about AdminCallable vs ClientCallable
        /// </summary>
        [ClientCallable]
        public async Task<bool> CreateLocationContentViews(
            List<LocationData> locationDatas, List<ProductData> productDatas)
        {
            //TODO: MAKE SURE it does not create If THERE IS ALREADY A DB


            IDataFactory dataFactory = new GPWBasicDataFactory();

            List<LocationContentView> locationContentViews =
                await dataFactory.CreateLocationContentViews(locationDatas, productDatas);

            bool isSuccess = false;
            try
            {
                var db = Storage.GetDatabase<GPWDataStorage>();
                var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");

                collection.InsertOne(new LocationContentViewsWrapper()
                {
                    LocationContentViews = locationContentViews
                });

                isSuccess = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return isSuccess;
        }

        [ClientCallable]
        public async Task<List<LocationContentView>> GetLocationContentViews()
        {
            List<LocationContentView> locationContentViews = null;

            try
            {
                var db = Storage.GetDatabase<GPWDataStorage>();
                var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");

                //Find the ONE element. TODO: Refactor?
                var x = collection
                    .Find(e => e.LocationContentViews != null)
                    .ToList()[0];

                locationContentViews = x.LocationContentViews;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            Debug.Log($"GetLocationContentViews() end with Count= {locationContentViews.Count}");
            Debug.Log($"GetLocationContentViews() end with type [0] of {locationContentViews[0].GetType().Name}");
            return locationContentViews;
        }

        [ClientCallable]
        public async Task<List<LocationContentView>> GetTestWithoutDB(
            List<LocationData> locationDatas,
            List<ProductData> productDatas)
        {
            List<LocationContentView> locationContentViews = null;

            IDataFactory dataFactory = new GPWBasicDataFactory();

            locationContentViews =
                await dataFactory.CreateLocationContentViews(locationDatas, productDatas);

            Debug.Log($"GetTestWithoutDB() end with Count= {locationContentViews.Count}");
            Debug.Log($"GetTestWithoutDB() end with type [0] of {locationContentViews[0].GetType().Name}");
            return locationContentViews;
        }

        [ClientCallable]
        public async Task<List<LocationContentView>> GetTestWithoutFactory(
            List<LocationData> locationDatas, List<ProductData> productDatas)
        {
            LocationContent lc = ScriptableObject.CreateInstance<LocationContent>();
            lc.LocationData.Title = "test";
            lc.LocationData.RandomSeed = 99;

            List<LocationContentView> locationContentViews = new List<LocationContentView>();
            locationContentViews.Add(new LocationContentView(lc.LocationData, new List<ProductData>()));


            Debug.Log($"GetTestWithoutFactory() end with Count= {locationContentViews.Count}");
            Debug.Log($"GetTestWithoutFactory() end with type [0] of {locationContentViews[0].GetType().Name}");
            return locationContentViews;
        }

        [ClientCallable]
        public async Task<Goods> GetGoods(
            List<LocationContent> locationContents, List<ProductContent> productContents)
        {
            Goods goods = new Goods();
            goods.Price = 10;
            goods.Quantity = 22;

            //test method
            return goods;
        }

 
            
        [ClientCallable]
        public async Task<List<LocationData>> GetLocationContentsPluralPassthrough(
            List<LocationData> locationDatas)
        {
            return locationDatas;
        }

        
        [ClientCallable]
        public async Task<LocationData> GetLocationContentSinglePassthrough(
            LocationData locationData)
        {
            return locationData;
        }
        
    }
}