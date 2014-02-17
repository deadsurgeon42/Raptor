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
	/// The API's game hooks.
	/// </summary>
	public static class GameHooks
	{
		#region Draw
		/// <summary>
		/// Event arguments for Draw hooks.
		/// </summary>
		public class DrawEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the sprite batch.
			/// </summary>
			public SpriteBatch SpriteBatch { get; private set; }
			internal DrawEventArgs(SpriteBatch sb)
			{
				SpriteBatch = sb;
			}
		}
		/// <summary>
		/// The events that run before drawing certain things.
		/// </summary>
		public static Dictionary<string, EventHandler<DrawEventArgs>> Draw = new Dictionary<string, EventHandler<DrawEventArgs>>
		{
			{ "", null },
			{ "Interface", null },
			{ "Inventory", null },
			{ "Map", null },
			{ "Menu", null },
			{ "NPCChat", null },
			{ "PlayerChat", null },
			{ "Tiles", null },
			{ "Walls", null },
			{ "Wires", null },
		};
		internal static bool InvokeDraw(SpriteBatch spriteBatch, string type)
		{
			if (!ClientApi.Main.IsActive)
				return true;

			switch (type)
			{
				case "":
					Raptor.Draw(spriteBatch);
					break;
				case "Interface":
					Raptor.DrawInterface(spriteBatch);
					break;
				case "PlayerChat":
					Raptor.DrawPlayerChat(spriteBatch);
					return true;
			}

			if (Draw[type] != null)
			{
				var args = new DrawEventArgs(spriteBatch);
				Draw[type](null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Drawn
		/// <summary>
		/// Event arguments for Drawn hooks.
		/// </summary>
		public class DrawnEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the sprite batch.
			/// </summary>
			public SpriteBatch SpriteBatch { get; private set; }

			internal DrawnEventArgs(SpriteBatch sb)
			{
				SpriteBatch = sb;
			}
		}
		/// <summary>
		/// The events that run after drawing certain things.
		/// </summary>
		public static Dictionary<string, EventHandler<DrawnEventArgs>> Drawn = new Dictionary<string, EventHandler<DrawnEventArgs>>
		{
			{ "", null },
			{ "Interface", null },
			{ "Inventory", null },
			{ "Map", null },
			{ "Menu", null },
			{ "NPCChat", null },
			{ "PlayerChat", null },
			{ "Tiles", null },
			{ "Walls", null },
			{ "Wires", null },
		};
		internal static void InvokeDrawn(SpriteBatch spriteBatch, string type)
		{
			if (Drawn[type] != null)
				Drawn[type](null, new DrawnEventArgs(spriteBatch));
		}
		#endregion

		#region Initialized
		/// <summary>
		/// The event that runs after the game is initialized.
		/// </summary>
		public static event EventHandler Initialized;
		internal static void InvokeInitialized()
		{
			Raptor.Initialize();

			if (Initialized != null)
				Initialized(null, EventArgs.Empty);
		}
		#endregion

		#region LoadedContent
		/// <summary>
		/// Event arguments for LoadedContent hooks.
		/// </summary>
		public class LoadedContentEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the content manager used for loading content.
			/// </summary>
			public ContentManager Content { get; private set; }
			internal LoadedContentEventArgs(ContentManager cm)
			{
				Content = cm;
			}
		}
		/// <summary>
		/// The event that runs after the game loaded content.
		/// </summary>
		public static event EventHandler<LoadedContentEventArgs> LoadedContent;
		internal static void InvokeLoadedContent(ContentManager cm)
		{
			Raptor.LoadedContent(cm);

			if (LoadedContent != null)
				LoadedContent(null, new LoadedContentEventArgs(cm));
		}
		#endregion

		#region NewText
		/// <summary>
		/// Event arguments for NewText hooks.
		/// </summary>
		public class NewTextEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets or sets the color.
			/// </summary>
			public Color Color { get; set; }
			/// <summary>
			/// Gets or sets the text.
			/// </summary>
			public string Text { get; set; }
			internal NewTextEventArgs(Color color, string text)
			{
				Color = color;
				Text = text;
			}
		}
		/// <summary>
		/// The event that runs before new text is printed.
		/// </summary>
		public static event EventHandler<NewTextEventArgs> NewText;
		internal static void InvokeNewText(string text, byte r, byte g, byte b)
		{
			if ((r | g | b) == 0)
				r = g = b = 255;

			if (NewText != null)
			{
				var args = new NewTextEventArgs(new Color(r, g, b), text);
				NewText(null, args);

				if (!args.Handled)
					Raptor.NewText(text, r, g, b);
			}
			else
				Raptor.NewText(text, r, g, b);
		}
		#endregion

		#region Update
		/// <summary>
		/// Event arguments for Update hooks.
		/// </summary>
		public class UpdateEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the game time.
			/// </summary>
			public GameTime GameTime { get; private set; }
			internal UpdateEventArgs(GameTime gt)
			{
				GameTime = gt;
			}
		}
		/// <summary>
		/// The event that runs before the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Update;
		internal static bool InvokeUpdate(GameTime gt)
		{
			Raptor.Update();

			if (Update != null)
			{
				var args = new UpdateEventArgs(gt);
				Update(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Updated
		/// <summary>
		/// Event arguments for Updated hooks.
		/// </summary>
		public class UpdatedEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the game time.
			/// </summary>
			public GameTime GameTime { get; private set; }
			internal UpdatedEventArgs(GameTime gt)
			{
				GameTime = gt;
			}
		}
		/// <summary>
		/// The event that runs after the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdatedEventArgs> Updated;
		internal static void InvokeUpdated(GameTime gt)
		{
			if (Updated != null)
				Updated(null, new UpdatedEventArgs(gt));
		}
		#endregion
	}
}
