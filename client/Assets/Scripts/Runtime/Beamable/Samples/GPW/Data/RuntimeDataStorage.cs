using System.Threading.Tasks;
using Beamable.Samples.Core.Components;
using Beamable.Samples.GPW.Content;

namespace Beamable.Samples.GPW.Data
{
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class RuntimeDataStorage : SingletonMonobehavior<RuntimeDataStorage>
	{
		//  Properties  ----------------------------------
		public bool IsInitialized { get { return _isInitialized; } set { _isInitialized = value; } }
		public GameService GameService { get { return _gameService; } }

		//  Fields  --------------------------------------
		private GameService _gameService = new GameService();
		private bool _isInitialized = false;

		//  Unity Methods  --------------------------------

		//  Other Methods  --------------------------------
		public async Task Initialize(Configuration configuration)
		{
			if (!_isInitialized)
			{
				IBeamableAPI beamableAPI = await Beamable.API.Instance;
				await _gameService.Initialize(configuration);
				_isInitialized = true;
			}
		}
	}
}
