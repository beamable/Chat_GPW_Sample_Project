using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Beamable.Samples.GPW.Data
{
   /// <summary>
   /// Server-side data: Represents a commodity for buy/sell
   /// </summary>
   [Serializable]
   public class ProductData 
   {
      //  Fields ---------------------------------------
      
      //At runtime, Store this value from the ProductContent
      [HideInInspector]
      public string Id = "";
      
      //At runtime, Store this value from the ProductContent
      //Using type string since apparently type AssetReferenceSprite does not serialize
      [HideInInspector]
      public string IconAssetGUID = "";

      public AssetReferenceSprite IconAssetReferenceSprite
      {
         get
         {
            if (string.IsNullOrEmpty(IconAssetGUID))
            {
               throw new Exception(("IconAssetGUID must be NOT IsNullOrEmpty"));
            }
            return new AssetReferenceSprite(IconAssetGUID);
         }
      }
      
      public string Title = "";
      public int PriceMin = 1;
      public int PriceMax = 10;
      
      public bool IsInitialized { private set; get; }

      /// <summary>
      /// Copy in values from ProductContent
      /// </summary>
      public void Initialize(string id, AssetReferenceSprite newIcon)
      {
         if (IsInitialized)
         {
            throw new Exception("Already Initialized");
         }

         IsInitialized = true;
         Id = id;
         IconAssetGUID = newIcon.AssetGUID;
      }

      public ProductData Clone()
      {
         ProductData productData = new ProductData
         {
            Id = Id,
            IconAssetGUID = IconAssetGUID,
            Title = Title,
            PriceMin = PriceMin,
            PriceMax = PriceMax
         };
         return productData;
      }
   }
}

