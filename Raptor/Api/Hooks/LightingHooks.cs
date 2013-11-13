using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's lighting hooks.
	/// </summary>
	public static class LightingHooks
	{
		#region Color
		/// <summary>
		/// Occurs before the tiles are colored.
		/// </summary>
		public static event EventHandler Color;
		internal static void InvokeColor()
		{
			if (Color != null)
				Color(null, EventArgs.Empty);
		}
		#endregion

		#region ColorR
		/// <summary>
		/// Event arguments for Color-related hooks.
		/// </summary>
		public class ColorEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the X position of the tile.
			/// </summary>
			public int X { get; private set; }
			/// <summary>
			/// Gets the Y position of the tile.
			/// </summary>
			public int Y { get; private set; }
			internal ColorEventArgs(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
		/// <summary>
		/// Occurs before a tile is colored red.
		/// </summary>
		public static event EventHandler<ColorEventArgs> ColorR;
		internal static bool InvokeColorR(int x, int y)
		{
			if (ColorR != null)
			{
				var args = new ColorEventArgs(x, y);
				ColorR(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region ColorG
		/// <summary>
		/// Occurs before a tile is colored green.
		/// </summary>
		public static event EventHandler<ColorEventArgs> ColorG;
		internal static bool InvokeColorG(int x, int y)
		{
			if (ColorG != null)
			{
				var args = new ColorEventArgs(x, y);
				ColorG(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region ColorB
		/// <summary>
		/// Occurs before a tile is colored blue.
		/// </summary>
		public static event EventHandler<ColorEventArgs> ColorB;
		internal static bool InvokeColorB(int x, int y)
		{
			if (ColorB != null)
			{
				var args = new ColorEventArgs(x, y);
				ColorB(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
	}
}
