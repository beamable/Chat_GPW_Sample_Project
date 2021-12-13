using System;
using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Content
{
   
   [Serializable]
   public class LocationContentRef : ContentRef<LocationContent> {}

   /// <summary>
   /// Server-side data: Represents a geographic in-game location (e.g. North America)
   /// </summary>
   [ContentType("location")]
   [Serializable]
   public class LocationContent : ContentObject
   {
      //  Fields ---------------------------------------
      [SerializeField] 
      public LocationData LocationData = new LocationData();
   }
}
