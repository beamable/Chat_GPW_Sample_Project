using System.Threading.Tasks;
using UnityEngine.Events;

namespace Beamable.Samples.GPW.Data.Storage
{
	public class SubStorageEvent : UnityEvent<SubStorage>{}

	public class SubStorage {}
	
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class SubStorage<T> : SubStorage
	{
		//  Events  --------------------------------------
		public SubStorageEvent OnRefreshed = new SubStorageEvent();
		
		//  Properties  ----------------------------------
		public bool IsInitialized {  get { return _isInitialized; } set { _isInitialized = value; } }

		//  Fields  --------------------------------------
		private bool _isInitialized = false;

		//  Other Methods  --------------------------------
		public virtual Task Initialize(Configuration configuration)
		{
			return null;
		}
		
		public void ForceRefresh()
		{
			OnRefreshed.Invoke(this);
		}
	}
}

