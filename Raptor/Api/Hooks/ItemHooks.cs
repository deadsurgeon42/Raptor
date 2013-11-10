using System;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for Item hooks.
	/// </summary>
	public class ItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the Item instance.
		/// </summary>
		public Item Item
		{
			get;
			private set;
		}

		internal ItemEventArgs(Item item)
		{
			Item = item;
		}
	}

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
				SetDefaults(null, new ItemEventArgs((Item)item));
		}
		#endregion
	}
}
