using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.Core.Components;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;
using UnityEngine;

namespace Beamable.Samples.GPW
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class GameController : SingletonMonobehavior<GameController>
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
        
        public void UpdateLocationTo()
        {
            var current = PersistentDataStorage.PersistentData.LocationContentViewCurrent;
            var list = RuntimeDataStorage.RuntimeData.LocationContentViews;
            int currentIndex = 0;
            foreach (var x in list)
            {
                if (x.LocationContent.Id == current.LocationContent.Id)
                {
                    break;
                }
                currentIndex++;
            }

            int nextIndex = currentIndex + 1;
            if (nextIndex > list.Count -1)
            {
                nextIndex = 0;
            }
         
            PersistentDataStorage.PersistentData.LocationContentViewCurrent =
                list[nextIndex];
            PersistentDataStorage.ForceRefresh();
        }
        
        public void UpdateBankTo()
        {
            int amountToAddToBank = 10;
            _persistentDataStorage.PersistentData.CashAmount -= amountToAddToBank;
            _persistentDataStorage.PersistentData.BankAmount += amountToAddToBank;
            _persistentDataStorage.ForceRefresh();
        }
      
        public void UpdateDebtTo()
        {
            int amountToAddToDebt = 10;
            _persistentDataStorage.PersistentData.CashAmount += amountToAddToDebt;
            _persistentDataStorage.PersistentData.DebitAmount += amountToAddToDebt;
            _persistentDataStorage.ForceRefresh();
        }
        
        
        //  Event Handlers  -------------------------------
        private void InventoryService_OnChanged(InventoryView inventoryView)
        {
            Debug.Log("InventoryService_OnChanged() : " + 
                      inventoryView.items.Count);
            
            _runtimeDataStorage.RuntimeData.InventoryView = inventoryView;
            _runtimeDataStorage.ForceRefresh();
        }
        
        
        private void PersistentDataStorage_OnChanged(SubStorage subStorage)
        {
            PersistentDataStorage persistentDataStorage = subStorage as PersistentDataStorage;
         
            Debug.Log("PersistentDataStorage_OnChanged() : " + 
                      persistentDataStorage.PersistentData.LocationContentViewCurrent.LocationContent.Title);
        }
      
      
        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            Debug.Log("RuntimeDataStorage_OnChanged() : -------");
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
            InventoryView inventoryView = runtimeDataStorage.RuntimeData.InventoryView;

            if (_persistentDataStorage.PersistentData.LocationContentViewCurrent == null)
            {
                _persistentDataStorage.PersistentData.LocationContentViewCurrent = 
                    runtimeDataStorage.RuntimeData.LocationContentViews[0];
            }
        }
    }
}
