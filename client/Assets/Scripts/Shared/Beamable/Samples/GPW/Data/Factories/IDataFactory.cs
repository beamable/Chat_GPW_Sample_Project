using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Content;

namespace Beamable.Samples.GPW.Data.Factories
{
   /// <summary>
   /// Client-side factory for the loaded data content
   /// </summary>
   public interface IDataFactory 
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public Task<List<LocationContentView>> CreateLocationContentViews(
         List<LocationData> locationDatas, List<ProductData> productDatas);
   }
}
