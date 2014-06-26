//  Raptor - a client API for Terraria
//  Copyright (C) 2013 MarioE
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's lighting hooks.
	/// </summary>
	public static class LightingHooks
	{
		#region Colored
		/// <summary>
		/// Event arguments for Colored hooks.
		/// </summary>
		public class ColoredEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the lighting state for the adjacent block.
			/// </summary>
			public Lighting.LightingSwipeData SwipeData { get; private set; }

			internal ColoredEventArgs(Lighting.LightingSwipeData swipeData)
			{
				SwipeData = swipeData;
			}
		}
		/// <summary>
		/// The event that runs when colors are calculated
		/// </summary>
		public static event EventHandler<ColoredEventArgs> Colored;
		internal static void InvokeColored(object swipeData)
		{
			if (Colored != null)
				Colored(null, new ColoredEventArgs((Lighting.LightingSwipeData)swipeData));
		}
		#endregion
	}
}
