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
		/// Event arguments for SetDefaults hooks.
		/// </summary>
		public class SetDefaultsEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the Item instance.
			/// </summary>
			public Item Item { get; private set; }
			internal SetDefaultsEventArgs(Item item)
			{
				Item = item;
			}
		}
		/// <summary>
		/// The event that runs after an item's default values are set.
		/// </summary>
		public static event EventHandler<SetDefaultsEventArgs> SetDefaults;
		internal static void InvokeSetDefaults(object item)
		{
			if (SetDefaults != null)
				SetDefaults(null, new SetDefaultsEventArgs((Item)item));
		}
		#endregion
	}
}
