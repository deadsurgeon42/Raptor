using System;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's item hooks.
	/// </summary>
	public static class ItemHooks
	{
		#region SetDefaults
		/// <summary>
		/// The event that runs after an item's default values are set.
		/// </summary>
		public static event EventHandler<ItemEventArgs> SetDefaults;

		internal static void InvokeSetDefaults(object item)
		{
			if (SetDefaults != null)
			{
				SetDefaults(null, new ItemEventArgs { Item = (Item)item });
			}
		}
		#endregion
	}
}
