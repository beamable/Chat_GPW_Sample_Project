using System.Threading.Tasks;
using Beamable.Samples.GPW.Data.Factories;
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
		public SubStorageEvent OnChanged = new SubStorageEvent();
		
		//  Properties  ----------------------------------
		public bool IsInitialized {  get { return _isInitialized; } set { _isInitialized = value; } }

		//  Fields  --------------------------------------
		private bool _isInitialized = false;

		//  Other Methods  --------------------------------
		
		//Used by most Substorage
		public virtual Task Initialize(Configuration configuration)
		{
			return null;
		}
		
		//Used by some Substorage
		public virtual Task Initialize(Configuration configuration, IDataFactory dataFactory)
		{
			return null;
		}
		
		public void ForceRefresh()
		{
			OnChanged.Invoke(this);
		}
	}
}

