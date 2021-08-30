﻿using System;
using Beamable.Common.Leaderboards;
using Beamable.Samples.GPW.Content;
using UnityEngine;

namespace Beamable.Samples.GPW.Data
{
   /// <summary>
   /// Store the common configuration for easy editing ats
   /// EditTime and RuntTime with the Unity Inspector Window.
   /// </summary>
   [CreateAssetMenu(
      fileName = Title,
      menuName = BeamableConstants.MENU_ITEM_PATH_ASSETS_BEAMABLE_SAMPLES + "/" +
      "Multiplayer/Create New " + Title,
      order = BeamableConstants.MENU_ITEM_PATH_ASSETS_BEAMABLE_ORDER_1)]
   public class Configuration : ScriptableObject
   {
      //  Constants  -----------------------------------
      private const string Title = "Configuration";

      //  Properties -----------------------------------
      
      //TODO: Remove this and add logging?
      public bool IsDemoMode { get { return _isDemoMode; } }
      
      public string Scene01IntroName { get { return _scene01IntroName; } }

      public string Scene02GameName { get { return _scene02GameName; } }

      public string Scene03ChatName { get { return _scene03ChatName; } }
      public string Scene04LeaderboardName { get { return _scene04LeaderboardName; } }
      
      public float DelayBeforeLoadScene { get { return _delayBeforeLoadScene; } }
      public float DelayAfterDataLoading { get { return _delayAfterDataLoading; } }
      public float DelayFadeInUI { get { return _delayFadeInUI; } }

      public RemoteConfigurationRef RemoteConfigurationRef { get { return _remoteConfigurationRef; } }
      
      public LeaderboardRef LeaderboardRef { get { return _leaderboardRef; } }
      public int LeaderboardRowCountMin { get { return _leaderboardRowCountMin; } }
      public int LeaderboardScoreMin { get { return _leaderboardScoreMin; } }
      public int LeaderboardScoreMax { get { return _leaderboardScoreMax; } }
      public string DialogBoxLoadingText { get { return _dialogBoxLoadingText; } }


      //  Fields ---------------------------------------
      
      /// <summary>
      /// Determines if we are demo mode. Demo mode does several operations
      /// which are not recommended in a production project including 
      /// creating mock data for the game.
      /// </summary>
      [Header("Debug")]
      [SerializeField]
      private bool _isDemoMode = true;
      
      [Header("Scene Names")]
      [SerializeField]
      private string _scene01IntroName = "";

      [SerializeField]
      private string _scene02GameName = "";

      [SerializeField]
      private string _scene03ChatName = "";
      
      [SerializeField]
      private string _scene04LeaderboardName = "";


      [Header("Content")] 
      [SerializeField]
      private RemoteConfigurationRef _remoteConfigurationRef = null;
      
      [SerializeField]
      private LeaderboardRef _leaderboardRef = null;

      [Header("Cosmetics")]
      [SerializeField]
      private float _delayBeforeLoadScene = 0.25f;
      
      [SerializeField]
      private float _delayAfterDataLoading = 0.25f;
         
      [SerializeField]
      private float _delayFadeInUI = 0.25f;
      
      [SerializeField]
      private string _dialogBoxLoadingText = "Loading {0}...";

      
      [Header("Leaderboard (Mock Data)")] 
      [SerializeField]
      private int _leaderboardRowCountMin = 10;
      
      [SerializeField]
      private int _leaderboardScoreMin = 100;
      
      [SerializeField]
      private int _leaderboardScoreMax = 1000;

      
      //  Unity Methods ---------------------------------------
      protected void OnValidate()
      {
         // Example validation, remove as needed
         _delayFadeInUI = Mathf.Max(_delayFadeInUI, 0.25f);
      }
   }
}
