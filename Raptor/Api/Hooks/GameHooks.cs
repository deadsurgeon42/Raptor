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
		};
		internal static bool InvokeDraw(SpriteBatch spriteBatch, string type)
		{
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

		#region ILModified
		/// <summary>
		/// Event arguments for ILModified hooks.
		/// </summary>
		public class ILModifiedEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the assembly definition.
			/// </summary>
			public AssemblyDefinition Assembly { get; private set; }
			/// <summary>
			/// Gets the main module definition.
			/// </summary>
			public ModuleDefinition Module
			{
				get { return Assembly.MainModule; }
			}
			internal ILModifiedEventArgs(AssemblyDefinition asm)
			{
				Assembly = asm;
			}
		}
		/// <summary>
		/// The event that runs after the game's IL is modified.
		/// </summary>
		public static event EventHandler<ILModifiedEventArgs> ILModified;
		internal static void InvokeILModified(AssemblyDefinition asm)
		{
			if (ILModified != null)
				ILModified(null, new ILModifiedEventArgs(asm));
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
