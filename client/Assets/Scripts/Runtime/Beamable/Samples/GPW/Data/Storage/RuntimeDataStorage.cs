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
        public RuntimeData RuntimeData
        {
            get { return _runtimeData; }
        }

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

                var beamContext = BeamContext.Default;
                await beamContext.OnReady;

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
            foreach (var productContentRef in _runtimeData.RemoteConfiguration.ProductContentRefs)
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
            foreach (var locationContentRef in _runtimeData.RemoteConfiguration.LocationContentRefs)
            {
                LocationContent locationContent = await locationContentRef.Resolve();

                // Clone here for several reasons: Including to avoid dirtying the ContentManager
                LocationData locationData = locationContent.LocationData.Clone();
                locationdatas.Add(locationData);
            }

            // Get Data            
            List<LocationContentView> freshLocationContentViews = await _dataFactory.GetLocationContentViews(
                locationdatas, productDatas);

            // Check Data
            ValidateLocationContentViews(freshLocationContentViews);

            // Store Data for Runtime usage
            _runtimeData.LocationContentViews = freshLocationContentViews;
        }

        private void ValidateLocationContentViews(List<LocationContentView> locationContentViews)
        {
            Assert.IsNotNull(locationContentViews);
            Assert.AreNotEqual(locationContentViews.Count, 0);

            foreach (var lcv in locationContentViews)
            {
                Assert.AreNotEqual(lcv.LocationData.Title, "");
                Assert.AreNotEqual(lcv.LocationData.RandomSeed, 0);
                Assert.IsNotNull(lcv.ProductContentViewCollection, "");
                Assert.AreNotEqual(lcv.ProductContentViewCollection.ProductContentViews.Count, 0);
                foreach (var productContentView in lcv.ProductContentViewCollection.ProductContentViews)
                {
                    Assert.AreNotEqual(productContentView.ProductData.Title, "");
                    Assert.AreNotEqual(productContentView.ProductData.IconAssetGUID, "");
                    Assert.AreNotEqual(productContentView.MarketGoods.Price, 0);
                }
            }
        }
    }
}