//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
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
		internal static Texture2D rectBackTexture;

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
			form.KeyPress += Input.Form_KeyPress;

			var state = form.WindowState;
			form.MinimumSize = new System.Drawing.Size(816, 638); 
			form.WindowState = state;
		}

		internal static void LoadedContent(ContentManager content)
		{
			string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Raptor");

			Main.fontItemStack = Main.fontMouseText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Regular"));
			Main.fontDeathText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Title"));

			var invBack = content.Load<Texture2D>(Path.Combine(dir, "UI", "InvBack"));
			Main.inventoryBackTexture = invBack;
			for (int i = 2; i <= 14; i++)
				typeof(Main).GetField("inventoryBack" + i + "Texture").SetValue(null, invBack);
			Main.chatBackTexture = content.Load<Texture2D>(Path.Combine(dir, "UI", "NpcChatBack"));
			rectBackTexture = content.Load<Texture2D>(Path.Combine(dir, "UI", "RectBack"));
		}
		internal static void Update()
		{
			if (!Main.hasFocus)
				return;

			Input.DisabledMouse = false;
			Input.DisabledKeyboard = false;
			Input.Update();

		    Main.chatRelease = false;

		    if (Input.IsKeyTapped(Keys.Enter) && !Input.Alt)
		    {
		        if (Main.chatMode)
		        {
		            if (Main.chatText.StartsWith("."))
		                Commands.Execute(Main.chatText.Substring(1));
		            else
		            {
		                if (Main.netMode == 0)
		                    Main.NewText(String.Format("<{0}> {1}", Utils.LocalPlayer.name, Main.chatText));
		                else
		                    NetMessage.SendData((int)PacketTypes.Chat, -1, -1, Main.chatText);
		            }

		            Main.chatMode = false;
		            Main.chatText = "";
		            Main.PlaySound(11);
		        }
		        else
		        {
		            Main.chatMode = true;
		            Main.PlaySound(10);
		        }
		    }

			#region Keybinds
			if (!Main.chatMode && !Main.editSign && !Main.gameMenu && !Input.DisabledKeyboard)
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
	}
}
