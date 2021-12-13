using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Factories;
using Beamable.Server.Clients;
using UnityEngine;

#pragma warning disable 1998
namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Client-side factory for the loaded data content
   /// </summary>
   public class GPWMicroStorageDataFactory : IDataFactory
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public async Task<List<LocationContentView>> CreateLocationContentViews(
         List<LocationData> locationDatas, List<ProductData> productDatas)
      {
         
         GPWDataServiceClient gpwDataServiceClient = new GPWDataServiceClient();
         
         bool testBool = await gpwDataServiceClient.GetTestBool();
         Debug.Log($"1 GetTestWithoutFactory() outside with testBool= {testBool}");
         
         bool testBool2 = await gpwDataServiceClient.GetTestBool();
         Debug.Log($"2 GetTestWithoutFactory() outside with testBool= {testBool2}");

         bool hasLocationContentViews = await gpwDataServiceClient.HasLocationContentViews();
         Debug.Log($"3 HasLocationContentViews() outside with hasLocationContentViews= {hasLocationContentViews}");
         // if (!hasLocationContentViews)
         // {
         //    // Create list
         //    bool isSuccess = await gpwDataServiceClient.CreateLocationContentViews(locationDatas, productDatas);
         //
         //    if (!isSuccess)
         //    {
         //       throw new Exception("CreateLocationContentViews() failed with isSuccess={isSuccess}.");
         //    }
         // }

         
         // Populate List
         LocationContentViewCollection locationContentViewCollection = 
            await gpwDataServiceClient.GetLocationContentViewsWithoutDB(locationDatas, productDatas);
         
         Debug.Log($"GetTestWithoutFactory() outside with Count= " +
                   $"{locationContentViewCollection.LocationContentViews?.Count}");
         
         //todo rEMOVE
         if (locationContentViewCollection.LocationContentViews == null && 
             locationContentViewCollection.LocationContentViews.Count == 1)
         {
            Debug.Log("NOTHING FOUND");
            return null;
         }
         
         //  Sort list: A to Z
         locationContentViewCollection.LocationContentViews.Sort((p1, p2) =>
         {
            return string.Compare(p2.LocationData.Title, p2.LocationData.Title, 
               StringComparison.InvariantCulture);
         });
         
         // Return List
         return locationContentViewCollection.LocationContentViews;
      }
   }
}
