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
      
      [ClientCallable]
      public async Task<bool> CreateLocationContentViews( 
         List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         IDataFactory dataFactory = new GPWBasicDataFactory();

         List<LocationContentView> locationContentViews = 
            await dataFactory.CreateLocationContentViews(locationContents, productContents);

         bool isSuccess = false;
         try
         {
            var db = Storage.GetDatabase<GPWDataStorage>();
            var collection = db.GetCollection<LocationContentViewsWrapper>("location_content_views");
            
            //Find the ONE element. TODO: Refactor?
            //locationContentViews = collection.InsertOne()

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
            locationContentViews = collection
                  .Find(e => e.LocationContentViews != null)
                  .ToList()[0].LocationContentViews;
         }
         catch (Exception e)
         {
            Debug.LogError(e.Message);
         }

         return locationContentViews;
      }
   }
}