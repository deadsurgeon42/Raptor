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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using Terraria;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Raptor
{
	/// <summary>
	///   Manages input.
	/// </summary>
	public static class Input
	{
		/// <summary>
		///   Represents special keys.
		/// </summary>
		[Flags]
		public enum SpecialKeys
		{
			Up = 0x01,
			Down = 0x02,
			Backspace = 0x04,
			V = 0x08,
			Enter = 0x10
		}

		private static readonly List<char> charCodes = new List<char>();
		private static readonly List<byte> keyCodes = new List<byte>();

		private static KeyboardState lastKeyboard = Keyboard.GetState();
		private static MouseState lastMouse = Mouse.GetState();
		private static KeyboardState keyboard = Keyboard.GetState();
		private static MouseState mouse = Mouse.GetState();

		/// <summary>
		///   Whether or not the keyboard for Terraria should be disabled.
		/// </summary>
		public static bool DisabledKeyboard;

		/// <summary>
		///   Whether or not the mouse for Terraria should be disabled.
		/// </summary>
		public static bool DisabledMouse;

		/// <summary>
		///   Gets the active special keys.
		/// </summary>
		public static SpecialKeys ActiveSpecialKeys { get; private set; }

		/// <summary>
		///   Gets whether an alt key is down.
		/// </summary>
		public static bool Alt => keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);

		/// <summary>
		///   Gets whether a control key is down.
		/// </summary>
		public static bool Control => keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);

		/// <summary>
		///   Gets the mouse delta scroll wheel value.
		/// </summary>
		public static int MouseDScroll => mouse.ScrollWheelValue - lastMouse.ScrollWheelValue;

		/// <summary>
		///   Gets the mouse delta X position.
		/// </summary>
		public static int MouseDX => mouse.X - lastMouse.X;

		/// <summary>
		///   Gets the mouse delta Y position.
		/// </summary>
		public static int MouseDY => mouse.Y - lastMouse.Y;

		/// <summary>
		///   Gets if the LMB is clicked.
		/// </summary>
		public static bool MouseLeftClick
			=> mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released;

		/// <summary>
		///   Gets if the LMB is pressed.
		/// </summary>
		public static bool MouseLeftDown => mouse.LeftButton == ButtonState.Pressed;

		/// <summary>
		///   Gets if the LMB is released.
		/// </summary>
		public static bool MouseLeftRelease
			=> mouse.LeftButton != ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Pressed;

		/// <summary>
		///   Gets if the RMB is clicked.
		/// </summary>
		public static bool MouseRightClick
			=> mouse.RightButton == ButtonState.Pressed && lastMouse.RightButton == ButtonState.Released;

		/// <summary>
		///   Gets if the RMB is pressed.
		/// </summary>
		public static bool MouseRightDown => mouse.RightButton == ButtonState.Pressed;

		/// <summary>
		///   Gets if the RMB is released.
		/// </summary>
		public static bool MouseRightRelease
			=> mouse.RightButton != ButtonState.Pressed && lastMouse.RightButton == ButtonState.Pressed;

		/// <summary>
		///   Gets the mouse scroll wheel value.
		/// </summary>
		public static int MouseScroll => mouse.ScrollWheelValue;

		/// <summary>
		///   Gets the mouse X position.
		/// </summary>
		public static int MouseX => mouse.X;

		/// <summary>
		///   Gets the mouse Y position.
		/// </summary>
		public static int MouseY => mouse.Y;

		/// <summary>
		///   Gets if a shift key is down.
		/// </summary>
		public static bool Shift => keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

		/// <summary>
		///   Gets the string formed from the most recent WM_CHAR messages.
		/// </summary>
		public static string TypedString { get; private set; }

		internal static void FilterMessage(ref Message m)
		{
			// WM_KEYDOWN
			if (m.Msg == 0x100)
			{
				keyCodes.Add((byte) m.WParam);

				// Translate message
				var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(m));
				Marshal.StructureToPtr(m, ptr, true);
				keyBoardInput.TranslateMessage(ptr);
			}
		}

		internal static void Form_KeyPress(object o, KeyPressEventArgs e)
		{
			if (!e.Handled && e.KeyChar >= 32 && e.KeyChar != 127)
				charCodes.Add(e.KeyChar);
		}

		internal static string GetInputText(string text)
		{
			if (!Main.hasFocus)
				return text;

			Main.inputTextEnter = ActiveSpecialKeys.HasFlag(SpecialKeys.Enter);

			if (ActiveSpecialKeys.HasFlag(SpecialKeys.Backspace) && text.Length != 0)
				if (Control)
				{
					var words = text.Split(' ');
					return string.Join(" ", words, 0, words.Length - 1);
				}
				else
				{
					return text.Substring(0, text.Length - 1);
				}
			if (Control && ActiveSpecialKeys.HasFlag(SpecialKeys.V) && Clipboard.ContainsText())
				return text + Clipboard.GetText();
			return text + TypedString;
		}

		/// <summary>
		///   Gets if a key is down.
		/// </summary>
		public static bool IsKeyDown(Keys key)
		{
			return keyboard.IsKeyDown(key);
		}

		/// <summary>
		///   Gets if a key was released; that is, if the key is currently depressed but was pressed before.
		/// </summary>
		public static bool IsKeyReleased(Keys key)
		{
			return keyboard.IsKeyUp(key) && lastKeyboard.IsKeyDown(key);
		}

		/// <summary>
		///   Gets if a key was tapped; that is, if the key is currently pressed but was depressed before.
		/// </summary>
		public static bool IsKeyTapped(Keys key)
		{
			return keyboard.IsKeyDown(key) && lastKeyboard.IsKeyUp(key);
		}

		/// <summary>
		///   Gets all keys that are currently pressed down on the keyboard.
		/// </summary>
		public static Keys[] GetPressedKeys()
		{
			return keyboard.GetPressedKeys();
		}

		internal static void Update()
		{
			lastKeyboard = keyboard;
			keyboard = Keyboard.GetState();
			lastMouse = mouse;
			mouse = Mouse.GetState();

			TypedString = "";
			foreach (char c in charCodes)
				TypedString += c;
			charCodes.Clear();

			ActiveSpecialKeys = 0;

			foreach (byte t in keyCodes)
				switch (t)
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
			keyCodes.Clear();
		}

		/// <summary>
		///   A key combination.
		/// </summary>
		public struct Keybind
		{
			/// <summary>
			///   Whether the Alt key must be held.
			/// </summary>
			public bool Alt;

			/// <summary>
			///   Whether the Ctrl key must be held.
			/// </summary>
			public bool Control;

			/// <summary>
			///   The key.
			/// </summary>
			public Keys Key;

			/// <summary>
			///   Whether the Shift key must be held.
			/// </summary>
			public bool Shift;

			public override bool Equals(object obj)
			{
				return obj is Keybind && (Keybind) obj == this;
			}

			public override int GetHashCode()
			{
				return (int) Key;
			}

			public static bool operator ==(Keybind kb1, Keybind kb2)
			{
				return kb1.Key == kb2.Key && kb1.Alt == kb2.Alt && kb1.Control == kb2.Control && kb1.Shift == kb2.Shift;
			}

			public static bool operator !=(Keybind kb1, Keybind kb2)
			{
				return kb1.Key != kb2.Key || kb1.Alt != kb2.Alt || kb1.Control != kb2.Control || kb1.Shift != kb2.Shift;
			}
		}
	}
}