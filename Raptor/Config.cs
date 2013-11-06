using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;

namespace Raptor
{
	/// <summary>
	/// The Raptor configuration file.
	/// </summary>
	public class Config
	{
		/// <summary>
		/// The number of chat lines to show.
		/// </summary>
		[Description("The number of chat lines to show.")]
		public int ChatShow = 16;
		/// <summary>
		/// The speed to scroll chat with the mouse wheel.
		/// </summary>
		[Description("The speed to scroll chat with the mouse wheel.")]
		public int ChatScrollSpeed = 6;
		/// <summary>
		/// The key bindings to commands.
		/// </summary>
		[Description("The key bindings to commands.")]
		public Dictionary<Keys, string> KeyBindings = new Dictionary<Keys, string>();
		/// <summary>
		/// Whether to show the Terraria splash screen.
		/// </summary>
		[Description("Whether to show the Terraria splash screen.")]
		public bool ShowSplashScreen = false;
	}
}
