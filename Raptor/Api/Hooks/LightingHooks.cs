//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
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
	///   The API's lighting hooks.
	/// </summary>
	public static class LightingHooks
	{
		#region Colored

		/// <summary>
		///   Event arguments for Color hooks.
		/// </summary>
		public class ColorEventArgs : EventArgs
		{
			internal ColorEventArgs(Lighting.LightingSwipeData swipeData)
			{
				SwipeData = swipeData;
			}

			/// <summary>
			///   Gets the lighting state for the adjacent block.
			/// </summary>
			public Lighting.LightingSwipeData SwipeData { get; private set; }
		}

		/// <summary>
		///   The event that runs before colors are calculated.
		/// </summary>
		public static event EventHandler<ColorEventArgs> Color;

		internal static void InvokeColor(object swipeData)
		{
			Color?.Invoke(null, new ColorEventArgs((Lighting.LightingSwipeData) swipeData));
		}

		#endregion
	}
}