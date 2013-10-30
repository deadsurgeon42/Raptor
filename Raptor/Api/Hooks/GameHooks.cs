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
	#region DrawEventArgs
	/// <summary>
	/// Event arguments for Draw hooks.
	/// </summary>
	public class DrawEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the sprite batch.
		/// </summary>
		public SpriteBatch SpriteBatch
		{
			get;
			internal set;
		}
	}
	#endregion
	#region DrawnEventArgs
	/// <summary>
	/// Event arguments for Drawn hooks.
	/// </summary>
	public class DrawnEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the sprite batch.
		/// </summary>
		public SpriteBatch SpriteBatch
		{
			get;
			internal set;
		}
	}
	#endregion

	#region ILModifiedEventArgs
	/// <summary>
	/// Event arguments for ILModified hooks.
	/// </summary>
	public class ILModifiedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the assembly definition.
		/// </summary>
		public AssemblyDefinition Assembly
		{
			get;
			internal set;
		}
	}
	#endregion

	#region LoadedContentEventArgs
	/// <summary>
	/// Event arguments for LoadedContent hooks.
	/// </summary>
	public class LoadedContentEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the content manager used for loading content.
		/// </summary>
		public ContentManager Content
		{
			get;
			internal set;
		}
	}
	#endregion

	#region NewTextEventArgs
	/// <summary>
	/// Event arguments for NewText hooks.
	/// </summary>
	public class NewTextEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		public Color Color
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text
		{
			get;
			set;
		}
	}
	#endregion

	#region UpdateEventArgs
	/// <summary>
	/// Event arguments for Update hooks.
	/// </summary>
	public class UpdateEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the game time.
		/// </summary>
		public GameTime GameTime
		{
			get;
			internal set;
		}
	}
	#endregion
	#region UpdatedEventArgs
	/// <summary>
	/// Event arguments for Updated hooks.
	/// </summary>
	public class UpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the game time.
		/// </summary>
		public GameTime GameTime
		{
			get;
			internal set;
		}
	}
	#endregion

	/// <summary>
	/// The API's game hooks.
	/// </summary>
	public static class GameHooks
	{
		#region Draw
		/// <summary>
		/// The events that run before drawing certain things.
		/// </summary>
		public static Dictionary<string, EventHandler<DrawEventArgs>> Draw = new Dictionary<string, EventHandler<DrawEventArgs>>
		{
			{ "", null },
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
				case "PlayerChat":
					Raptor.DrawPlayerChat(spriteBatch);
					return true;
			}

			if (Draw[type] != null)
			{
				var args = new DrawEventArgs { SpriteBatch = spriteBatch };
				Draw[type](null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Drawn
		/// <summary>
		/// The events that run after drawing certain things.
		/// </summary>
		public static Dictionary<string, EventHandler<DrawnEventArgs>> Drawn = new Dictionary<string, EventHandler<DrawnEventArgs>>
		{
			{ "", null },
			{ "NPCChat", null },
			{ "PlayerChat", null },
		};

		internal static void InvokeDrawn(SpriteBatch spriteBatch, string type)
		{
			if (Drawn[type] != null)
				Drawn[type](null, new DrawnEventArgs { SpriteBatch = spriteBatch });
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
		/// The event that runs after the game's IL is modified.
		/// </summary>
		public static event EventHandler<ILModifiedEventArgs> ILModified;
		internal static void InvokeILModified(AssemblyDefinition asm)
		{
			if (ILModified != null)
				ILModified(null, new ILModifiedEventArgs { Assembly = asm });
		}
		#endregion

		#region LoadedContent
		/// <summary>
		/// The event that runs after the game loaded content.
		/// </summary>
		public static event EventHandler<LoadedContentEventArgs> LoadedContent;

		internal static void InvokeLoadedContent(ContentManager content)
		{
			Raptor.LoadedContent(content);

			if (LoadedContent != null)
				LoadedContent(null, new LoadedContentEventArgs { Content = content });
		}
		#endregion

		#region NewText
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
				var args = new NewTextEventArgs { Color = new Color(r, g, b), Text = text };
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
		/// The event that runs before the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Update;
		internal static bool InvokeUpdate(GameTime gameTime)
		{
			Raptor.Update();

			if (Update != null)
			{
				var args = new UpdateEventArgs { GameTime = gameTime };
				Update(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Updated
		/// <summary>
		/// The event that runs after the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdatedEventArgs> Updated;
		internal static void InvokeUpdated(GameTime gameTime)
		{
			if (Updated != null)
				Updated(null, new UpdatedEventArgs { GameTime = gameTime });
		}
		#endregion
	}
}
