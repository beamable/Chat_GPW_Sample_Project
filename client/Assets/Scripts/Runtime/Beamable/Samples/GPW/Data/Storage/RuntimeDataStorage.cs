using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Content;
using Beamable.Samples.GPW.Data.Factories;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace Beamable.Samples.GPW.Data.Storage
{
    public enum ChatMode
    {
        None,
        Global,
        Location,
        Direct
    }
    
    public class RuntimeData
    {
        //  Properties  ----------------------------------
        public int ItemsCurrent
        {
            get
            {
                int itemsCurrent = 0;
                if (InventoryView != null)
                {
                    foreach (KeyValuePair<string, List<ItemView>> kvp in InventoryView.items)
                    {
                        itemsCurrent += kvp.Value.Count;
                    }
                }
                return itemsCurrent;
            } 
        }
        
        public int ItemsMax
        {
            get
            {  
                if (RemoteConfiguration == null)
                {
                    return 0;
                }
                return RemoteConfiguration.ItemsMax;
            }
        }

        //  Fields  --------------------------------------
        public InventoryView InventoryView = null;
        public List<LocationContentView> LocationContentViews = new List<LocationContentView>();
        public RemoteConfiguration RemoteConfiguration = null;
        public ChatMode ChatMode = ChatMode.None;
        public float BankInterestCurrent;
        public float DebtInterestCurrent;
        public int CashTransactionMin;
        public string PreviousSceneName = "";
    }
        
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class RuntimeDataStorage : SubStorage<RuntimeDataStorage>
    {
        //  Properties  ----------------------------------
        public RuntimeData RuntimeData { get { return _runtimeData; } }
  
        //  Fields  --------------------------------------
        private RuntimeData _runtimeData = new RuntimeData();
        private IDataFactory _dataFactory = null;
        
        //  Unity Methods  --------------------------------

        //  Other Methods  --------------------------------
        public override async Task Initialize(Configuration configuration, IDataFactory dataFactory)
        {
            if (!IsInitialized)
            {

                _dataFactory = dataFactory;
                Assert.IsNotNull(_dataFactory, "_dataFactory must exist. Set via Configuration via inspector.");
                
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                
                _runtimeData.RemoteConfiguration = await configuration.RemoteConfigurationRef.Resolve();
                _runtimeData.LocationContentViews.Clear();
                
                ///////////////////////
                // FACTORY: Populate Locations, each with products
                ///////////////////////
                await ResetGameDataViaDataFactory();
   
                ///////////////////////
                // Money
                ///////////////////////
                // WHY RANDOM SEED? So the users get a DETERMINISTIC experience. This may
                // or may not be needed for these specific values - srivello
                Random random = new System.Random(_runtimeData.RemoteConfiguration.RandomSeed);
                double BankInterestCurrent = _runtimeData.RemoteConfiguration.BankInterestMin +
                                            random.NextDouble() * _runtimeData.RemoteConfiguration.BankInterestMax;
                double DebtInterestCurrent = _runtimeData.RemoteConfiguration.DebtInterestMin +
                                             random.NextDouble() * _runtimeData.RemoteConfiguration.DebtInterestMax;

                _runtimeData.BankInterestCurrent = (float)Math.Round(BankInterestCurrent, 2);
                _runtimeData.DebtInterestCurrent = (float)Math.Round(DebtInterestCurrent, 2);
                _runtimeData.CashTransactionMin = _runtimeData.RemoteConfiguration.CashTransactionMin;
                
                ForceRefresh();
                IsInitialized = true;
            }
        }

        
          /// <summary>
          /// FACTORY: Populate Locations, each with products
          /// </summary>
        public async Task ResetGameDataViaDataFactory()
        {
            ///////////////////////
            // Get ProductData
            ///////////////////////
            List<ProductData> productDatas = new List<ProductData>();
            foreach (var productContentRef in  _runtimeData.RemoteConfiguration.ProductContentRefs)
            {
                ProductContent productContent = await productContentRef.Resolve();
                
                // Clone here for several reasons: Including to avoid dirtying the ContentManager
                ProductData productData = productContent.ProductData.Clone();
                productData.Initialize(productContent.Id, productContent.icon);
                productDatas.Add(productData);
            }
                
            //  Sort the product list from a to z
            productDatas.Sort((p1, p2) =>
            {
                return string.Compare(p2.Title, p2.Title, 
                    StringComparison.InvariantCulture);
            });
            
            ///////////////////////
            // Get LocationData
            ///////////////////////
            List<LocationData> locationdatas = new List<LocationData>();
            foreach (var locationContentRef in  _runtimeData.RemoteConfiguration.LocationContentRefs)
            {
                LocationContent locationContent = await locationContentRef.Resolve();
                
                // Clone here for several reasons: Including to avoid dirtying the ContentManager
                LocationData locationData = locationContent.LocationData.Clone();
                locationdatas.Add(locationData);
            }
            
            _runtimeData.LocationContentViews = await _dataFactory.GetLocationContentViews (
                locationdatas, productDatas);
            
            Debug.Log($"GetLocationContentViews() success. LocationContentViews.Count = " +
                      $"{_runtimeData.LocationContentViews.Count}");

            foreach (var locationContentView in _runtimeData.LocationContentViews)
            {
                Debug.Log("\tlocationContentView: " + locationContentView.LocationData.Title);
                Debug.Log("\t count: " + locationContentView.ProductContentViewCollection.ProductContentViews.Count);
                foreach (var productContentView in locationContentView.ProductContentViewCollection.ProductContentViews)
                {
                    Debug.Log("\t\tproductContentView t: " + productContentView.ProductData.Title);
                    Debug.Log("\t\tproductContentView p : " + productContentView.MarketGoods.Price);
                    Debug.Log("\t\tproductContentView q: " + productContentView.MarketGoods.Quantity);
                }
            }

        }
    }
}
