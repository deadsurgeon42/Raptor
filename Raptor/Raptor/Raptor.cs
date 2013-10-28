using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using NLua;
using Raptor.Api;
using Raptor.Api.Commands;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// The Raptor client.
	/// </summary>
	public static class Raptor
	{
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
		static uint chatBlinkTimer;
		static int chatViewOffset;
		static bool commandMode;
		static List<Chat> rawChat = new List<Chat>();

		internal static List<Task> scriptTasks = new List<Task>();

		/// <summary>
		/// Gets the configuration file.
		/// </summary>
		public static Config Config
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the lua instance.
		/// </summary>
		public static Lua Lua
		{
			get;
			private set;
		}

		internal static void DeInitialize()
		{
			string configPath = Path.Combine("Raptor", "config.json");
			File.WriteAllText(configPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
		}
		internal static void Initialize()
		{
			string version = "Raptor v" + ClientApi.ApiVersion;
			ClientApi.Main.Window.Title = version;
			Main.chTitle = false;
			Main.versionNumber = "Terraria " + Main.versionNumber + "\n" + version;
			Main.versionNumber2 = "Terraria " + Main.versionNumber2 + "\n" + version;

			Commands.Init();

			#region Config
			string configPath = Path.Combine("Raptor", "config.json");
			if (!File.Exists(configPath))
			{
				Config = new Config();
				File.WriteAllText(configPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
			}
			else
				Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));

			Main.mouseTextColorChange = 0;
			Main.showSplash = Config.ShowSplashScreen;
			#endregion

			Lua = new Lua();
			Lua.LoadCLRPackage();

			ClientApi.Main.Window.ClientSizeChanged += Window_ClientSizeChanged;
		}

		internal static void DrawPrePlayerChat(SpriteBatch sb)
		{
			var chatRectangle = new Rectangle(92, Main.screenHeight - 51 - Config.ChatShow * 21, Main.screenWidth - 312, Config.ChatShow * 21 + 12);
			if (Main.chatMode)
			{
				string text = Main.chatText;
				if ((chatBlinkTimer++) % 40 > 10)
					text += "|";

				sb.DrawGuiRectangle(new Rectangle(92, Main.screenHeight - 33, Main.screenWidth - 312, 28), new Color(200, 200, 200, 220));
				if (commandMode)
				{
					sb.DrawGuiText("Cmd:", new Vector2(46, Main.screenHeight - 30), Color.Orange, Main.fontMouseText);
					sb.DrawGuiText("/" + text, new Vector2(98, Main.screenHeight - 30), Color.Orange, Main.fontMouseText);
				}
				else
				{
					sb.DrawGuiText("Chat:", new Vector2(46, Main.screenHeight - 30), Color.White, Main.fontMouseText);
					sb.DrawGuiText(text, new Vector2(98, Main.screenHeight - 30), Color.White, Main.fontMouseText);
				}

				sb.DrawGuiRectangle(chatRectangle, new Color(200, 200, 200, 220));
			}

			int linesShown = 0;
			for (int i = -1; i + chatViewOffset >= 0 && linesShown < Config.ChatShow &&
				(Main.chatMode || chat[i + chatViewOffset].timeOut > 0); i--)
			{
				sb.DrawGuiText(chat[i + chatViewOffset].text,
					new Vector2(98, Main.screenHeight - 64 - linesShown++ * 21),
					chat[i + chatViewOffset].color,
					Main.fontMouseText);
			}

			if (Main.chatMode)
			{
				if (chat.Count > Config.ChatShow)
				{
					int scrollbarSize = (int)(Config.ChatShow * (Config.ChatShow * 21d - 4d) / chat.Count);
					int scrollbarOffset = (int)((chatViewOffset - Config.ChatShow) * (Config.ChatShow * 21d - 4d) / chat.Count);

					var scrollbar = new Rectangle(Main.screenWidth - 232, chatRectangle.Y + scrollbarOffset + 6, 6, scrollbarSize);
					sb.Draw(Main.inventoryBackTexture, scrollbar, new Rectangle(8, 8, 36, 36), new Color(20, 20, 20, 200));
				}
				if (chatRectangle.Contains(Input.MouseX, Input.MouseY))
				{
					sb.Draw(Main.cursorTexture,
						new Rectangle(Input.MouseX + 1, Input.MouseY + 1, (int)(Main.cursorScale * 15.4f), (int)(Main.cursorScale * 15.4f)),
						new Color((int)(Main.cursorColor.R * 0.2f), (int)(Main.cursorColor.G * 0.2f), (int)(Main.cursorColor.B * 0.2f), (int)(Main.cursorColor.A * 0.5f)));
					sb.Draw(Main.cursorTexture,
						new Rectangle(Input.MouseX, Input.MouseY, (int)(Main.cursorScale * 14.0f), (int)(Main.cursorScale * 14.0f)),
						Main.cursorColor);
				}
			}
		}
		internal static void NewText(string text, byte r, byte g, byte b)
		{
			rawChat.Add(new Chat { color = new Color(r, g, b), text = text });
			if (rawChat.Count > 1000)
				rawChat.RemoveAt(0);

			float length = 0f;
			var lineBuilder = new StringBuilder();
			float lineLength = 0f;
			int linesAdded = 0;
			float spaceLength = Main.fontMouseText.MeasureString(" ").X;

			foreach (string word in text.Split(' '))
			{
				length = Main.fontMouseText.MeasureString(word).X + spaceLength;
				lineLength += length;

				if (lineLength > Main.screenWidth - 338f)
				{
					chat.Add(new Chat { color = new Color(r, g, b), text = lineBuilder.ToString(), timeOut = 600 });
					lineLength = 4 * spaceLength + length;
					lineBuilder.Clear().Append("    " + word + " ");
					linesAdded++;
				}
				else
					lineBuilder.Append(word + " ");
			}
			if (lineBuilder.ToString() != "")
			{
				chat.Add(new Chat { color = new Color(r, g, b), text = lineBuilder.ToString(), timeOut = 600 });
				lineBuilder.Clear();
				linesAdded++;
			}

			chatViewOffset += linesAdded;
		}
		internal static void PreUpdate()
		{
			#region Chat handling
			if (((Input.IsKeyTapped(Keys.Enter) && Main.netMode == 0 && !Input.Alt) ||
				Input.IsKeyTapped(Keys.OemQuestion) && !Input.Shift)
				&& !Main.chatMode && !Main.editSign && !Main.gameMenu)
			{
				Main.chatMode = true;
				Main.chatRelease = false;
				Main.chatText = "";
				Main.keyCount = 0;
				Main.PlaySound(10);

				commandMode = Input.IsKeyTapped(Keys.OemQuestion);
				if (commandMode)
				{
					Input.TypedString = Input.TypedString.Replace("/", "");
				}
			}
			else if (Input.IsKeyTapped(Keys.Enter) && Main.chatMode)
			{
				if (Main.chatText == "")
				{
					Main.chatMode = false;
				}
				else if (commandMode)
				{
					typedCommands.Add(Main.chatText);
					if (typedCommands.Count > 1000)
					{
						typedCommands.RemoveAt(0);
					}
					typedCommandOffset = typedCommands.Count;
					Commands.Execute(Main.chatText);
				}
				else if (Main.netMode == 0)
				{
					typedChat.Add(Main.chatText);
					if (typedChat.Count > 1000)
						typedChat.RemoveAt(0);
					typedChatOffset = typedChat.Count;
					Main.NewText(String.Format("<{0}> {1}", Main.player[Main.myPlayer].name, Main.chatText));
				}
				else if (Main.netMode == 1)
				{
					typedChat.Add(Main.chatText);
					if (typedChat.Count > 1000)
						typedChat.RemoveAt(0);
					typedChatOffset = typedChat.Count;
					NetMessage.SendData(25, -1, -1, Main.chatText, Main.myPlayer);
				}

				Main.chatRelease = false;
				Main.chatText = "";
				Main.PlaySound(11);
				chatViewOffset = chat.Count;
			}
			#endregion
			#region Chat update
			if (Main.chatMode)
			{
				var chatRectangle = new Rectangle(96, Main.screenHeight - 49 - Config.ChatShow * 21, Main.screenWidth - 312, Config.ChatShow * 21 + 12);
				if (chatRectangle.Contains(Input.MouseX, Input.MouseY))
				{
					// Disable the mouse in Terraria.
					Main.gamePad = true;
					Main.mouseX = Main.mouseY = -100;

					Main.mouseState = Main.oldMouseState = Mouse.GetState();
					if (chat.Count > Config.ChatShow)
					{
						chatViewOffset += Math.Sign(Input.MouseScroll) * -Config.ChatScrollSpeed;
						chatViewOffset = (chatViewOffset < Config.ChatShow) ? Config.ChatShow : chatViewOffset;
						chatViewOffset = (chatViewOffset > chat.Count) ? chat.Count : chatViewOffset;
					}
				}

				if (Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.Up))
				{
					if (commandMode && typedCommands.Count != 0)
					{
						int prev = typedCommandOffset;

						typedCommandOffset--;
						typedCommandOffset = (typedCommandOffset < 0) ? 0 : typedCommandOffset;
						Main.chatText = typedCommands[typedCommandOffset];

						if (prev != typedCommandOffset)
							Main.PlaySound(12);
					}
					else if (!commandMode && typedChat.Count != 0)
					{
						int prev = typedChatOffset;

						typedChatOffset--;
						typedChatOffset = (typedChatOffset < 0) ? 0 : typedChatOffset;
						Main.chatText = typedChat[typedChatOffset];

						if (prev != typedChatOffset)
							Main.PlaySound(12);
					}
				}
				else if (Input.ActiveSpecialKeys.HasFlag(Input.SpecialKeys.Down))
				{
					if (commandMode && typedCommands.Count != 0)
					{
						int prev = typedCommandOffset;

						typedCommandOffset++;
						if (typedCommandOffset >= typedCommands.Count)
						{
							typedCommandOffset = typedCommands.Count;
							Main.chatText = "";
						}
						else
							Main.chatText = typedCommands[typedCommandOffset];

						if (prev != typedCommandOffset)
							Main.PlaySound(12);
					}
					else if (!commandMode && typedChat.Count != 0)
					{
						int prev = typedChatOffset;

						typedChatOffset++;
						if (typedChatOffset >= typedChat.Count)
						{
							typedChatOffset = typedChat.Count;
							Main.chatText = "";
						}
						else
							Main.chatText = typedChat[typedChatOffset];

						if (prev != typedChatOffset)
							Main.PlaySound(12);
					}
				}
			}
			else
			{
				typedChatOffset = typedChat.Count;
				typedCommandOffset = typedCommands.Count;
				commandMode = false;
				Main.gamePad = false;
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
			if (!Main.chatMode && !Main.editSign)
			{
				foreach (KeyValuePair<Keys, string> kvp in Config.KeyBindings)
				{
					if (Input.IsKeyTapped(kvp.Key))
					{
						Commands.Execute(kvp.Value);
					}
				}
			}
			#endregion
		}
		internal static void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			int width = ClientApi.Main.Window.ClientBounds.Width;

			if (width != Main.screenWidth)
			{
				chat.Clear();
				float length = 0f;
				float lineLength = 0f;
				var lineBuilder = new StringBuilder();
				float spaceLength = Main.fontMouseText.MeasureString(" ").X;

				for (int i = 0; i < rawChat.Count; i++)
				{
					foreach (string word in rawChat[i].text.Split(' '))
					{
						length = Main.fontMouseText.MeasureString(word).X + spaceLength;
						lineLength += length;

						if (lineLength > Main.screenWidth - 338f)
						{
							chat.Add(new Chat { color = rawChat[i].color, text = lineBuilder.ToString(), timeOut = rawChat[i].timeOut });
							lineLength = 4 * spaceLength + length;
							lineBuilder.Clear().Append("    " + word + " ");
						}
						else
							lineBuilder.Append(word + " ");
					}
					if (lineBuilder.ToString() != "")
					{
						chat.Add(new Chat { color = rawChat[i].color, text = lineBuilder.ToString(), timeOut = rawChat[i].timeOut });
						lineBuilder.Clear();
					}

					lineLength = 0f;
				}
				chatViewOffset = chat.Count;
			}
		}
	}
}
