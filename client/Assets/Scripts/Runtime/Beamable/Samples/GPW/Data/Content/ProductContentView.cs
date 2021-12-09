using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.GPW.Content
{
   public class Goods
   {
      public int Price;
      public int Quantity;

      public override string ToString()
      {
         return $"[Goods (Price={Price}, Quantity={Quantity})";
      }
   }

   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   public class ProductContentView 
   {
      //  Fields ---------------------------------------
      public ProductContent ProductContent = null;
      public Goods OwnedGoods = new Goods();
      public Goods MarketGoods = new Goods();
      public bool CanBuy = false;
      public bool CanSell = false;
      
      public ProductContentView(ProductContent productContent, Random random)
      {
         ProductContent = productContent;

         int priceRange = ProductContent.PriceMax - ProductContent.PriceMin;
         float priceRangePercent = random.Next(100) * .01f;
         priceRangePercent = ProductContent.PriceMin + priceRangePercent * priceRange;
         
         // Update Mkt
         MarketGoods.Price = (int)priceRangePercent;
         MarketGoods.Quantity = random.Next(1, 10);
         
         Debug.Log($"ProductContentView = {ProductContent.Title}, MarketGoods = {MarketGoods}");
      }

      public override string ToString()
      {
         return $"[PCV({ProductContent.Title}, Mkt: {MarketGoods}, Owned: {OwnedGoods})]";
      }
   }
}
