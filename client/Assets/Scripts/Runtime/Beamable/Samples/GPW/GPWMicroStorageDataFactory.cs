using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
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

         bool hasLocationContentViews = await gpwDataServiceClient.HasLocationContentViews();
         
         if (!hasLocationContentViews)
         {
            // Create list
            await gpwDataServiceClient.CreateLocationContentViews(locationDatas, productDatas);
         }

         
         
         // FAILURE!
         Debug.Log("\n");
         Debug.Log($"2 CALL GetLocationContentsPluralPassthrough() with locationDatas[0] = " + locationDatas[0]);
         var result2 =
            await gpwDataServiceClient.GetLocationContentsPluralPassthrough(locationDatas);
         Debug.Log($"2 RETURN GetLocationContentsPluralPassthrough() outside with result= {result2}");
         Debug.Log($"2 RETURN GetLocationContentsPluralPassthrough() outside with result[0]= {result2[0]}");
         
         
         
         Debug.Log("\n");
         Debug.Log($"3 CALL GetLocationContentSinglePassthrough() with locationDatas[0] = " + locationDatas[0]);
         var result3 =
            await gpwDataServiceClient.GetLocationContentSinglePassthrough(locationDatas[0]);
         Debug.Log($"3 RETURN GetLocationContentSinglePassthrough() outside with result= {result3}");
         
         
         
         
         // Populate List
         List<LocationContentView> locationContentViews = 
            await gpwDataServiceClient.GetTestWithoutFactory(locationDatas, productDatas);
         
         Debug.Log($"GetTestWithoutFactory() outside with Count= {locationContentViews?.Count}");
         
         //todo rEMOVE
         if (locationContentViews == null)
         {
            Debug.Log("NOTHING FOUND");
            return null;
         }
         
         //  Sort list: A to Z
         locationContentViews.Sort((p1, p2) =>
         {
            return string.Compare(p2.LocationData.Title, p2.LocationData.Title, 
               StringComparison.InvariantCulture);
         });

         // Return List
         return null;
      }
   }
}
