using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.Components;
using Beamable.Samples.Core.UI.ScrollingList;
using Beamable.Samples.GPW.Content;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Storage
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class GPWSingleton : SingletonMonobehavior<GPWSingleton>
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; } protected set { _isInitialized = value; } }
        public RuntimeDataStorage RuntimeDataStorage { get { return _runtimeDataStorage; } }
        public PersistentDataStorage PersistentDataStorage { get { return _persistentDataStorage; } }
        public GameServices GameServices { get { return _gameServices; } }
        
        //  Fields  --------------------------------------
        
        private RuntimeDataStorage _runtimeDataStorage = new RuntimeDataStorage();
        private PersistentDataStorage _persistentDataStorage = new PersistentDataStorage();
        private GameServices _gameServices = new GameServices();
        private bool _isInitialized = false;
        

        //  Unity Methods  --------------------------------

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration)
        {
            if (!IsInitialized)
            {
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                
                // GameServices
                _gameServices.OnInventoryViewChanged.AddListener(InventoryService_OnChanged);
                _runtimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
                _persistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
                _gameServices.OnInventoryViewChanged.AddListener(InventoryService_OnChanged);
                await _gameServices.Initialize(configuration);
                await _runtimeDataStorage.Initialize(configuration);
                await _persistentDataStorage.Initialize(configuration);

                // Money
                _persistentDataStorage.PersistentData.BankAmount = 
                    _runtimeDataStorage.RuntimeData.RemoteConfiguration.BankAmountInitial;
                
                _persistentDataStorage.PersistentData.CashAmount = 
                    _runtimeDataStorage.RuntimeData.RemoteConfiguration.CashAmountInitial;
                
                _persistentDataStorage.PersistentData.DebitAmount = 
                    _runtimeDataStorage.RuntimeData.RemoteConfiguration.DebtAmountInitial;

                // Turns
                _persistentDataStorage.PersistentData.TurnCurrent = 1;
                _persistentDataStorage.PersistentData.TurnsTotal = 
                    _runtimeDataStorage.RuntimeData.RemoteConfiguration.TurnsTotal;
                
                IsInitialized = true;
                _persistentDataStorage.OnChanged.Invoke(_persistentDataStorage);
            }
        }
        
        
        //  Event Handlers  -------------------------------
        private void InventoryService_OnChanged(InventoryView inventoryView)
        {
            _runtimeDataStorage.RuntimeData.InventoryView = inventoryView;
            _runtimeDataStorage.ForceRefresh();
        }
        
        
        private void PersistentDataStorage_OnChanged(SubStorage subStorage)
        {
            PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         
            Debug.Log("LocationCurrent: " + 
                      persistentDataStorage.PersistentData.LocationCurrent.Title);
        }
      
      
        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            Debug.Log("RuntimeDataStorage_OnChanged: -------");
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
            InventoryView inventoryView = runtimeDataStorage.RuntimeData.InventoryView;
         
            _persistentDataStorage.PersistentData.LocationCurrent = 
                runtimeDataStorage.RuntimeData.LocationContents[0];
        }
    }
}
