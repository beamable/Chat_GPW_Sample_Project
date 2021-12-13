using System;
using UnityEngine;

namespace Beamable.Samples.GPW.Data
{
   /// <summary>
   /// Server-side data: Represents a geographic in-game location (e.g. North America)
   /// </summary>
   [Serializable]
   public class LocationData 
   {
      //  Fields ---------------------------------------
      [SerializeField]
      public string Title = "";
      [SerializeField]
      public int RandomSeed = 1;
   }
}
