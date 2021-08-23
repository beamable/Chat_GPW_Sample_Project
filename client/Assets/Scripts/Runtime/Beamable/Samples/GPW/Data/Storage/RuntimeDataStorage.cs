﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.GPW.Data.Storage
{
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
    }
        
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class RuntimeDataStorage : SubStorage<RuntimeDataStorage>
    {
        //  Properties  ----------------------------------
        public RuntimeData RuntimeData { get { return _runtimeData; ForceRefresh(); } }

        //  Fields  --------------------------------------
        private IBeamableAPI _beamableAPI = null;
        private RuntimeData _runtimeData = new RuntimeData();
        
        //  Unity Methods  --------------------------------

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration)
        {
            if (!IsInitialized)
            {
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                
                _runtimeData.RemoteConfiguration = await configuration.RemoteConfigurationRef.Resolve();
                _runtimeData.LocationContentViews.Clear();
                
                // Get products
                List<ProductContent> productContents = new List<ProductContent>();
                foreach (var productContentRef in  _runtimeData.RemoteConfiguration.ProductContentRefs)
                {
                    ProductContent productContent = await productContentRef.Resolve();
                    productContents.Add(productContent);
                }
                
                // Loop through locations, add a copy of the products to each
                foreach (var locationContentRef in  _runtimeData.RemoteConfiguration.LocationContentRefs)
                {
                    LocationContent locationContent = await locationContentRef.Resolve();
                    
                    // Populate with new, generated client-side data
                    LocationContentView locationContentView =
                        new LocationContentView(locationContent, productContents);
                    
                    _runtimeData.LocationContentViews.Add(locationContentView);
                }

                ForceRefresh();
                IsInitialized = true;
            }
        }
    }
}
