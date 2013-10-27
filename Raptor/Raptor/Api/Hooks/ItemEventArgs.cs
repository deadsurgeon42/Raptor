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
		public Item Item { get; internal set; }
	}
}
