using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beamable.Api.Auth;
using Beamable.Api.Leaderboard;
using Beamable.Api.Stats;
using Beamable.Common.Api;
using Beamable.Common.Api.Leaderboards;
using Beamable.Common.Leaderboards;
using Beamable.Samples.GPW;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.Core.Data
{
   /// <summary>
   /// Create mock data. This is appropriate for a sample project, but not for
   /// production.
   /// </summary>
   public static class MockDataCreator
   {
      //  Other Methods --------------------------------

      /// <summary>
      /// Because this game is NOT a production game with real users, it is helpful
      /// to populate the Leaderboard with some mock users scores for
      /// cosmetic reasons
      /// </summary>
      /// <param name="beamableAPI"></param>
      /// <param name="leaderboardContent"></param>
      /// <param name="leaderboardRowCountMin"></param>
      /// <param name="leaderboardScoreMin"></param>
      /// <param name="leaderboardScoreMax"></param>
      public static async void PopulateLeaderboardWithMockData(IBeamableAPI beamableAPI,
         LeaderboardContent leaderboardContent, 
         int leaderboardRowCountMin, int leaderboardScoreMin, int leaderboardScoreMax)
      {
         LeaderboardService leaderboardService = beamableAPI.LeaderboardService;
         StatsService statsService = beamableAPI.StatsService;
         IAuthService authService = beamableAPI.AuthService;

         // Capture current user
         var localDbid = beamableAPI.User.id;

         // Check Leaderboard
         LeaderBoardView leaderboardView = await leaderboardService.GetBoard(leaderboardContent.Id, 0, 100);

         // Not enough data in the leaderboard? Create users with mock scores
         int currentRowCount = leaderboardView.rankings.Count;
         int targetRowCount = leaderboardRowCountMin;

         StringBuilder stringBuilder = new StringBuilder();
         stringBuilder.AppendLine($"PopulateLeaderboardWithMockData() ...");
         stringBuilder.AppendLine();
         stringBuilder.AppendLine($"* Before, rowCount={currentRowCount}");

         if (currentRowCount < targetRowCount)
         {
            int itemsToCreate = targetRowCount - currentRowCount;
            for (int i = 0; i < itemsToCreate; i++)
            {
               // Create NEW user
               // Login as NEW user (Required before using "SetScore")
               await authService.CreateUser().FlatMap(beamableAPI.ApplyToken);

               // Rename NEW user
               string alias = CreateNewRandomAlias("User");
               SetCurrentUserAlias(statsService, alias);
           
               // Submit mock score for NEW user
               double mockScore = UnityEngine.Random.Range(leaderboardScoreMin, leaderboardScoreMax);
               mockScore = GPWHelper.GetRoundedScore(mockScore);
               await leaderboardService.SetScore(leaderboardContent.Id, mockScore);

               stringBuilder.AppendLine($"* During, Created Mock User. Alias={alias}, score:{mockScore}");

            }
         }

         LeaderBoardView leaderboardViewAfter = await leaderboardService.GetBoard(leaderboardContent.Id, 0, 100);
         int currentRowCountAfter = leaderboardViewAfter.rankings.Count;
         stringBuilder.AppendLine($"* After, rowCount={currentRowCountAfter}");
         stringBuilder.AppendLine().AppendLine();
         
         Debug.Log(stringBuilder.ToString());

         // Login again as local user
         var deviceUsers = await beamableAPI.GetDeviceUsers();
         var user = deviceUsers.First(bundle => bundle.User.id == localDbid);
         await beamableAPI.ApplyToken(user.Token);
      }

      /// <summary>
      /// The the user alias which is visible in the Leaderboard Scene
      /// </summary>
      /// <param name="statsService"></param>
      /// <param name="alias"></param>
      public static async Task<EmptyResponse> SetCurrentUserAlias(StatsService statsService, string alias)
      {
         await statsService.SetStats("public", new Dictionary<string, string>()
            {
               { "alias", alias },
            });

         return new EmptyResponse();
      }
      
      public static async Task<string> GetCurrentUserAlias(StatsService statsService, long dbid)
      {
         var stats = await statsService.GetStats("client", "public", "player", dbid );
         
         string alias = CreateNewRandomAlias("User");
         stats.TryGetValue("alias", out alias);
         return alias;
      }



      /// <summary>
      /// Inspired by http://developer.qbapi.com/Generate-a-Random-Username.aspx
      /// </summary>
      private static string CreateNewRandomAlias(string prependName)
      {
         string alias = prependName;

         char[] lowers = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
         char[] uppers = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
         char[] numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

         int l = lowers.Length;
         int u = uppers.Length;
         int n = numbers.Length;

         Random random = new Random();
         alias += "_";
         //
         alias += lowers[random.Next(0, l)].ToString();
         //
         alias += uppers[random.Next(0, u)].ToString();
         //
         alias += "_";
         //
         alias += numbers[random.Next(0, n)].ToString();
         alias += numbers[random.Next(0, n)].ToString();
         alias += numbers[random.Next(0, n)].ToString();

         return alias;
      }
   }
}