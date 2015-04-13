﻿//  Raptor - a client API for Terraria
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Raptor.Api;
using Raptor.Api.Commands;
using Raptor.Extensions;
using Terraria;

using Form = System.Windows.Forms.Form;

namespace Raptor
{
	/// <summary>
	/// The Raptor client.
	/// </summary>
	public static class Raptor
	{
		static Texture2D[] cursorTextures = new Texture2D[6];
		internal static Texture2D rectBackTexture;

		class Chat
		{
			public Color color;
			public string text;
			public int timeOut;
		}
		
		static List<string> typedChat = new List<string>();
		static int typedChatOffset;
		static List<string> typedCommands = new List<string>();
		static int typedCommandOffset;
		
		static List<Chat> chat = new List<Chat>();
		static int textBlinkTimer;
		static int chatViewOffset;
		static int chatMode;
		static List<Chat> rawChat = new List<Chat>();

		/// <summary>
		/// Gets the configuration file.
		/// </summary>
		public static Config Config { get; internal set; }

		internal static void DeInitialize()
		{
		}
		internal static void Initialize()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Main.versionNumber = "Terraria " + Main.versionNumber + "\nRaptor v" + ClientApi.ApiVersion;

			Commands.Initialize();
			Utils.Initialize();
			string configPath = "raptor.config";
			if (!File.Exists(configPath))
				File.WriteAllText(configPath, JsonConvert.SerializeObject(Config = new Config(), Formatting.Indented));
			else
				Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));

			Main.showSplash = Config.ShowSplashScreen;
			
			Form form = (Form)Form.FromHandle(ClientApi.Main.Window.Handle);
			form.ClientSizeChanged += Form_ClientSizeChanged;
			form.KeyPress += Input.Form_KeyPress;

			var State = form.WindowState;
			form.MinimumSize = new System.Drawing.Size(816, 638); 
			form.WindowState = State;
		}

		internal static void DrawCursor(SpriteBatch sb)
		{
			if (Input.CursorType == 0)
			{
				Texture2D cursor = cursorTextures[Input.CursorType];
				sb.Draw(cursor,
					new Rectangle(Input.MouseX + 1, Input.MouseY + 1, (int)(Main.cursorScale * 15.4f), (int)(Main.cursorScale * 15.4f)),
					new Color((int)(Main.cursorColor.R * 0.2f), (int)(Main.cursorColor.G * 0.2f), (int)(Main.cursorColor.B * 0.2f), (int)(Main.cursorColor.A * 0.5f)));
				sb.Draw(cursor,
					new Rectangle(Input.MouseX, Input.MouseY, (int)(Main.cursorScale * 14.0f), (int)(Main.cursorScale * 14.0f)),
					Main.cursorColor);
			}
			else
			{
				Texture2D cursor = cursorTextures[Input.CursorType];
				sb.Draw(cursor,
					new Rectangle(Input.MouseX + 1 - cursor.Width / 2, Input.MouseY + 1 - cursor.Height / 2, (int)(Main.cursorScale * 15.4f), (int)(Main.cursorScale * 15.4f)),
					new Color((int)(Main.cursorColor.R * 0.2f), (int)(Main.cursorColor.G * 0.2f), (int)(Main.cursorColor.B * 0.2f), (int)(Main.cursorColor.A * 0.5f)));
				sb.Draw(cursor,
					new Rectangle(Input.MouseX - cursor.Width / 2, Input.MouseY - cursor.Height / 2, (int)(Main.cursorScale * 14.0f), (int)(Main.cursorScale * 14.0f)),
					Main.cursorColor);
			}
		}
		internal static void DrawPlayerChat(SpriteBatch sb)
		{
			var chatRectangle = new Rectangle(92, Main.screenHeight - 51 - Config.ChatShow * 19, Main.screenWidth - 312, Config.ChatShow * 19 + 12);
			if (chatMode > 0)
			{
				string text = Main.chatText + (textBlinkTimer % 40 > 10 ? "|" : "");

				sb.DrawGuiRectangle(
					new Rectangle(92, Main.screenHeight - 33,
					Main.screenWidth - 312, 28),
					new Color(100, 100, 100, 200),
					Main.inventoryBackTexture);

				if (chatMode == 1)
				{
					sb.DrawGuiText("Chat:", new Vector2(46, Main.screenHeight - 30), Color.White);
					sb.DrawGuiText(text, new Vector2(98, Main.screenHeight - 30), Color.White);
				}
				else
				{
					sb.DrawGuiText("Cmd:", new Vector2(46, Main.screenHeight - 30), Color.Orange);
					sb.DrawGuiText("/" + text, new Vector2(98, Main.screenHeight - 30), Color.Orange);
				}

				sb.DrawGuiRectangle(chatRectangle, new Color(100, 100, 100, 200), Main.inventoryBackTexture);
			}

			int linesShown = 0;
			for (int i = -1; i + chatViewOffset >= 0 && linesShown < Config.ChatShow && (chatMode > 0 || chat[i + chatViewOffset].timeOut > 0); i--)
			{
				sb.DrawGuiText(chat[i + chatViewOffset].text,
					new Vector2(98, Main.screenHeight - 64 - linesShown++ * 19),
					chat[i + chatViewOffset].color,
					Main.fontMouseText);
			}

			if (chatMode > 0 && chat.Count > Config.ChatShow)
			{
				int scrollbarSize = (int)(Config.ChatShow * (Config.ChatShow * 19.0 - 4.0) / chat.Count);
				int scrollbarOffset = (int)((chatViewOffset - Config.ChatShow) * (Config.ChatShow * 19.0 - 4.0) / chat.Count);

				sb.Draw(Main.inventoryBackTexture,
					new Rectangle(Main.screenWidth - 232, chatRectangle.Y + scrollbarOffset + 6, 6, scrollbarSize),
					new Rectangle(8, 8, 36, 36),
					new Color(150, 150, 150, 200));
			}
		}
		internal static void LoadedContent(ContentManager content)
		{
			string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Raptor");

			Main.cursorTexture = Main.cursor2Texture = cursorTextures[0] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "Normal"));
			cursorTextures[1] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "Move"));
			cursorTextures[2] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "HorzResize"));
			cursorTextures[3] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "VertResize"));
			cursorTextures[4] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "DiagResize"));
			// it's easier to do this than to just flip the texture
			cursorTextures[5] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "DiagResize2"));

			Main.fontItemStack = Main.fontMouseText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Regular"));
			Main.fontDeathText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Title"));

			rectBackTexture = content.Load<Texture2D>(Path.Combine(dir, "UI", "RectBack"));
		}
		internal static void NewText(string text, byte r, byte g, byte b)
		{
			if (Config.LogChat)
				Log.LogInfo("Chat: {0}", text);

			rawChat.Add(new Chat { color = new Color(r, g, b), text = text });
			if (rawChat.Count > 500)
				rawChat.RemoveAt(0);

			var lineBuilder = new StringBuilder();
			float lineLength = 0f;
			int linesAdded = 0;
			float spaceLength = Main.fontMouseText.MeasureString(" ").X;

			foreach (string word in text.Split(' '))
			{
				float length = Main.fontMouseText.MeasureString(word).X + spaceLength;
				lineLength += length;

				if (lineLength > Main.screenWidth - 338f && lineBuilder.Length > 0)
				{
					chat.Add(new Chat { color = new Color(r, g, b), text = lineBuilder.ToString(), timeOut = 600 });
					lineLength = 4 * spaceLength + length;
					lineBuilder.Clear().Append("    ").Append(word).Append(" ");
					linesAdded++;
				}
				else
					lineBuilder.Append(word).Append(" ");
			}
			if (lineBuilder.Length > 0)
			{
				chat.Add(new Chat { color = new Color(r, g, b), text = lineBuilder.ToString(), timeOut = 600 });
				linesAdded++;
			}

			chatViewOffset += linesAdded;
		}
		internal static void Update()
		{
			if (!Main.hasFocus)
				return;

			Input.CursorType = 0;
			Input.DisabledMouse = false;
			Input.DisabledKeyboard = false;
			Input.Update();
			textBlinkTimer++;

			// Do not allow Terraria's chat code to run b/c we handle it ourselves
			Main.chatRelease = false;

			if (Main.gameMenu)
			{
				if (Main.netMode != 1)
				{
					chat.Clear();
					rawChat.Clear();
					chatViewOffset = 0;
				}
				typedChat.Clear();
				typedCommands.Clear();
				typedChatOffset = 0;
				typedCommandOffset = 0;
				chatMode = 0;
				Main.chatText = "";
				return;
			}

			#region Chat handling
			if (((Input.IsKeyTapped(Keys.Enter) && !Input.Alt) || (Input.IsKeyTapped(Keys.OemQuestion) && !Input.Shift))
				&& chatMode == 0 && !Main.editSign && !Main.gameMenu && !Input.DisabledKeyboard)
			{
				chatMode = Input.IsKeyTapped(Keys.OemQuestion) ? 2 : 1;
				Main.PlaySound(10);
			}
			else if (chatMode > 0)
			{
				var chatRectangle = new Rectangle(96, Main.screenHeight - 49 - Config.ChatShow * 19, Main.screenWidth - 312, Config.ChatShow * 19 + 12);
				if (chatRectangle.Contains(Input.MouseX, Input.MouseY))
				{
					Input.DisabledMouse = true;
					if (chat.Count > Config.ChatShow)
					{
						chatViewOffset += Math.Sign(Input.MouseDScroll) * -Config.ChatScrollSpeed;
						chatViewOffset = (chatViewOffset < Config.ChatShow) ? Config.ChatShow : chatViewOffset;
						chatViewOffset = (chatViewOffset > chat.Count) ? chat.Count : chatViewOffset;
					}
				}

				Input.DisabledKeyboard = true;
				Main.chatText = Input.GetInputText(Main.chatText);
				while (Main.fontMouseText.MeasureString(Main.chatText).X > Main.screenWidth - 324)
					Main.chatText = Main.chatText.Substring(0, Main.chatText.Length - 1);

				if (Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.Up))
				{
					if (chatMode == 1 && typedChat.Count != 0)
					{
						typedChatOffset--;
						if (typedChatOffset < 0)
							typedChatOffset = 0;
						Main.chatText = typedChat[typedChatOffset];
					}
					else if (chatMode == 2 && typedCommands.Count != 0)
					{
						typedCommandOffset--;
						if (typedCommandOffset < 0)
							typedCommandOffset = 0;
						Main.chatText = typedCommands[typedCommandOffset];
					}
				}
				else if (Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.Down))
				{
					if (chatMode == 1 && typedChat.Count != 0)
					{
						typedChatOffset++;

						if (typedChatOffset >= typedChat.Count)
						{
							typedChatOffset = typedChat.Count;
							Main.chatText = "";
						}
						else
							Main.chatText = typedChat[typedChatOffset];
					}
					else if (chatMode == 2 && typedCommands.Count != 0)
					{
						typedCommandOffset++;

						if (typedCommandOffset >= typedCommands.Count)
						{
							typedCommandOffset = typedCommands.Count;
							Main.chatText = "";
						}
						else
							Main.chatText = typedCommands[typedCommandOffset];
					}
				}
				else if (Input.IsKeyTapped(Keys.Escape) || (Input.IsKeyTapped(Keys.Enter) && Main.chatText == ""))
				{
					chatMode = 0;
					chatViewOffset = chat.Count;
					typedChatOffset = typedChat.Count;
					typedCommandOffset = typedCommands.Count;

					Main.chatText = "";
					Main.PlaySound(11);
				}
				else if (Input.IsKeyTapped(Keys.Enter))
				{
					if (chatMode == 1)
					{
						typedChat.Add(Main.chatText);
						if (typedChat.Count > 1000)
							typedChat.RemoveAt(0);
						typedChatOffset = typedChat.Count;
						if (Main.netMode == 1)
							NetMessage.SendData(25, -1, -1, Main.chatText, Main.myPlayer);
						else
							Main.NewText(String.Format("<{0}> {1}", Main.player[Main.myPlayer].name, Main.chatText));
					}
					else
					{
						typedCommands.Add(Main.chatText);
						if (typedCommands.Count > 1000)
							typedCommands.RemoveAt(0);
						typedCommandOffset = typedCommands.Count;
						Commands.Execute(Main.chatText);
					}

					Main.chatText = "";
					Main.PlaySound(11);
					chatViewOffset = chat.Count;
				}
			}
			for (int i = 0; i < chat.Count; i++)
			{
				if (chat[i].timeOut > 0)
					chat[i].timeOut--;
			}
			for (int i = 0; i < rawChat.Count; i++)
			{
				if (rawChat[i].timeOut > 0)
					rawChat[i].timeOut = 0;
			}
			#endregion
			#region Keybinds
			if (chatMode == 0 && !Main.editSign && !Main.gameMenu && !Input.DisabledKeyboard)
			{
				foreach (var kvp in Config.Keybinds)
				{
					string key = kvp.Key;
					var keybind = new Input.Keybind();
					while (!Utils.TryParseXNAKey(key, out keybind.Key))
					{
						switch (key[0])
						{
							case '!':
								keybind.Alt = true;
								break;
							case '^':
								keybind.Control = true;
								break;
							case '+':
								keybind.Shift = true;
								break;
						}
						key = key.Substring(1);
					}

					if (Input.IsKeyTapped(keybind.Key) &&
						Input.Alt == keybind.Alt && Input.Control == keybind.Control && Input.Shift == keybind.Shift)
					{
						foreach (var command in kvp.Value)
							Commands.Execute(command);
					}
				}
			}
			#endregion

			if (Input.DisabledMouse)
				Main.mouseState = Main.oldMouseState = new MouseState(-100, -100, Input.MouseScroll, 0, 0, 0, 0, 0);
			if (Input.DisabledKeyboard)
				Main.keyState = Main.inputText = Main.oldInputText = new KeyboardState(null);
		}

		internal static void Form_ClientSizeChanged(object sender, EventArgs e)
		{
			int newWidth = ClientApi.Main.Window.ClientBounds.Width;
			if (newWidth < 800)
				newWidth = 800;

			if (newWidth != Main.screenWidth)
			{
				chat.Clear();
				var lineBuilder = new StringBuilder();
				float spaceLength = Main.fontMouseText.MeasureString(" ").X;

				for (int i = 0; i < rawChat.Count; i++)
				{
					float lineLength = 0f;

					foreach (string word in rawChat[i].text.Split(' '))
					{
						float length = Main.fontMouseText.MeasureString(word).X + spaceLength;
						lineLength += length;

						if (lineLength > newWidth - 338f)
						{
							chat.Add(new Chat { color = rawChat[i].color, text = lineBuilder.ToString(), timeOut = rawChat[i].timeOut });
							lineLength = 4 * spaceLength + length;
							lineBuilder.Clear().Append("    ").Append(word).Append(" ");
						}
						else
							lineBuilder.Append(word).Append(" ");
					}
					if (lineBuilder.Length > 0)
					{
						chat.Add(new Chat { color = rawChat[i].color, text = lineBuilder.ToString(), timeOut = rawChat[i].timeOut });
						lineBuilder.Clear();
					}
				}
				chatViewOffset = chat.Count;
			}
		}
	}
}
