using System;
using Beamable.Common;
using UnityEngine;

namespace Beamable.Samples.GPW
{
	/// <summary>
	/// Custom-formatted readme file with markdown-like display. 
	/// 
	/// Inspired by Unity's "Learn" Sample Projects
	/// 
	/// </summary>
	[CreateAssetMenu(
		fileName = "ReadMe",
		menuName = Constants.MenuItems.Assets.Paths.MENU_ITEM_PATH_ASSETS_BEAMABLE_SAMPLES + "/" +
		           "ReadMe",
		order = Constants.MenuItems.Assets.Orders.MENU_ITEM_PATH_ASSETS_BEAMABLE_ORDER_1)]
	public class Readme : ScriptableObject
	{
		public Texture2D icon;
		public string title;
		public Section[] sections;
		public bool loadedLayout;

		[Serializable]
		public class Section
		{
			public string heading, text, linkText, url;
		}
	}	
}
