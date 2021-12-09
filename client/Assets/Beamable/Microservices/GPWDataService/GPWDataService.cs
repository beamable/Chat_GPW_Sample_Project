using Beamable.Server;

namespace Beamable.Server
{
   [Microservice("GPWDataService")]
   public class GPWDataService : Microservice
   {
      [ClientCallable]
      public void ServerCall()
      {
         // This code executes on the server.
      }
   }
}