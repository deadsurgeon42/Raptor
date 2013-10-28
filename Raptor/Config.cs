using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Raptor
{
	/// <summary>
	/// The configuration file.
	/// </summary>
	public class Config
	{
		/// <summary>
		/// The number of chat lines to show.
		/// </summary>
		public int ChatShow = 10;
		/// <summary>
		/// The speed to scroll chat with the middle wheel.
		/// </summary>
		public int ChatScrollSpeed = 3;
		/// <summary>
		/// The key bindings to commands.
		/// </summary>
		public Dictionary<Keys, string> KeyBindings = new Dictionary<Keys, string>();
		/// <summary>
		/// Whether to show the Terraria splash screen.
		/// </summary>
		public bool ShowSplashScreen = false;
	}
}
