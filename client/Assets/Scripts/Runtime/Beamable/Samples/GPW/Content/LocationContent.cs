using System;
using Beamable.Common.Content;

namespace Beamable.Samples.GPW.Content
{
   [Serializable]
   public class LocationContentRef : ContentRef<LocationContent> {}
   
   /// <summary>
   /// Server-side data: Represents a geographic in-game location (e.g. North America)
   /// </summary>
   [ContentType("location")]
   public class LocationContent : ContentObject
   {
      //  Fields ---------------------------------------
      public string Title = "";
      public int RandomSeed = 1;
   }
}
