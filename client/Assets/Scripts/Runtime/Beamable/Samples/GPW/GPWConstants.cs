using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Store commonly used static values
   /// </summary>
   public static class GPWConstants
   {

      //  Fields ---------------------------------------

      /// <summary>
      /// Determines if using Unity debug log statements.
      /// </summary>
      //TODO: Remove and use logger?
      public static bool IsDebugLogging = true;

      /// <summary>
      /// Used as a 'null' value.
      /// </summary>
      public const int UnsetValue = -1;

      // Chat
      public static string ChatRoomNameGlobal = "Global";
      private static string ChatRoomNameLocation = "Location";
      private static string ChatRoomNameDirect = "Direct";
      private static string ChatRoomNameSeparator = "_";
      public static string GetChatRoomNameLocation(LocationContent locationContent)
      {
         return ChatRoomNameLocation +
                ChatRoomNameSeparator +
                locationContent.Title;
      }
      
      public static string GetChatRoomNameDirect()
      {
         //TODO: pass in the 2 playerids that are chatting?
         long dbid01 = 01;
         long dbid02 = 01;
         return ChatRoomNameDirect +
                ChatRoomNameSeparator +
                dbid01 +
                ChatRoomNameSeparator +
                dbid02;
      }

   }
}