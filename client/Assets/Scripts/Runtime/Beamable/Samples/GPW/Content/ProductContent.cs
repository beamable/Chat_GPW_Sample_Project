using System;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace Beamable.Samples.GPW.Content
{
   [Serializable]
   public class ProductContentRef : ContentRef<ProductContent> {}
   
   /// <summary>
   /// Store the data: Represents a commodity for buy/sell
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
