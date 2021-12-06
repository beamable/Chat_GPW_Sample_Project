using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Content;
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
      public async Task<List<LocationContentView>> CreateLocationContentView(
         List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         // Create list
         List<LocationContentView> locationContentViews = new List<LocationContentView>();
         
         // Populate List
         foreach (var locationContent in  locationContents)
         {
            // Populate with new, generated client-side data
            LocationContentView locationContentView =
               new LocationContentView(locationContent, productContents);

            locationContentViews.Add(locationContentView);
         }
         
         //  Sort list: A to Z
         locationContentViews.Sort((p1, p2) =>
         {
            return string.Compare(p2.LocationContent.Title, p2.LocationContent.Title, 
               StringComparison.InvariantCulture);
         });

         // Return List
         return locationContentViews;
      }
   }
}
