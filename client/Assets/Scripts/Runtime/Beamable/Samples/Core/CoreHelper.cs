using System;

namespace Beamable.Samples.Core
{
   /// <summary>
   /// Store commonly reused functionality for concerns: General
   /// </summary>
   public static class CoreHelper
   {
      /// <summary>
      /// Convert the <see cref="float"/> to a <see cref="string"/>
      /// with rounding like "10.1";
      /// </summary>
      public static string GetRoundedTime(float value)
      {
         return string.Format("{0:0.0}", value);
      }


      /// <summary>
      /// Convert the <see cref="double"/> to a whole number like an <see cref="int"/>.
      /// </summary>
      public static double GetRoundedScore(double score)
      {
         return (int)score;
      }


      /// <summary>
      /// Convert the <see cref="string"/> to a whole number like an <see cref="int"/>.
      /// </summary>
      public static double GetRoundedScore(string score)
      {
         return GetRoundedScore(Double.Parse(score));
      }
   }
}