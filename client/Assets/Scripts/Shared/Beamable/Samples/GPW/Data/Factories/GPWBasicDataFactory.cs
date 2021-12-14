using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Content;
using Beamable.Samples.GPW.Data.Factories;

#pragma warning disable 1998
namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Client-side factory for the loaded data content
   /// </summary>
   public class GPWBasicDataFactory : IDataFactory
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public async Task<List<LocationContentView>> GetLocationContentViews(
         List<LocationData> locationDatas, List<ProductData> productDatas)
      {
         // Create list
         List<LocationContentView> locationContentViews = new List<LocationContentView>();
         
         // Populate List
         foreach (var locationContent in  locationDatas)
         {
            // Populate with new, generated client-side data
            LocationContentView locationContentView =
               new LocationContentView(locationContent, productDatas);

            locationContentViews.Add(locationContentView);
         }
         
         //  Sort list: A to Z
         locationContentViews.Sort((p1, p2) =>
         {
            return string.Compare(p2.LocationData.Title, p2.LocationData.Title, 
               StringComparison.InvariantCulture);
         });

         // Return List
         return locationContentViews;
      }
   }
}
