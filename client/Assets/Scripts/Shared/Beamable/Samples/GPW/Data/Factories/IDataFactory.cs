using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beamable.Samples.GPW.Data.Factories
{
   /// <summary>
   /// Client-side factory for the loaded data content
   /// </summary>
   public interface IDataFactory 
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public Task<List<LocationContentView>> GetLocationContentViews(
         List<LocationData> locationDatas, List<ProductData> productDatas);
   }
}
