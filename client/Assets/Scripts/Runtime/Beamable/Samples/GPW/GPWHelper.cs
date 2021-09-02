using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Beamable.Samples.Core.Audio;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.Utilities;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.UI.Scripts;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Beamable.Samples.GPW
{
   /// <summary>
   /// Store commonly reused functionality for concerns: General
   /// </summary>
   public static class GPWHelper
   {
      
      public const int UnsetValue = -1;
      
      // Chat
      public static string ChatRoomNameGlobal = "Global";
      private static string ChatRoomNameLocation = "Location";
      private static string ChatRoomNameDirect = "Direct";
      private static string ChatRoomNameSeparator = "_";
      public static string DefaultLocalAlias = "This is you! :)";
      public static string DefaultRemoteAliasPrefix = "Player";
      public static string Intro = "Intro";
      public static string Reset = "Reset";
      public static string Game = "Game";
      public static string Ok = "Ok";
      public static string Cancel = "Cancel";
      public static string SubmitScore = "Submit Score";
      public static string Quit = "Cancel";
      
      public static string GetChatRoomNameGlobal()
      {
         return ChatRoomNameGlobal;
      }
      
      public static string GetChatRoomNameLocation(LocationContent locationContent)
      {
         return ChatRoomNameLocation +
                ChatRoomNameSeparator +
                locationContent.Title;
      }
      
      public static string GetChatRoomNameDirect()
      {
         // TODO: Optional, create a CUSTOM room for each two dbid's of the players
         // in the room. This is only needed if multiple direct rooms are offered in the future.
         // For now its not needed.
         long dbid01 = 01;
         long dbid02 = 01;
         return ChatRoomNameDirect +
                ChatRoomNameSeparator +
                dbid01 +
                ChatRoomNameSeparator + "On" + ChatRoomNameSeparator +
                dbid02;
      }
      
      public static string GetChatRoomTopic(ChatMode chatMode)
      {
         string topic = "";
         switch (chatMode)
         {
            case ChatMode.Global:
               topic = "Discuss global prices";
               break;
            case ChatMode.Location:
               topic = "Discuss local prices";
               break;
            case ChatMode.Direct:
               topic = "Discuss game strategies";
               break;
            default:
               SwitchDefaultException.Throw(chatMode);
               break;
         }

         return topic;
      }

      //  Other Methods ---------------------------   -----
      public static void PlayAudioClipPrimaryClick()
      {
         SoundManager.Instance.PlayAudioClip(SoundConstants.Click01);
      }
      
      public static void PlayAudioClipSecondaryClick()
      {
         SoundManager.Instance.PlayAudioClip(SoundConstants.Click02);
      }
      
      public static IEnumerator LoadScene_Coroutine(string sceneName, float delayBeforeLoading)
      {
         PlayAudioClipPrimaryClick();
         
         //Store previous scene. Useful for some 'back' buttons
         GPWController.Instance.RuntimeDataStorage.RuntimeData.PreviousSceneName = 
            SceneManager.GetActiveScene().name;
         
         Configuration.Debugger.Log($"LoadScene() to {sceneName}");

         yield return new WaitForSeconds(delayBeforeLoading/1000);
         SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
      }


      /// <summary>
      /// Return the intro menu text. This serves as a welcome to the game plot and game instructions.
      /// If error, help text is shown.
      /// </summary>
      public static string GetIntroAboutBodyText(bool isConnected,
         bool isBeamableSDKInstalled, 
         long dbid,
         string isBeamableSDKInstalledErrorMessage)
      {
         string text = "";

         // Is Beamable SDK Properly Installed In Unity?
         if (isBeamableSDKInstalled)
         {
            // Is Game Properly Connected To Internet?
            if (isConnected)
            {
               text += GPWHelper.GameInstructionsText;
               text += GPWHelper.GetBulletList("Status", new List<string>
               {
                  $"Connected: {true}",
                  $"DBID: {dbid}", 
               });
            }
            else
            {
               // Error
               text += GPWHelper.InternetOfflineInstructionsText;
            }
         }
         else
         {
            // Error
            text += $"_isBeamableSDKInstalled = {isBeamableSDKInstalled}." + "\n\n";
            text += $"_isBeamableSDKInstalledErrorMessage = {isBeamableSDKInstalledErrorMessage}" + "\n\n";
            text += GPWHelper.BeamableSDKInstallInstructionsText;
         }

         return text;
      }


      /// <summary>
      /// Return a random item from the list. 
      /// This provides cosmetic variety.
      /// </summary>
      public static string GetRandomString(List<string> items)
      {
         int index = UnityEngine.Random.Range(0, items.Count);
         return items[index];
      }


      public static float GetAudioPitchByGrowthPercentage(float growthPercentage)
      {
         //Range from 0.5 to 1.5
         return 0.5f + Mathf.Clamp01(growthPercentage);
      }


      public static string InternetOfflineInstructionsText
      {
         get
         {
            string text = "";
            text += "<color=#ff0000>You are currently offline.</color>" + "\n\n";
            text += "<align=left>";
            text += GPWHelper.GetBulletList("Suggestions\n", new List<string> {
               "Stop the Scene in the Unity Editor",
               "Connect to the internet",
               "Play the Scene in the Unity Editor",
               "Enjoy!"
            }); ;
            text += "</align>";
            return text;
         }
      }


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


      private static string GameInstructionsText
      {
         get
         {
            string text = "";

            text += "Buy low, sell high! Finish rich and be top on the Leaderboard." + "\n\n";

            text += "This sample project demonstrates Beamable's Chat feature.\n\n";
            
            text += GPWHelper.GetBulletList("Resources", new List<string>
            {
               "Overview: " + TMP_HyperlinkHandler.WrapTextWithLink("Chat (GPW) Sample", "https://docs.beamable.com/docs/chat-gpw-sample-project"),
               "Feature: " + TMP_HyperlinkHandler.WrapTextWithLink("Chat", "https://docs.beamable.com/docs/chat-feature")
            });

            return text;
         }
      }

      private static string BeamableSDKInstallInstructionsText
      {
         get
         {
            string text = "";
            text += "<color=#ff0000>";
            text += GPWHelper.GetBulletList("Todo", new List<string> {
               "Download & Install " + TMP_HyperlinkHandler.WrapTextWithLink("Beamable SDK", "http://docs.beamable.com"),
               "Open the Beamable Toolbox Window in Unity",
               "Register or Sign In"
            });
            text += "</color>";
            return text;
         }
      }




      private static string GetBulletList(string title, List<string> items)
      {
         string text = "";
         text += $"{title}" + "\n";
         text += "<indent=5%>";
         foreach (string item in items)
         {
            text += $"• {item}" + "\n";
         }
         text += "</indent>" + "\n";
         return text;
      }
      
     

      public static async void AddressablesLoadAssetAsync(AssetReferenceSprite assetReferenceSprite, Image destinationImage)
      {
         // Check before await
         if (destinationImage == null || assetReferenceSprite == null)
         {
            return;
         }

         // Hide it
         TweenHelper.ImageDoFade(destinationImage, 0, 0, 0, 0);

         Sprite sprite = await AddressableSpriteLoader.LoadSprite(assetReferenceSprite);

         // Check after await
         if (destinationImage == null || assetReferenceSprite == null)
         {
            return;
         }

         if (sprite != null)
         {
            destinationImage.sprite = sprite;

            // Show it
            TweenHelper.ImageDoFade(destinationImage, 0, 1, 0.25f, 0);
         }
      }

      public static void QuitSafe()
      {
         if (Application.isEditor)
         {
            //In the Unity Editor, mimic the user clicking 'Stop' to stop.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
         }
         else
         {
            //In the build, mimic the user clicking 'X' to quit.
            Application.Quit();
         }
      }

      public static void SetButtonText(Button button, string line1, string line2, int spaceBetweenLines = 10)
      {
         button.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"{line1}"+
            $"<size={spaceBetweenLines}>\n  \n</size>"+
            $"<size=20>{line2}</size>";
      }
      
      public static void SetButtonText(Button button, string line1)
      {
         button.GetComponentInChildren<TextMeshProUGUI>().text = $"{line1}";
      }

      private static string GetPluralized(string titleSingular, string titlePlural, int amountCurrent)
      {
         amountCurrent = Mathf.Abs(amountCurrent);
         
         if (amountCurrent == 1)
         {
            return titleSingular;
         }
         else
         {
            //The grammar looks good here for 0 amount too
            return titlePlural;
         }
      }

      /// <summary>
      /// Buy items?
      /// </summary>
      public static void ShowDialogBoxBuy(DialogSystem dialogSystem, ProductContentView productContentView, Action<int> onComplete)
      {
         int amount = 1;
         int deltaAmount = 1;
         string getBodyText(int amountCurrent)
         {
            return $"Buy {amountCurrent:000} {GetPluralized("item","items", amountCurrent)} for ${(amountCurrent * productContentView.MarketGoods.Price):000}?";
         }

         dialogSystem.ShowDialogBox<DialogUI>(
            dialogSystem.DialogUIPrefab,
            $"{productContentView.ProductContent.Title}: You own {productContentView.OwnedGoods.Quantity:000} at ${productContentView.OwnedGoods.Price:000}.",
            getBodyText(amount),
            new List<DialogButtonData>
            {
               new DialogButtonData($"+{deltaAmount}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  int nextAmount = amount + deltaAmount;
                  if (GPWController.Instance.CanBuyItem(productContentView, nextAmount))
                  {
                     amount = nextAmount;
                  }
                  dialogSystem.CurrentDialogUI.BodyText.text = getBodyText(amount);
               }),
               new DialogButtonData($"-{deltaAmount}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  amount -= deltaAmount;
                  amount = Mathf.Max(amount, 0);
                  dialogSystem.CurrentDialogUI.BodyText.text = getBodyText(amount);
               }),
               new DialogButtonData(GPWHelper.Ok, delegate
               {
                  onComplete.Invoke(amount);
               })
            });
      }
      
      /// <summary>
      /// Sell items?
      /// </summary>
      public static void ShowDialogBoxSell(DialogSystem dialogSystem, ProductContentView productContentView, Action<int> onComplete)
      {
         int amount = 1;
         int deltaAmount = 1;
         string getBodyText(int amountCurrent)
         {
            return $"Sell {amountCurrent:000} {GetPluralized("item","items", amountCurrent)} for ${(amountCurrent * productContentView.MarketGoods.Price):000}?";
         }

         dialogSystem.ShowDialogBox<DialogUI>(
            dialogSystem.DialogUIPrefab,
            $"{productContentView.ProductContent.Title}: You own {productContentView.OwnedGoods.Quantity:000} at ${productContentView.OwnedGoods.Price:000}.",
            getBodyText(amount),
            new List<DialogButtonData>
            {
               new DialogButtonData($"+{deltaAmount}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  int nextAmount = amount + deltaAmount;
                  if (GPWController.Instance.CanSellItem(productContentView, nextAmount))
                  {
                     amount = nextAmount;
                  }
                  dialogSystem.CurrentDialogUI.BodyText.text = getBodyText(amount);
               }),
               new DialogButtonData($"-{deltaAmount}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  amount -= deltaAmount;
                  amount = Mathf.Max(amount, 0);
                  dialogSystem.CurrentDialogUI.BodyText.text = getBodyText(amount);
               }),
               new DialogButtonData(GPWHelper.Ok, delegate
               {
                  onComplete.Invoke(amount);
               })
            });
      }


      /// <summary>
      /// Transfer from CASH → BANK?
      /// </summary>
      public static void ShowDialogBank(DialogSystem dialogSystem, int cashTransactionMin, Action<int> onTransferCashToBank)
      {
         dialogSystem.ShowDialogBox<DialogUI>(
            dialogSystem.DialogUIPrefab,
            $"Transfer from CASH → BANK?",
            "Positive BANK will help your final score.\n\n\nBANK increases by " +
            $"{GPWController.Instance.RuntimeDataStorage.RuntimeData.BankInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData($"+{cashTransactionMin}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  onTransferCashToBank.Invoke(cashTransactionMin);
               }),
               new DialogButtonData($"-{cashTransactionMin}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  onTransferCashToBank.Invoke(-cashTransactionMin);
               }),
               new DialogButtonData(GPWHelper.Ok, async delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  await dialogSystem.HideDialogBoxImmediate();
               })
            });
      }

      /// <summary>
      /// Transfer from CASH → DEBT?
      /// </summary>
      public static void ShowDialogDebt(DialogSystem dialogSystem, int cashTransactionMin, Action<int> onTransferCashToDebt)
      {
         dialogSystem.ShowDialogBox<DialogUI>(
            dialogSystem.DialogUIPrefab,
            "Transfer from CASH → DEBT?",
            $"Positive DEBT will hurt your final score.\n\n\nDEBT increases by " +
            $"{GPWController.Instance.RuntimeDataStorage.RuntimeData.DebtInterestCurrent}% every TURN.",
            new List<DialogButtonData>
            {
               new DialogButtonData($"+{cashTransactionMin}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  onTransferCashToDebt.Invoke(-cashTransactionMin);
               }),
               new DialogButtonData($"-{cashTransactionMin}", delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  onTransferCashToDebt.Invoke(cashTransactionMin);
                  
               }),
               new DialogButtonData(GPWHelper.Ok, async delegate
               {
                  GPWHelper.PlayAudioClipSecondaryClick();
                  await dialogSystem.HideDialogBoxImmediate();
               })
            });
      }
   }
}