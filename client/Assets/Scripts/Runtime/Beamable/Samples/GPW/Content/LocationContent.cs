using System;
using System.Collections.Generic;
using Beamable.Common.Content;

namespace Beamable.Samples.GPW.Content
{
   [Serializable]
   public class LocationContentRef : ContentRef<LocationContent> {}
   
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
         
         // Give a client-side copy of the products to each 
         // location for uniqueness.
         foreach (ProductContent productContent in productContents)
         {
            ProductContentViews.Add(
               new ProductContentView(productContent, LocationContent.RandomSeed));
         }
      }
   }
   
   /// <summary>
   /// Server-side data: Represents a geographic in-game location (e.g. North America)
   /// </summary>
   [ContentType("location")]
   public class LocationContent : ContentObject
   {
      //  Fields ---------------------------------------
      public string Title = "";
      public int RandomSeed = 1;
   }
}
