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
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using Terraria;

using Clipboard = System.Windows.Forms.Clipboard;

namespace Raptor
{
	/// <summary>
	/// Manages input.
	/// </summary>
	public static class Input
	{
		/// <summary>
		/// Represents special keys.
		/// </summary>
		[Flags]
		public enum SpecialKeys
		{
			Up = 0x01,
			Down = 0x02,
			Backspace = 0x04,
			V = 0x08,
			Enter = 0x10,
		}

		static List<char> charCodes = new List<char>();
		static List<byte> vkCodes = new List<byte>();
		
		static KeyboardState LastKeyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
		static MouseState LastMouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
		static KeyboardState Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
		static MouseState Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

		/// <summary>
		/// Gets the active special keys.
		/// </summary>
		public static SpecialKeys ActiveSpecialKeys { get; private set; }
		/// <summary>
		/// Gets if an alt key is down.
		/// </summary>
		public static bool Alt
		{
			get { return Keyboard.IsKeyDown(Keys.LeftAlt) || Keyboard.IsKeyDown(Keys.RightAlt); }
		}
		internal static int CursorType { get; set; }
		/// <summary>
		/// Gets if a control key is down.
		/// </summary>
		public static bool Control
		{
			get { return Keyboard.IsKeyDown(Keys.LeftControl) || Keyboard.IsKeyDown(Keys.RightControl); }
		}
		/// <summary>
		/// Whether or not the keyboard for Terraria should be disabled.
		/// </summary>
		public static bool DisabledKeyboard;
		/// <summary>
		/// Whether or not the mouse for Terraria should be disabled.
		/// </summary>
		public static bool DisabledMouse;
		/// <summary>
		/// Gets the mouse delta scroll wheel value.
		/// </summary>
		public static int MouseDScroll
		{
			get { return Mouse.ScrollWheelValue - LastMouse.ScrollWheelValue; }
		}
		/// <summary>
		/// Gets the mouse delta X position.
		/// </summary>
		public static int MouseDX
		{
			get { return Mouse.X - LastMouse.X; }
		}
		/// <summary>
		/// Gets the mouse delta Y position.
		/// </summary>
		public static int MouseDY
		{
			get { return Mouse.Y - LastMouse.Y; }
		}
		/// <summary>
		/// Gets if the LMB is clicked.
		/// </summary>
		public static bool MouseLeftClick
		{
			get { return Mouse.LeftButton == ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Released; }
		}
		/// <summary>
		/// Gets if the LMB is pressed.
		/// </summary>
		public static bool MouseLeftDown
		{
			get { return Mouse.LeftButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the LMB is released.
		/// </summary>
		public static bool MouseLeftRelease
		{
			get { return Mouse.LeftButton != ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the RMB is clicked.
		/// </summary>
		public static bool MouseRightClick
		{
			get { return Mouse.RightButton == ButtonState.Pressed && LastMouse.RightButton == ButtonState.Released; }
		}
		/// <summary>
		/// Gets if the RMB is pressed.
		/// </summary>
		public static bool MouseRightDown
		{
			get { return Mouse.RightButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets if the RMB is released.
		/// </summary>
		public static bool MouseRightRelease
		{
			get { return Mouse.RightButton != ButtonState.Pressed && LastMouse.RightButton == ButtonState.Pressed; }
		}
		/// <summary>
		/// Gets the mouse scroll wheel value.
		/// </summary>
		public static int MouseScroll
		{
			get { return Mouse.ScrollWheelValue; }
		}
		/// <summary>
		/// Gets the mouse X position.
		/// </summary>
		public static int MouseX
		{
			get { return Mouse.X; }
		}
		/// <summary>
		/// Gets the mouse Y position.
		/// </summary>
		public static int MouseY
		{
			get { return Mouse.Y; }
		}
		/// <summary>
		/// Gets if a shift key is down.
		/// </summary>
		public static bool Shift
		{
			get { return Keyboard.IsKeyDown(Keys.LeftShift) || Keyboard.IsKeyDown(Keys.RightShift); }
		}
		/// <summary>
		/// Gets the string formed from the most recent WM_CHAR messages.
		/// </summary>
		public static string TypedString { get; private set; }

		internal static void FilterMessage(ref System.Windows.Forms.Message m)
		{
			// WM_KEYDOWN
			if (m.Msg == 0x100)
			{
				vkCodes.Add((byte)m.WParam);

				// Translate message
				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(m));
				Marshal.StructureToPtr(m, ptr, true);
				keyBoardInput.TranslateMessage(ptr);
			}
		}
		internal static void Form_KeyPress(object o, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar >= 32 && e.KeyChar != 127)
				charCodes.Add(e.KeyChar);
		}
		internal static string GetInputText(string text)
		{
			if (!Main.hasFocus)
				return text;

			Main.inputTextEnter = ActiveSpecialKeys.HasFlag(SpecialKeys.Enter);
			
			if (ActiveSpecialKeys.HasFlag(SpecialKeys.Backspace) && text.Length != 0)
			{
				if (Control)
				{
					string[] words = text.Split(' ');
					return String.Join(" ", words, 0, words.Length - 1);
				}
				else
					return text.Substring(0, text.Length - 1);
			}
			else if (Control && ActiveSpecialKeys.HasFlag(SpecialKeys.V) && Clipboard.ContainsText())
				return text + Clipboard.GetText();
			else
				return text + TypedString;
		}
		/// <summary>
		/// Gets if a key is down.
		/// </summary>
		public static bool IsKeyDown(Keys key)
		{
			return Keyboard.IsKeyDown(key);
		}
		/// <summary>
		/// Gets if a key was released; that is, if the key is currently depressed but was pressed before.
		/// </summary>
		public static bool IsKeyReleased(Keys key)
		{
			return Keyboard.IsKeyUp(key) && LastKeyboard.IsKeyDown(key);
		}
		/// <summary>
		/// Gets if a key was tapped; that is, if the key is currently pressed but was depressed before.
		/// </summary>
		public static bool IsKeyTapped(Keys key)
		{
			return Keyboard.IsKeyDown(key) && LastKeyboard.IsKeyUp(key);
		}
		internal static void Update()
		{
			LastKeyboard = Keyboard;
			Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			LastMouse = Mouse;
			Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

			TypedString = "";
			for (int i = 0; i < charCodes.Count; i++)
				TypedString += charCodes[i];
			charCodes.Clear();

			ActiveSpecialKeys = 0;
			for (int i = 0; i < vkCodes.Count; i++)
			{
				switch (vkCodes[i])
				{
					case 0x08:
						ActiveSpecialKeys |= SpecialKeys.Backspace;
						break;
					case 0x0D:
						ActiveSpecialKeys |= SpecialKeys.Enter;
						break;
					case 0x26:
						ActiveSpecialKeys |= SpecialKeys.Up;
						break;
					case 0x28:
						ActiveSpecialKeys |= SpecialKeys.Down;
						break;
					case 0x56:
						ActiveSpecialKeys |= SpecialKeys.V;
						break;
				}
			}
			vkCodes.Clear();
		}
	}
}