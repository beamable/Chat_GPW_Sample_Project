using System;
using System.Collections.Generic;
using Beamable.Samples.GPW.Content;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.GPW.Data
{
   [Serializable]
   public class ProductContentViewCollection
   {
      public List<ProductContentView> ProductContentViews = new List<ProductContentView>();
   }
   
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   [Serializable]
   public class ProductContentView 
   {
      //  Fields ---------------------------------------
      public ProductData ProductData = null;
      public Goods OwnedGoods = new Goods();
      public Goods MarketGoods = new Goods();
      public bool CanBuy = false;
      public bool CanSell = false;
      
      public ProductContentView(ProductData productData, Random random)
      {
         ProductData = productData;

         int priceRange = ProductData.PriceMax - ProductData.PriceMin;
         float priceRangePercent = random.Next(100) * .01f;
         priceRangePercent = ProductData.PriceMin + priceRangePercent * priceRange;
         
         // Update Mkt
         MarketGoods.Price = (int)priceRangePercent;
         MarketGoods.Quantity = random.Next(1, 10);
         
      }

      public override string ToString()
      {
         return $"[PCV({ProductData.Title}, Mkt: {MarketGoods}, Owned: {OwnedGoods})]";
      }
   }
}
