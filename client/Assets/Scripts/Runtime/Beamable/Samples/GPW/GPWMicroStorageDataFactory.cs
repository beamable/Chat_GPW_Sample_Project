using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Content;
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
         List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         
         GPWDataServiceClient gpwDataServiceClient = new GPWDataServiceClient();

         bool hasLocationContentViews = await gpwDataServiceClient.HasLocationContentViews();
         
         if (!hasLocationContentViews)
         {
            // Create list
            await gpwDataServiceClient.CreateLocationContentViews(locationContents, productContents);
         }
         
         // Populate List
         List<LocationContentView> locationContentViews = await gpwDataServiceClient.GetTestWithoutFactory(locationContents, productContents);

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
            return string.Compare(p2.LocationContent.Title, p2.LocationContent.Title, 
               StringComparison.InvariantCulture);
         });

         // Return List
         return null;
      }
   }
}
