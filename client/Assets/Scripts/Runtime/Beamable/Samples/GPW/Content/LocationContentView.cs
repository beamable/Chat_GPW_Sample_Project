using System;
using System.Collections.Generic;
using Beamable.Common.Content;

namespace Beamable.Samples.GPW.Content
{
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   public class LocationContentView 
   {
      //  Fields ---------------------------------------
      public LocationContent LocationContent = null;
      public List<ProductContentView> ProductContentViews = null;
      
      public LocationContentView(LocationContent locationContent, 
         List<ProductContent> productContents)
      {
         LocationContent = locationContent;

         ProductContentViews = new List<ProductContentView>();
         
         // Set a Price, RANDOMLY within the permitted data range, that
         // is DETERMINISTIC (same) for all game clients
         Random random = new System.Random(LocationContent.RandomSeed);
         
         // Give a client-side copy of the products to each 
         // location for uniqueness.
         foreach (ProductContent productContent in productContents)
         {
            ProductContentViews.Add(
               new ProductContentView(productContent, random));
         }
      }
   }
}
