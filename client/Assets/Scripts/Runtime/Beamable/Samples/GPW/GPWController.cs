using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Experimental.Api.Chat;
using Beamable.Samples.Core.Components;
using Beamable.Samples.Core.Exceptions;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Storage;

namespace Beamable.Samples.GPW
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class GPWController : SingletonMonobehavior<GPWController>
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
        private Configuration _configuration = null;
        private bool _isInitialized = false;

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration2)
        {
            if (!IsInitialized)
            {
                /////////////////////////////
                // GameServices
                /////////////////////////////
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                _configuration = configuration2;
                _gameServices.OnInventoryViewChanged.AddListener(InventoryService_OnChanged);
                _runtimeDataStorage.OnChanged.AddListener(RuntimeDataStorage_OnChanged);
                _persistentDataStorage.OnChanged.AddListener(PersistentDataStorage_OnChanged);
                await ResetGame();
                IsInitialized = true;
                
                

            }
        }
        
        public void GoToLocation()
        {
            if (_persistentDataStorage.PersistentData.IsGameOver)
            {
                return;
            }
            
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

            // Progress Turn / Bank / Debt
            GoToNextTurn();
            
            // Change Location
            PersistentDataStorage.PersistentData.LocationContentViewCurrent =
                list[nextIndex];
            PersistentDataStorage.ForceRefresh();
        }

        private void GoToNextTurn()
        {
            if (_persistentDataStorage.PersistentData.IsGameOver)
            {
                return;
            }

            // Calculate interest - BankAmount 
            float bankAmount = PersistentDataStorage.PersistentData.BankAmount +
                (PersistentDataStorage.PersistentData.BankAmount * RuntimeDataStorage.RuntimeData.BankInterestCurrent);
            PersistentDataStorage.PersistentData.BankAmount = (int)bankAmount;
            
            // Calculate interest - BankAmount 
            float debtAmount = PersistentDataStorage.PersistentData.DebitAmount +
                               (PersistentDataStorage.PersistentData.DebitAmount * RuntimeDataStorage.RuntimeData.DebtInterestCurrent);
            PersistentDataStorage.PersistentData.DebitAmount = (int)debtAmount;
            
            // Advance the turn counter
            PersistentDataStorage.PersistentData.TurnCurrent++;

        }
        
        public void TransferCashToBank(int amountToAddToBank)
        {
            // The amount may be positive or negative
            // Check that both balances will be above zero
            if (_persistentDataStorage.PersistentData.CashAmount - amountToAddToBank >= 0 && 
                _persistentDataStorage.PersistentData.BankAmount + amountToAddToBank >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount -= amountToAddToBank;
                _persistentDataStorage.PersistentData.BankAmount += amountToAddToBank;
                _persistentDataStorage.ForceRefresh();
            }
        }
      
        public void TransferCashToDebt(int amountToPayoffDebt)
        {
            // The amount may be positive or negative
            // Check that both balances will be above zeros
            if (_persistentDataStorage.PersistentData.CashAmount - amountToPayoffDebt >= 0 && 
                _persistentDataStorage.PersistentData.DebitAmount + amountToPayoffDebt >= 0)
            {
                _persistentDataStorage.PersistentData.CashAmount += amountToPayoffDebt;
                _persistentDataStorage.PersistentData.DebitAmount += amountToPayoffDebt;
                _persistentDataStorage.ForceRefresh();
            }
        }
        
        public double CalculatedCurrentScore()
        {
            int cashAmount = GPWController.Instance.PersistentDataStorage.PersistentData.CashAmount;
            int bankAmount = GPWController.Instance.PersistentDataStorage.PersistentData.BankAmount;
            int debtAmount = GPWController.Instance.PersistentDataStorage.PersistentData.DebitAmount;

            return cashAmount + bankAmount - debtAmount;
        }
        
        public bool HasCurrentRoomHandle
        {
            get
            {
                return GetCurrentRoomHandle() != null;
            }
            
        }
        
        public RoomHandle GetCurrentRoomHandle()
        {
            string currentRoomName = "";
			
            switch (_runtimeDataStorage.RuntimeData.ChatMode)
            {
                case ChatMode.Global:
                    currentRoomName = GPWConstants.ChatRoomNameGlobal;
                    break;
                case ChatMode.Location:
                    LocationContent locationContent = _persistentDataStorage.
                        PersistentData.LocationContentViewCurrent.LocationContent;
                    currentRoomName = GPWConstants.GetChatRoomNameLocation(locationContent);
                    break;
                case ChatMode.Direct:
                    currentRoomName = GPWConstants.GetChatRoomNameDirect();
                    break;
                    SwitchDefaultException.Throw(_runtimeDataStorage.RuntimeData.ChatMode);
                    break;
            }

            if (!_gameServices.HasRoom(currentRoomName))
            {
                return null;
            }
            
            return _gameServices.GetRoom(currentRoomName);
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
        }
      
      
        private void RuntimeDataStorage_OnChanged(SubStorage subStorage)
        {
            RuntimeDataStorage runtimeDataStorage = subStorage as RuntimeDataStorage;
            InventoryView inventoryView = runtimeDataStorage.RuntimeData.InventoryView;

            if (_persistentDataStorage?.PersistentData?.LocationContentViewCurrent == null &&
                runtimeDataStorage?.RuntimeData?.LocationContentViews != null)
            {
                _persistentDataStorage.PersistentData.LocationContentViewCurrent = 
                    runtimeDataStorage.RuntimeData.LocationContentViews[0];
            }
        }


        public async Task<EmptyResponse> ResetGame()
        {
            /////////////////////////////
            // Systems
            /////////////////////////////
            await _gameServices.Initialize(_configuration);
            await _runtimeDataStorage.Initialize(_configuration);
            await _persistentDataStorage.Initialize(_configuration);
        
            /////////////////////////////
            // Money
            /////////////////////////////
            _persistentDataStorage.PersistentData.BankAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.BankAmountInitial;
            
            _persistentDataStorage.PersistentData.CashAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.CashAmountInitial;
            
            _persistentDataStorage.PersistentData.DebitAmount = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.DebtAmountInitial;
            
            /////////////////////////////
            // Chat
            /////////////////////////////
            while (!_gameServices.HasChatView)
            {
                await Task.Delay(100);
            }
            
            // Chat - Global Room
            await _gameServices.CreateRoomSafe(GPWConstants.ChatRoomNameGlobal);
            
            // Chat - Direct Room - TODO: Should I use the existing 'DirectRooms' 
            // available on the chatView instance?
            await _gameServices.CreateRoomSafe(GPWConstants.GetChatRoomNameDirect());
            
            // Chat - Location Rooms
            foreach (LocationContentView locationContentView in _runtimeDataStorage.RuntimeData.LocationContentViews)
            {
                await _gameServices.CreateRoomSafe(
                    GPWConstants.GetChatRoomNameLocation(locationContentView.LocationContent));
            }
            
            // When running this scene directly, go back to intro scene
            // When running in production, go to the previous scene
            _runtimeDataStorage.RuntimeData.PreviousSceneName = _configuration.Scene01IntroName;
            
            // Default to global chat
            _runtimeDataStorage.RuntimeData.ChatMode = ChatMode.Global;

            /////////////////////////////
            // Turns
            /////////////////////////////
            _persistentDataStorage.PersistentData.TurnCurrent = 1;
            _persistentDataStorage.PersistentData.TurnsTotal = 
                _runtimeDataStorage.RuntimeData.RemoteConfiguration.TurnsTotal;
            _persistentDataStorage.OnChanged.Invoke(_persistentDataStorage);

            return new EmptyResponse();
        }
    }
}
