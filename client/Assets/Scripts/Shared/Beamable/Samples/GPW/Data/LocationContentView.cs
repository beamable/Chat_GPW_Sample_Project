using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.GPW.Data
{
   [Serializable]
   public class LocationContentViewCollection
   {
      public List<LocationContentView> LocationContentViews = new List<LocationContentView>();
   }
   
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   [Serializable]
   public class LocationContentView 
   {
      //  Fields ---------------------------------------
      public LocationData LocationData = null;
      public ProductContentViewCollection ProductContentViewCollection = new ProductContentViewCollection();

      public LocationContentView(LocationData locationData, 
         List<ProductData> productDatas)
      {
         
         LocationData = locationData;
         ProductContentViewCollection.ProductContentViews = new List<ProductContentView>();
         
         // Set a Price, RANDOMLY within the permitted data range, that
         // is DETERMINISTIC (same) for all game clients
         Random random = new System.Random(LocationData.RandomSeed);
         
         // Give a client-side copy of the products to each 
         // location for uniqueness.
         foreach (ProductData productData in productDatas)
         {
            ProductContentViewCollection.ProductContentViews.Add(
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
