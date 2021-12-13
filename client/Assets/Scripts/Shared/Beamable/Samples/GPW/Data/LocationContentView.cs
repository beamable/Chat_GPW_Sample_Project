using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.GPW.Data
{
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   public class LocationContentView 
   {
      //  Fields ---------------------------------------
      public LocationData LocationData = null;
      public List<ProductContentView> ProductContentViews = null;
      
      public LocationContentView(LocationData locationData, 
         List<ProductData> productDatas)
      {
         LocationData = locationData;
         Debug.Log($"----LocationContentView = {locationData.Title}");

         ProductContentViews = new List<ProductContentView>();
         
         // Set a Price, RANDOMLY within the permitted data range, that
         // is DETERMINISTIC (same) for all game clients
         Random random = new System.Random(LocationData.RandomSeed);
         
         // Give a client-side copy of the products to each 
         // location for uniqueness.
         foreach (ProductData productData in productDatas)
         {
            ProductContentViews.Add(
               new ProductContentView(productData, random));
         }
      }
      
      
      //  Other Methods -----------------------------------
      public override string ToString()
      {
         return $"[LocationContentView ({LocationData.Title})]";
      }
   }
}
