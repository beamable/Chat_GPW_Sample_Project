using System.Collections.Generic;
using AirFishLab.ScrollingList;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
using Beamable.Samples.GPW.UI.ScrollingList;
using Beamable.Samples.GPW.Views;
using UnityEngine;
using ProductContent = Beamable.Samples.GPW.Content.ProductContent;

namespace Beamable.Samples.GPW
{

   /// <summary>
   /// Handles the main scene logic: Game
   /// </summary>
   public class GameSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public GameUIView GameUIView { get { return _gameUIView; } }
      public Configuration Configuration { get { return _configuration; } }

      //  Fields ---------------------------------------
      private IBeamableAPI _beamableAPI = null;
      
      [SerializeField]
      private Configuration _configuration = null;
      
      [SerializeField]
      private GameUIView _gameUIView = null;
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.ChatButton.onClick.AddListener(ChatButton_OnClicked);
         _gameUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _gameUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         SetupBeamable();
      }
      

      //  Other Methods  -----------------------------
      private async void SetupBeamable()
      {
         _beamableAPI = await Beamable.API.Instance;
         
         if (!GPWSingleton.Instance.IsInitialized)
         {
            await GPWSingleton.Instance.Initialize(_configuration);
         }

         GPWSingleton.Instance.PersistentDataStorage.OnRefreshed.AddListener(PersistentStorage_OnChanged);
         GPWSingleton.Instance.PersistentDataStorage.ForceRefresh();

         Debug.Log("Products.Count: " + 
                   GPWSingleton.Instance.GameServices.ProductContents.Count);
         
         Debug.Log("Locations.Count: " + 
                   GPWSingleton.Instance.GameServices.LocationContents.Count);
         
         Debug.Log("LocationCurrent: " + 
                   GPWSingleton.Instance.GameServices.LocationCurrent.Title);

         List<ProductContent> list = GPWSingleton.Instance.GameServices.ProductContents;

         // Prepare list
         ProductContentListBank listBank = 
            _gameUIView.CircularScrollingList.gameObject.AddComponent<ProductContentListBank>();
         listBank.Initialize(list);
         
         // Render list
         _gameUIView.CircularScrollingList.ListBank = listBank;
         _gameUIView.CircularScrollingList.OnInitialized.AddListener(CircularScrollingList_OnInitialized);
         _gameUIView.CircularScrollingList.Initialize();
         
      }

      private void PersistentStorage_OnChanged(SubStorage subStorage)
      {
         PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         PersistentData persistentData = persistentDataStorage.PersistentData;
        _gameUIView.PersistentData = persistentData;
      }


      /// <summary>
      /// Render UI text
      /// </summary>
      /// <param name="message"></param>
      /// <param name="statusTextMode"></param>
      public void SetStatusText(string message, TMP_BufferedText.BufferedTextMode statusTextMode)
      {
         _gameUIView.BufferedText.SetText(message, statusTextMode);
      }

      //  Event Handlers -------------------------------
      private void ChatButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.ChatSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void LeaderboardButton_OnClicked()
      {
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.LeaderboardSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void QuitButton_OnClicked()
      {
         Debug.Log("as are you sure? This blanks progress");
         StartCoroutine(GPWHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void CircularScrollingList_OnInitialized(CircularScrollingList circularScrollingList)
      {
         foreach (ListBox listBox in circularScrollingList.ListBoxes)
         {
            ProductContentListItem productContentListItem = listBox as ProductContentListItem;
            productContentListItem.OnBuy.RemoveAllListeners();
            productContentListItem.OnBuy.AddListener(ProductContentListItem_OnBuy);
            productContentListItem.OnSell.RemoveAllListeners();
            productContentListItem.OnSell.AddListener(ProductContentListItem_OnSell);
         }
      }

      private async void ProductContentListItem_OnBuy(ProductContent productContent)
      {
         Debug.Log("Buy: " + productContent.Title);

         var canBuyItem = await GPWSingleton.Instance.CanBuyItem(productContent.Id, 1);
         bool isSuccessful = false;
         if (canBuyItem)
         {
            isSuccessful = await GPWSingleton.Instance.BuyItem(productContent.Id, 1);
         }
         
         Debug.Log("isSuccessful: " + isSuccessful);
      }
      private async void ProductContentListItem_OnSell(ProductContent productContent)
      {
         Debug.Log("Sell: " + productContent.Title);

         bool canSellItem = await GPWSingleton.Instance.CanSellItem(productContent.Id, 1);
         bool isSuccessful = false;
         if (canSellItem)
         {
            isSuccessful = await GPWSingleton.Instance.SellItem(productContent.Id, 1);
         }
         
         Debug.Log("isSuccessful: " + isSuccessful);
      }
   }
}