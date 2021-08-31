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
