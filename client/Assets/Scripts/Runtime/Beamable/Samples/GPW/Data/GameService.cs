using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.Core.Components;
using Beamable.Samples.GPW.Content;
using UnityEngine.Events;

namespace Beamable.Samples.GPW.Data
{
	public class RefreshEvent : UnityEvent<GameService>{}
	
	/// <summary>
	/// Game-specific wrapper for calling Beamable online services
	/// </summary>
	public class GameService 
	{
		//  Events  --------------------------------------
		public RefreshEvent OnRefresh = new RefreshEvent();
		
		//  Properties  ----------------------------------
		public RemoteConfiguration RemoteConfiguration { get { return _remoteConfiguration; } }
		public LocationContent LocationCurrent { get { return _locationCurrent; } }
		public List<LocationContent> LocationContents { get { return _locationContents; } }
		public List<ProductContent> ProductContents { get { return _productContents; } }
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }

		//  Fields  --------------------------------------
		private long _localPlayerDbid;
		private RemoteConfiguration _remoteConfiguration;
		private List<LocationContent> _locationContents = new List<LocationContent>();
		private List<ProductContent> _productContents = new List<ProductContent>();
		private LocationContent _locationCurrent = null;
		private bool _isInitialized = false;
		private IBeamableAPI _beamableAPI = null;

		//  Unity Methods  --------------------------------

		//  Other Methods  --------------------------------
		public async Task Initialize(Configuration configuration)
		{
			if (!_isInitialized)
			{
				_beamableAPI = await Beamable.API.Instance;
				_localPlayerDbid = _beamableAPI.User.id;
				_remoteConfiguration = await configuration.RemoteConfigurationRef.Resolve();
				
				_locationContents.Clear();
				foreach (var locationContentRef in _remoteConfiguration.LocationContentRefs)
				{
					LocationContent locationContent = await locationContentRef.Resolve();
					_locationContents.Add(locationContent);
				}
				
				_productContents.Clear();
				foreach (var productContentRef in _remoteConfiguration.ProductContentRefs)
				{
					ProductContent productContent = await productContentRef.Resolve();
					_productContents.Add(productContent);
				}

				_locationCurrent = _locationContents[0];
				
				_isInitialized = true;
			}

			OnRefresh.Invoke(this);
		}
	}
}
