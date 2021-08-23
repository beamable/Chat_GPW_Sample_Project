using System;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.GPW.Content
{
   [Serializable]
   public class ProductContentRef : ContentRef<ProductContent> {}
   
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   public class ProductContentView 
   {
      //  Fields ---------------------------------------
      public ProductContent ProductContent = null;
      public int Price;
      public int Quantity;

      public ProductContentView(ProductContent productContent, int randomSeed)
      {
         ProductContent = productContent;
         
         // Set a Price, RANDOMLY within the permitted data range, that
         // is DETERMINISTIC (same) for all game clients
         Random random = new System.Random(randomSeed);
         int delta = ProductContent.PriceMax - ProductContent.PriceMin;
         double p = random.NextDouble();
         Debug.Log("p1  = " + p);
         p = ProductContent.PriceMin + p * delta;
         Debug.Log("p2  = " + p);
         Price = (int)p;
         Quantity = random.Next(1, 10);
      }

      
   }
   
   /// <summary>
   /// Server-side data: Represents a commodity for buy/sell
   /// </summary>
   [ContentType("product")]
   public class ProductContent : ItemContent
   {
      //  Fields ---------------------------------------
      public string Title = "";
      public int PriceMin = 1;
      public int PriceMax = 10;
   }
}
