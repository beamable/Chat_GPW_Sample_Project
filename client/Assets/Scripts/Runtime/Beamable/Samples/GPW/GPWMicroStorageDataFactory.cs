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
   public class GPWMicroStorageDataFactory : IDataFactory
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public async Task<List<LocationContentView>> CreateLocationContentViews(
         List<LocationContent> locationContents, List<ProductContent> productContents)
      {
         
         // Create list
         
         // Populate List
         GPWDataServiceClient gpwDataServiceClient = new GPWDataServiceClient();
         
         if (await gpwDataServiceClient.HasLocationContentViews())
         {
            //gpwDataServiceClient.CreateLocationContentViews(locationContents, productContents);
         }
         
         List<LocationContentView> locationContentViews = null; //await gpwDataServiceClient.GetLocationContentViews();;
         
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
