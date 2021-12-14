using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Content
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
      [SerializeField]
      public ProductData ProductData = null;
   }
}

