using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using NLua;
using Raptor.Api;
using Raptor.Api.Commands;
using Raptor.Api.TShock;
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
		static string mouseText = "";
		internal static Texture2D rectBackTexture;

		class Chat
		{
			public Color color;
			public string text;
			public int timeOut;
		}
		[Flags]
		enum Resize
		{
			Left = 1,
			Right = 2,
			Up = 4,
			Down = 8
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

		internal static bool isEditingRegions;
		static List<Region> regions = new List<Region>();
		static List<Region> regionsToDraw = new List<Region>();

		static bool isCreatingRegion;
		static Point regionClickPt;
		static Point regionPt1;
		static Point regionPt2;
		static bool isNamingRegion;
		static string regionName = "";

		static Region selectedRegion;

		static bool regionMove;
		static Point regionMovePt;
		static Resize regionResize;
		static Rectangle regionResizeArea;

		internal static bool isEditingWarps;
		static List<Warp> warps = new List<Warp>();
		static List<Warp> warpsToDraw = new List<Warp>();

		static bool isCreatingWarp;
		static Point warpPt;
		static bool isNamingWarp;
		static string warpName = "";

		static bool warpMove;
		static Point warpMovePt;

		static Warp selectedWarp;

		/// <summary>
		/// Gets the configuration file.
		/// </summary>
		public static Config Config { get; internal set; }
		/// <summary>
		/// Gets the lua instance.
		/// </summary>
		public static Lua Lua { get; internal set; }
		static List<string> negatedPermissions = new List<string>();
		/// <summary>
		/// Gets the list of negated TShock permissions.
		/// </summary>
		public static ReadOnlyCollection<string> NegatedPermissions
		{
			get { return new ReadOnlyCollection<string>(negatedPermissions); }
		}
		static List<string> permissions = new List<string>();
		/// <summary>
		/// Gets the list of TShock permissions.
		/// </summary>
		public static ReadOnlyCollection<string> Permissions
		{
			get { return new ReadOnlyCollection<string>(permissions); }
		}

		internal static void DeInitialize()
		{
			Lua.Dispose();
		}
		internal static void Initialize()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Main.versionNumber = "Terraria " + Main.versionNumber + "\nRaptor v" + ClientApi.ApiVersion;

			Commands.Init();
			string configPath = Path.Combine("Raptor", "config.json");
			if (!File.Exists(configPath))
				File.WriteAllText(configPath, JsonConvert.SerializeObject(Config = new Config(), Formatting.Indented));
			else
				Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
			Lua = new Lua();
			Lua.LoadCLRPackage();

			Main.showSplash = Config.ShowSplashScreen;
			
			Form form = (Form)Form.FromHandle(ClientApi.Main.Window.Handle);
			form.ClientSizeChanged += Form_ClientSizeChanged;
			form.KeyPress += Input.Form_KeyPress;
		}

		internal static void Draw(SpriteBatch sb)
		{
			Main.mouseTextColor = 255;
			Main.mouseTextColorChange = 0;
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
		internal static void DrawInterface(SpriteBatch sb)
		{
			if (isEditingRegions)
			{
				foreach (Region r in regionsToDraw)
				{
					var rectangle = new Rectangle(
						r.Area.X * 16 - (int)Main.screenPosition.X, r.Area.Y * 16 - (int)Main.screenPosition.Y,
						r.Area.Width * 16, r.Area.Height * 16);
					sb.DrawGuiRectangle(rectangle, r != selectedRegion ? new Color(30, 30, 30, 125) : new Color(30, 30, 200, 125));
				}
				if (isCreatingRegion || isNamingRegion)
				{
					var selection = new Rectangle(
						regionPt1.X * 16 - (int)Main.screenPosition.X, regionPt1.Y * 16 - (int)Main.screenPosition.Y,
						(regionPt2.X - regionPt1.X + 1) * 16, (regionPt2.Y - regionPt1.Y + 1) * 16);
					sb.DrawGuiRectangle(selection, new Color(200, 30, 30, 125));
				}
			}
			else if (isEditingWarps)
			{
				foreach (Warp w in warpsToDraw)
				{
					var rectangle = new Rectangle(
						w.Position.X * 16 - (int)Main.screenPosition.X, w.Position.Y * 16 - (int)Main.screenPosition.Y,
						32, 48);
					sb.DrawGuiRectangle(rectangle, w != selectedWarp ? new Color(30, 30, 30, 150) : new Color(30, 30, 200, 125));
				}
				if (isCreatingWarp || isNamingWarp)
				{
					var selection = new Rectangle(
						warpPt.X * 16 - (int)Main.screenPosition.X, warpPt.Y * 16 - (int)Main.screenPosition.Y,
						32, 48);
					sb.DrawGuiRectangle(selection, new Color(200, 30, 30, 125));
				}
			}

			sb.DrawGuiMouseText(mouseText, Color.White);
			mouseText = "";
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

				sb.DrawGuiRectangle(
					chatRectangle,
					new Color(100, 100, 100, 200),
					Main.inventoryBackTexture);
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
					new Color(20, 20, 20, 200));
			}
		}
		internal static void LoadedContent(ContentManager content)
		{
			string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Raptor");

			Main.cursorTexture = cursorTextures[0] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "Normal"));
			cursorTextures[1] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "Move"));
			cursorTextures[2] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "HorzResize"));
			cursorTextures[3] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "VertResize"));
			cursorTextures[4] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "DiagResize"));
			// it's easier to do this than to just flip the texture
			cursorTextures[5] = content.Load<Texture2D>(Path.Combine(dir, "Cursors", "DiagResize2"));

			Main.fontItemStack = Main.fontMouseText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Regular"));
			Main.fontDeathText = content.Load<SpriteFont>(Path.Combine(dir, "Fonts", "Title"));

			Texture2D invBack = content.Load<Texture2D>(Path.Combine(dir, "UI", "InvBack"));
			Main.inventoryBackTexture = invBack;
			for (int i = 2; i <= 12; i++)
				typeof(Main).GetField("inventoryBack" + i + "Texture").SetValue(null, invBack);

			Main.chatBackTexture = content.Load<Texture2D>(Path.Combine(dir, "UI", "NpcChatBack"));

			rectBackTexture = content.Load<Texture2D>(Path.Combine(dir, "UI", "RectBack"));
		}
		internal static void NewText(string text, byte r, byte g, byte b)
		{
			if (Config.LogChat)
				Log.LogInfo("Chat: {0}", text);

			rawChat.Add(new Chat { color = new Color(r, g, b), text = text });
			if (rawChat.Count > 1000)
				rawChat.RemoveAt(0);

			var lineBuilder = new StringBuilder();
			float lineLength = 0f;
			int linesAdded = 0;
			float spaceLength = Main.fontMouseText.MeasureString(" ").X;

			foreach (string word in text.Split(' '))
			{
				float length = Main.fontMouseText.MeasureString(word).X + spaceLength;
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
				}
				chatMode = 0;
				chatViewOffset = 0;
				Main.chatText = "";

				isEditingRegions = false;
				isEditingWarps = false;
				permissions.Clear();
				negatedPermissions.Clear();
				regions.Clear();
				regionsToDraw.Clear();
				return;
			}

			#region Region editing
			if (isEditingRegions)
			{
				regionsToDraw.Clear();

				if (Input.MouseLeftClick)
					selectedRegion = null;
				if (!Input.MouseLeftDown)
				{
					if (regionMove)
					{
						Utils.SendRegion(selectedRegion);
						Log.LogInfo("Moved region \"{0}\" to ({1}, {2}).",
							selectedRegion.Name, selectedRegion.Area.X, selectedRegion.Area.Y);
						regionMove = false;
					}
					else if (regionResize > 0)
					{
						Utils.SendRegion(selectedRegion);
						Log.LogInfo("Resized region \"{0}\" to ({1}, {2}).",
							selectedRegion.Name, selectedRegion.Area.Width, selectedRegion.Area.Height);
						regionResize = 0;
					}
				}

				#region Drawing, moving, & resizing
				var screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
				for (int i = 0; i < regions.Count; i++)
				{
					Region r = regions[i];

					var rectangle = new Rectangle(
						r.Area.X * 16 - (int)Main.screenPosition.X, r.Area.Y * 16 - (int)Main.screenPosition.Y,
						r.Area.Width * 16, r.Area.Height * 16);

					if (rectangle.Intersects(screen))
					{
						regionsToDraw.Add(r);

						if (regionClickPt != Point.Zero)
							continue;

						#region Resizing Checks
						bool isLeft = new Rectangle(rectangle.Left, rectangle.Top, 8, rectangle.Height).Contains(Input.MouseX, Input.MouseY);
						bool isRight = new Rectangle(rectangle.Right - 8, rectangle.Top, 8, rectangle.Height).Contains(Input.MouseX, Input.MouseY);
						bool isUp = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 8).Contains(Input.MouseX, Input.MouseY);
						bool isDown = new Rectangle(rectangle.Left, rectangle.Bottom - 8, rectangle.Width, 8).Contains(Input.MouseX, Input.MouseY);
						
						if (isLeft || isRight)
						{
							Input.CursorType = 2;
							Input.DisabledMouse = true;

							if (Input.MouseLeftClick)
							{
								regionResize |= isLeft ? Resize.Left : Resize.Right;
								regionResizeArea = r.Area;
								selectedRegion = r;
							}
						}
						if (isUp || isDown)
						{
							Input.CursorType = Input.CursorType == 2 ? 4 : 3;
							if ((isUp && isLeft) || (isDown && isRight))
								Input.CursorType++;
							Input.DisabledMouse = true;

							if (Input.MouseLeftClick)
							{
								regionResize |= isUp ? Resize.Up : Resize.Down;
								regionResizeArea = r.Area;
								selectedRegion = r;
							}
						}
						#endregion

						if (rectangle.Contains(Input.MouseX, Input.MouseY) && !isLeft && !isRight && !isUp && !isDown)
						{
							mouseText = "Region name: " + r.Name;
							Input.CursorType = 1;
							Input.DisabledMouse = true;

							if (Input.MouseLeftClick)
							{
								regionMove = true;
								regionMovePt = new Point(Input.MouseX - rectangle.X, Input.MouseY - rectangle.Y);
								selectedRegion = r;
							}
						}
					}
				}
				#endregion
				#region Resizing
				if (regionResize != 0)
				{
					bool isLeft = regionResize.HasFlag(Resize.Left);
					bool isRight = regionResize.HasFlag(Resize.Right);
					bool isUp = regionResize.HasFlag(Resize.Up);
					bool isDown = regionResize.HasFlag(Resize.Down);
					mouseText = "Region name: " + selectedRegion.Name;
					Input.DisabledMouse = true;

					if (isLeft)
					{
						Input.CursorType = 2;

						int x = (int)Math.Round((Input.MouseX + Main.screenPosition.X) / 16);
						int width = regionResizeArea.X - x + regionResizeArea.Width;

						if (width <= 0)
						{
							width = 1;
							x = regionResizeArea.Right - 1;
						}

						selectedRegion.Area.X = x;
						selectedRegion.Area.Width = width;
					}
					if (isRight)
					{
						Input.CursorType = 2;

						int width = (int)Math.Round((Input.MouseX + Main.screenPosition.X) / 16) - regionResizeArea.X;
						if (width <= 0)
							width = 1;

						selectedRegion.Area.Width = width;
					}
					if (isUp)
					{
						Input.CursorType = Input.CursorType == 2 ? 4 : 3;
						if (isLeft)
							Input.CursorType++;

						int y = (int)Math.Round((Input.MouseY + Main.screenPosition.Y) / 16);
						int height = regionResizeArea.Y - y + regionResizeArea.Height;

						if (height <= 0)
						{
							height = 1;
							y = regionResizeArea.Bottom - 1;
						}

						selectedRegion.Area.Y = y;
						selectedRegion.Area.Height = height;
					}
					if (isDown)
					{
						Input.CursorType = Input.CursorType == 2 ? 4 : 3;
						if (isRight)
							Input.CursorType++;

						int height = (int)Math.Round((Input.MouseY + Main.screenPosition.Y) / 16) - regionResizeArea.Y;
						if (height <= 0)
							height = 1;

						selectedRegion.Area.Height = height;
					}
				}
				#endregion
				#region Moving
				if (regionMove)
				{
					mouseText = "Region name: " + selectedRegion.Name;
					Input.CursorType = 1;
					Input.DisabledMouse = true;

					selectedRegion.Area.X = (int)Math.Round((Main.screenPosition.X + Input.MouseX - regionMovePt.X) / 16.0);
					selectedRegion.Area.Y = (int)Math.Round((Main.screenPosition.Y + Input.MouseY - regionMovePt.Y) / 16.0);
				}
				#endregion
				#region Creating
				if (Input.MouseRightClick && regionClickPt == Point.Zero)
				{
					Input.DisabledMouse = true;

					isCreatingRegion = true;
					regionClickPt.X = (int)(Main.screenPosition.X + Input.MouseX) / 16;
					regionClickPt.Y = (int)(Main.screenPosition.Y + Input.MouseY) / 16;
					selectedRegion = null;
				}

				if (isCreatingRegion)
				{
					Input.DisabledMouse = true;

					if (Input.MouseRightDown && regionClickPt != Point.Zero)
					{
						int X = (int)(Main.screenPosition.X + Input.MouseX) / 16;
						int Y = (int)(Main.screenPosition.Y + Input.MouseY) / 16;
						if (X < regionClickPt.X)
						{
							regionPt1.X = X;
							regionPt2.X = regionClickPt.X;
						}
						else
						{
							regionPt1.X = regionClickPt.X;
							regionPt2.X = X;
						}
						if (Y < regionClickPt.Y)
						{
							regionPt1.Y = Y;
							regionPt2.Y = regionClickPt.Y;
						}
						else
						{
							regionPt1.Y = regionClickPt.Y;
							regionPt2.Y = Y;
						}
					}
					if (Input.MouseRightRelease)
					{
						isCreatingRegion = false;
						isNamingRegion = true;
						regionName = "";
						Main.PlaySound(10);
					}
				}
				#endregion
				#region Naming
				else if (isNamingRegion)
				{
					var rectangle = new Rectangle(
						regionPt1.X * 16 - (int)Main.screenPosition.X,
						regionPt1.Y * 16 - (int)Main.screenPosition.Y,
						(regionPt2.X - regionPt1.X + 1) * 16,
						(regionPt2.Y - regionPt1.Y + 1) * 16);
					if (rectangle.Contains(Input.MouseX, Input.MouseY))
					{
						Input.DisabledKeyboard = true;
						Input.DisabledMouse = true;

						regionName = Input.GetInputText(regionName);
						mouseText = "Region name: " + regionName + (textBlinkTimer % 40 > 10 ? "|" : "");
					}

					if (Input.IsKeyTapped(Keys.Enter) && regionName != "")
					{
						if (regions.Any(r => String.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase)))
							Utils.NewErrorText("Invalid region name.");
						else
						{
							Region region = new Region();
							region.Area = new Rectangle(regionPt1.X, regionPt1.Y, regionPt2.X - regionPt1.X + 1, regionPt2.Y - regionPt1.Y + 1);
							region.Name = regionName;
							regions.Add(region);
							regionsToDraw.Add(region);
							Utils.SendRegion(region);
							Log.LogInfo("Created new region \"{0}\" at ({1}, {2}) with size ({3}, {4})",
								regionName, region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height);

							isNamingRegion = false;
							regionClickPt = regionPt1 = regionPt2 = Point.Zero;
						}

						Main.PlaySound(11);
					}
					else if (Input.IsKeyTapped(Keys.Escape))
					{
						isNamingRegion = false;
						regionClickPt = regionPt1 = regionPt2 = Point.Zero;
						Main.PlaySound(11);
					}
				}
				#endregion
				#region Deleting
				if (selectedRegion != null && Input.IsKeyTapped(Keys.Delete))
				{
					Utils.SendRegionDelete(selectedRegion);
					regions.Remove(selectedRegion);
					regionsToDraw.Remove(selectedRegion);
					Log.LogInfo("Deleted region \"{0}\".", selectedRegion);

					Main.PlaySound(11);
				}
				#endregion
			}
			#endregion
			#region Warp editing
			else if (isEditingWarps)
			{
				warpsToDraw.Clear();

				if (Input.MouseLeftClick)
					selectedWarp = null;
				if (!Input.MouseLeftDown)
				{
					if (warpMove)
					{
						Utils.SendWarp(selectedWarp);
						Log.LogInfo("Moved warp \"{0}\" to ({1}, {2}).",
							selectedWarp.Name, selectedWarp.Position.X, selectedWarp.Position.Y);
						warpMove = false;
					}
				}

				#region Drawing & moving
				var screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
				for (int i = 0; i < warps.Count; i++)
				{
					Warp w = warps[i];

					var rectangle = new Rectangle(
						w.Position.X * 16 - (int)Main.screenPosition.X, w.Position.Y * 16 - (int)Main.screenPosition.Y,
						32, 48);

					if (rectangle.Intersects(screen))
					{
						warpsToDraw.Add(w);

						if (rectangle.Contains(Input.MouseX, Input.MouseY))
						{
							mouseText = "Warp name: " + w.Name;
							Input.CursorType = 1;
							Input.DisabledMouse = true;

							if (Input.MouseLeftClick)
							{
								warpMove = true;
								warpMovePt = new Point(Input.MouseX - rectangle.X, Input.MouseY - rectangle.Y);
								selectedWarp = w;
							}
						}
					}
				}
				#endregion
				#region Moving
				if (warpMove)
				{
					mouseText = "Warp name: " + selectedWarp.Name;
					Input.CursorType = 1;
					Input.DisabledMouse = true;

					selectedWarp.Position.X = (int)Math.Round((Main.screenPosition.X + Input.MouseX - warpMovePt.X) / 16.0);
					selectedWarp.Position.Y = (int)Math.Round((Main.screenPosition.Y + Input.MouseY - warpMovePt.Y) / 16.0);
				}
				#endregion
				#region Creating
				if (Input.MouseRightClick && warpPt == Point.Zero)
				{
					Input.DisabledMouse = true;

					isCreatingWarp = true;
					selectedWarp = null;
					warpPt.X = (int)(Main.screenPosition.X + Input.MouseX) / 16;
					warpPt.Y = (int)(Main.screenPosition.Y + Input.MouseY) / 16;
				}

				if (isCreatingWarp)
				{
					Input.DisabledMouse = true;

					if (Input.MouseRightDown && warpPt != Point.Zero)
					{
						warpPt.X = (int)(Main.screenPosition.X + Input.MouseX) / 16;
						warpPt.Y = (int)(Main.screenPosition.Y + Input.MouseY) / 16;
					}
					if (Input.MouseRightRelease)
					{
						isCreatingWarp = false;
						isNamingWarp = true;
						warpName = "";
						Main.PlaySound(10);
					}
				}
				#endregion
				#region Naming
				else if (isNamingWarp)
				{
					var rectangle = new Rectangle(
						warpPt.X * 16 - (int)Main.screenPosition.X, warpPt.Y * 16 - (int)Main.screenPosition.Y,
						32, 48);
					if (rectangle.Contains(Input.MouseX, Input.MouseY))
					{
						Input.DisabledKeyboard = true;
						Input.DisabledMouse = true;

						warpName = Input.GetInputText(warpName);
						mouseText = "Warp name: " + warpName + (textBlinkTimer % 40 > 10 ? "|" : "");
					}

					if (Input.IsKeyTapped(Keys.Enter) && warpName != "")
					{
						if (warps.Any(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase)))
							Utils.NewErrorText("Invalid warp name.");
						else
						{
							var warp = new Warp();
							warp.Name = warpName;
							warp.Position = warpPt;
							warps.Add(warp);
							warpsToDraw.Add(warp);
							Utils.SendWarp(warp);
							Log.LogInfo("Created new warp \"{0}\" at ({1}, {2}).", warpName, warpPt.X, warpPt.Y);

							isNamingWarp = false;
							warpPt = Point.Zero;
						}

						Main.PlaySound(11);
					}
					else if (Input.IsKeyTapped(Keys.Escape))
					{
						isNamingWarp = false;
						warpPt = Point.Zero;
						Main.PlaySound(11);
					}
				}
				#endregion
				#region Deleting
				if (selectedWarp != null && Input.IsKeyTapped(Keys.Delete))
				{
					Utils.SendWarpDelete(selectedWarp);
					warps.Remove(selectedWarp);
					warpsToDraw.Remove(selectedWarp);
					Log.LogInfo("Deleted warp \"{0}\".", selectedWarp.Name);

					Main.PlaySound(11);
				}
				#endregion
			}
			#endregion

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

				Input.DisabledKeyboard = true;
				Main.chatText = Main.GetInputText(Main.chatText);

				if (Input.IsKeyTapped(Keys.Escape))
				{
					chatMode = 0;
					chatViewOffset = chat.Count;
					typedChatOffset = typedChat.Count;
					typedCommandOffset = typedCommands.Count;

					Main.chatText = "";
					Main.PlaySound(11);
				}
				else if (Input.IsKeyTapped(Keys.Enter) && Main.chatText != "")
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
			#region Key bindings
			if (chatMode == 0 && !Main.editSign && !Main.gameMenu && !Input.DisabledKeyboard)
			{
				foreach (KeyValuePair<Keys, string> kvp in Config.KeyBindings)
				{
					if (Input.IsKeyTapped(kvp.Key))
						Commands.Execute(kvp.Value);
				}
			}
			#endregion

			if (Input.DisabledMouse)
				Main.mouseState = Main.oldMouseState = new MouseState(-100, -100, Input.MouseScroll, 0, 0, 0, 0, 0);
			if (Input.DisabledKeyboard)
				Main.keyState = Main.inputText = Main.oldInputText = new KeyboardState(null);
		}

		internal static bool GetData(int index, int length)
		{
			if (NetMessage.buffer[256].readBuffer[index] != (byte)PacketTypes.Raptor)
				return false;

			using (var reader = new BinaryReader(new MemoryStream(NetMessage.buffer[256].readBuffer, index + 1, length - 1)))
			{
				switch ((RaptorPacketTypes)reader.ReadByte())
				{
					case RaptorPacketTypes.Permissions:
						negatedPermissions.Clear();
						permissions.Clear();

						foreach (string p in reader.ReadString().Split(','))
						{
							if (p.StartsWith("!"))
								negatedPermissions.Add(p.Substring(1));
							else
								permissions.Add(p);
						}
						return true;
					case RaptorPacketTypes.Region:
						{
							var area = new Rectangle(
								reader.ReadInt32(), reader.ReadInt32(),
								reader.ReadInt32() + 1, reader.ReadInt32() + 1);
							string regionName = reader.ReadString();

							Region region = regions.Find(r => String.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase));

							if (region == null)
								regions.Add(new Region { Area = area, Name = regionName });
							else
								region.Area = area;
						}
						return true;
					case RaptorPacketTypes.RegionDelete:
						{
							string regionName = reader.ReadString();
							regions.RemoveAll(r => r.Name == regionName);
						}
						return true;
					case RaptorPacketTypes.Warp:
						{
							var position = new Point(reader.ReadInt32(), reader.ReadInt32());
							string warpName = reader.ReadString();
							Warp warp = warps.Find(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase));

							if (warp == null)
								warps.Add(new Warp { Position = position, Name = warpName });
							else
								warp.Position = position;
						}
						return true;
					case RaptorPacketTypes.WarpDelete:
						{
							string warpName = reader.ReadString();
							warps.RemoveAll(r => r.Name == warpName);
						}
						return true;
					default:
						return true;
				}
			}
		}
		internal static void GotData(int index, int length)
		{
			if (NetMessage.buffer[256].readBuffer[index] == (byte)PacketTypes.FirstSpawn)
				Utils.SendAcknowledge();
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
				}
				chatViewOffset = chat.Count;
			}
		}
	}
}
