using System.Threading.Tasks;

namespace Beamable.Samples.GPW.Data.Storage
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class RuntimeDataStorage
    {
        //  Properties  ----------------------------------
        public bool IsInitialized { get { return _isInitialized; }}

        //  Fields  --------------------------------------
        private bool _isInitialized = false;
        
        //  Unity Methods  --------------------------------

        //  Other Methods  --------------------------------
        public async Task Initialize(Configuration configuration)
        {
            if (!_isInitialized)
            {
                IBeamableAPI beamableAPI = await Beamable.API.Instance;
                _isInitialized = true;
            }
        }
    }
}
