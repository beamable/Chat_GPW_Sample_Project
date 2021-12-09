using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW;
using Beamable.Samples.GPW.Content;
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
         List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         
         //TODO: MAKE SURE it does not create If THERE IS ALREADY A DB
         
         
         IDataFactory dataFactory = new GPWBasicDataFactory();

         List<LocationContentView> locationContentViews = 
            await dataFactory.CreateLocationContentViews(locationContents, productContents);

         bool isSuccess = false;
         try
         {
            var db = Storage.GetDatabase<GPWDataStorage>();
            var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");
            
            collection.InsertOne( new LocationContentViewsWrapper()
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
      public async Task<List<LocationContentView>> GetTestWithoutDB ( List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         List<LocationContentView> locationContentViews = null;

         IDataFactory dataFactory = new GPWBasicDataFactory();

         locationContentViews = 
            await dataFactory.CreateLocationContentViews(locationContents, productContents);
         
         Debug.Log($"GetTestWithoutDB() end with Count= {locationContentViews.Count}");
         Debug.Log($"GetTestWithoutDB() end with type [0] of {locationContentViews[0].GetType().Name}");
         return locationContentViews;
      }
      
      [ClientCallable]
      public async Task<List<LocationContentView>> GetTestWithoutFactory ( List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         LocationContent lc = ScriptableObject.CreateInstance<LocationContent>();
         lc.Title = "test";
         lc.RandomSeed = 99;
         
         List<LocationContentView> locationContentViews = new List<LocationContentView>();
         locationContentViews.Add( new LocationContentView(lc, new List<ProductContent>()));

         
         Debug.Log($"GetTestWithoutFactory() end with Count= {locationContentViews.Count}");
         Debug.Log($"GetTestWithoutFactory() end with type [0] of {locationContentViews[0].GetType().Name}");
         return locationContentViews;
      }
   }
}