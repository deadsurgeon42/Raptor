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
		/// The keybinds.
		/// </summary>
		[Description("The keybinds.")]
		public Dictionary<string, List<string>> Keybinds = new Dictionary<string, List<string>>();
		/// <summary>
		/// Whether to log chat.
		/// </summary>
		[Description("Whether to log chat.")]
		public bool LogChat = true;
		/// <summary>
		/// Whether to show the Terraria splash screen.
		/// </summary>
		[Description("Whether to show the Terraria splash screen.")]
		public bool ShowSplashScreen = false;
	}
}
