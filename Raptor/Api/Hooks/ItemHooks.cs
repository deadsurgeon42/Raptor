//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2014 MarioE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
