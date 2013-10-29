using System;
using System.Collections.Generic;
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
		/// The events that run before and after drawing certain things.
		/// </summary>
		public static Dictionary<string, EventHandler<DrawEventArgs>> Draw = new Dictionary<string, EventHandler<DrawEventArgs>>
		{
			{ "Pre", null },
			{ "PreNPCChat", null },
			{ "PrePlayerChat", null },
			{ "Post", null },
			{ "PostPlayerChat", null },
			{ "PostNPCChat", null }
		};

		internal static bool InvokeDraw(SpriteBatch spriteBatch, string type)
		{
			switch (type)
			{
				case "Pre":
					Main.mouseTextColor = 255;
					Main.mouseTextColorChange = 0;
					break;
				case "PrePlayerChat":
					Raptor.DrawPrePlayerChat(spriteBatch);
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

		#region Initialized
		/// <summary>
		/// The event that runs after the game is initialized.
		/// </summary>
		public static event EventHandler Initialized;
		internal static void InvokeInitialized()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Raptor.Initialize();

			if (Initialized != null)
				Initialized(null, EventArgs.Empty);
		}
		#endregion

		#region InputText
		internal static string InvokeInputText(string text)
		{
			if (!Main.hasFocus)
				return text;

			string newText = text;
			if (Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.Backspace) && newText.Length != 0)
			{
				if (Input.Control)
				{
					string[] words = newText.Split(' ');
					newText = String.Join(" ", words, 0, words.Length - 1);
				}
				else
					newText = newText.Substring(0, newText.Length - 1);
			}
			else if (Input.Control && Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.V) && Clipboard.ContainsText())
			{
				newText += Clipboard.GetText();
			}
			else
				newText += Input.TypedString;

			return newText;
		}
		#endregion

		#region LoadedContent
		/// <summary>
		/// The event that runs after the game loaded content.
		/// </summary>
		public static event EventHandler<LoadContentEventArgs> LoadedContent;

		internal static void InvokeLoadedContent(ContentManager content)
		{
			string contentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Raptor");

			Main.fontItemStack = Main.fontMouseText = content.Load<SpriteFont>(Path.Combine(contentDirectory, "Font"));
			Main.fontDeathText = content.Load<SpriteFont>(Path.Combine(contentDirectory, "TitleFont"));
			Texture2D chatBack = content.Load<Texture2D>(Path.Combine(contentDirectory, "ChatBack"));

			Main.chatBackTexture = content.Load<Texture2D>(Path.Combine(contentDirectory, "NpcChatBack"));
			Main.inventoryBackTexture = chatBack;
			for (int i = 2; i <= 12; i++)
				typeof(Main).GetField("inventoryBack" + i + "Texture").SetValue(null, chatBack);

			if (LoadedContent != null)
				LoadedContent(null, new LoadContentEventArgs { Content = content });
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
				var args = new NewTextEventArgs
				{
					Color = new Color(r, g, b),
					Text = text
				};

				NewText(null, args);

				if (!args.Handled)
					Raptor.NewText(text, r, g, b);
			}
			else
				Raptor.NewText(text, r, g, b);
		}
		#endregion

		#region PostUpdate
		/// <summary>
		/// The event that runs after the game is updated each frame.
		/// </summary>
		public static event EventHandler PostUpdate;
		internal static void InvokePostUpdate()
		{
			if (PostUpdate != null)
				PostUpdate(null, EventArgs.Empty);
		}
		#endregion

		#region PreUpdate
		/// <summary>
		/// The event that runs before the game is updated each frame.
		/// </summary>
		public static event EventHandler PreUpdate;
		internal static void InvokePreUpdate()
		{
			Input.Update();
			Raptor.PreUpdate();

			if (PreUpdate != null)
				PreUpdate(null, EventArgs.Empty);
		}
		#endregion
	}
}
