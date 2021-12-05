using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.GPW.Data.Factories
{
   /// <summary>
   /// Client-side factory for the loaded data content
   /// </summary>
   public interface IDataFactory 
   {
      //  Fields ---------------------------------------
      
      //  Other Methods -----------------------------------
      public Task<List<LocationContentView>> CreateLocationContentView(
         List<LocationContentRef> locationContentRefs, List<ProductContent> productContents);
   }
}
