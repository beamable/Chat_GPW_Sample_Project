using System;
using System.Threading.Tasks;
using Beamable.Api.CloudSaving;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.GPW.Data.Storage
{
	[Serializable]
	public class PersistentData
	{
		public int BankAmount;
		public int CashAmount;
		public int DebtAmount;
		public int TurnCurrent;
		public int TurnsTotal;
		public int CurrentLocationIndex = -1;
		
		public bool IsGameOver
		{
			get
			{
				return TurnCurrent >= TurnsTotal;
			}
		}
	}
	
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class PersistentDataStorage : SubStorage<PersistentDataStorage>
	{
		//  Events  --------------------------------------
		
		//  Properties  ----------------------------------
		public PersistentData PersistentData = new PersistentData();

		//  Fields  --------------------------------------
		private CloudSavingService _cloudSavingService = null;

		//  Other Methods  --------------------------------
		public override async Task Initialize(Configuration configuration)
		{
			if (!IsInitialized)
			{
				IBeamableAPI beamableAPI = await Beamable.API.Instance;
				_cloudSavingService = beamableAPI.CloudSavingService;
				IsInitialized = true;
			}
		}
	}
}

