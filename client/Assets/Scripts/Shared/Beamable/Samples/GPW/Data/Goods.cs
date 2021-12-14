   
using System;

namespace Beamable.Samples.GPW.Content
{
   /// <summary>
   /// Client-side wrapper for the loaded data content
   /// </summary>
   [Serializable]
   public class Goods
   {
      public int Price;
      public int Quantity;

      public override string ToString()
      {
         return $"[Goods (Price={Price}, Quantity={Quantity})";
      }
   }
}
