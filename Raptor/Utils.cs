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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Raptor.Api;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// Contains various utility getters/setters and methods.
	/// </summary>
	public static class Utils
	{
		private static Dictionary<int, string> itemNames = new Dictionary<int, string>();
		private static string[] projNames = new string[Main.maxProjectileTypes];
		private static Dictionary<string, Keys> stringToXNAKey = new Dictionary<string, Keys>()
		{
			{ "\\", Keys.OemBackslash },
			{ "]", Keys.OemCloseBrackets },
			{ ",", Keys.OemComma },
			{ "-", Keys.OemMinus },
			{ "[", Keys.OemOpenBrackets },
			{ ".", Keys.OemPeriod },
			{ "=", Keys.OemPlus },
			{ "/", Keys.OemQuestion },
			{ "'", Keys.OemQuotes },
			{ ";", Keys.OemSemicolon },
		};
		private static Dictionary<Keys, string> xnaKeytoString = new Dictionary<Keys, string>()
		{
			{ Keys.OemBackslash, "\\" },
			{ Keys.OemCloseBrackets, "]" },
			{ Keys.OemComma, "," },
			{ Keys.OemMinus, "-" },
			{ Keys.OemOpenBrackets, "[" },
			{ Keys.OemPeriod, "." },
			{ Keys.OemPlus, "=" },
			{ Keys.OemQuestion, "/" },
			{ Keys.OemQuotes, "'" },
			{ Keys.OemSemicolon, ";" },
		};

		/// <summary>
		/// Gets the local player.
		/// </summary>
		public static Player LocalPlayer
		{
			get { return Main.player[Main.myPlayer]; }
		}

		internal static void Initialize()
		{
			var item = new Item();
			for (int i = -48; i < Main.maxItemTypes; i++)
			{
				item.netDefaults(i);
				itemNames.Add(i, item.name);
			}

			var proj = new Projectile();
			for (int i = 0; i < Main.maxProjectileTypes; i++)
			{
				proj.SetDefaults(i);
				projNames[i] = proj.name;
			}
		}

		/// <summary>
		/// Converts an XNA key to a string.
		/// </summary>
		/// <param name="key">The XNA key to convert.</param>
		/// <returns>The resultant string.</returns>
		public static string ConvertToString(Keys key)
		{
			if (xnaKeytoString.ContainsKey(key))
				return xnaKeytoString[key];
			return key.ToString();
		}
		/// <summary>
		/// Prints an error message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void ErrorMessage(string format, params object[] args)
		{
			if (!Main.gameMenu)
				Main.NewText(String.Format(format, args), 255, 0, 0);
		}
		/// <summary>
		/// Gets a list of matching items.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>The result.</returns>
		public static List<Item> FindItems(string name)
		{
			int id;
			if (int.TryParse(name, out id) && id >= -48 && id < Main.maxItemTypes)
			{
				var item = new Item();
				item.netDefaults(id);
				return new List<Item> { item };
			}

			var items = new List<Item>();
			for (int i = -48; i < Main.maxItemTypes; i++)
			{
				if (String.Equals(itemNames[i], name, StringComparison.CurrentCultureIgnoreCase))
				{
					var item = new Item();
					item.netDefaults(i);
					return new List<Item> { item };
				}
				else if (itemNames[i].ToLower().StartsWith(name.ToLower()))
				{
					var item = new Item();
					item.netDefaults(i);
					items.Add(item);
				}
			}
			return items;
		}
		/// <summary>
		/// Gets a list of matching players.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>The result.</returns>
		public static List<Player> FindPlayers(string name)
		{
			int id;
			if (int.TryParse(name, out id) && id >= 0 && id < Main.maxPlayers && Main.player[id].active)
				return new List<Player> { Main.player[id] };

			var players = new List<Player>();
			foreach (var player in Main.player.Where(p => p.active))
			{
				if (player.name == name)
					return new List<Player> { player };
				else if (player.name.ToLower().StartsWith(name.ToLower()))
					players.Add(player);
			}
			return players;
		}
		/// <summary>
		/// Gets a list of matching prefixes.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>The result.</returns>
		public static List<byte> FindPrefixes(string name)
		{
			byte id;
			if (byte.TryParse(name, out id) && id >= 0 && id < Lang.prefix.Length)
				return new List<byte> { id };

			var ids = new List<byte>();
			for (byte i = 0; i < Lang.prefix.Length; i++)
			{
				if (String.Equals(Lang.prefix[i], name, StringComparison.CurrentCultureIgnoreCase))
					return new List<byte> { i };
				else if (Lang.prefix[i].ToLower().StartsWith(name.ToLower()))
					ids.Add(i);
			}
			return ids;
		}
		/// <summary>
		/// Gets a list of matching projectiles.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>The result.</returns>
		public static List<Projectile> FindProjectiles(string name)
		{
			int id;
			if (int.TryParse(name, out id) && id >= 0 && id < Main.maxProjectileTypes)
			{
				var projectile = new Projectile();
				projectile.SetDefaults(id);
				return new List<Projectile> { projectile };
			}

			var projectiles = new List<Projectile>();
			for (int i = 0; i < Main.maxProjectileTypes; i++)
			{
				if (String.Equals(projNames[i], name, StringComparison.CurrentCultureIgnoreCase))
				{
					var projectile = new Projectile();
					projectile.SetDefaults(i);
					return new List<Projectile> { projectile };
				}
				else if (projNames[i].ToLower().StartsWith(name.ToLower()))
				{
					var projectile = new Projectile();
					projectile.SetDefaults(i);
					projectiles.Add(projectile);
				}
			}
			return projectiles;
		}
		/// <summary>
		/// Prints an info message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void InfoMessage(string format, params object[] args)
		{
			if (!Main.gameMenu)
				Main.NewText(String.Format(format, args), 255, 255, 0);
		}
		/// <summary>
		/// Prints a success message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void SuccessMessage(string format, params object[] args)
		{
			if (!Main.gameMenu)
				Main.NewText(String.Format(format, args), 0, 128, 0);
		}
		/// <summary>
		/// Trys to convert a string to an XNA key.
		/// </summary>
		/// <param name="key">The string to convert.</param>
		/// <returns>The resultant XNA key.</returns>
		public static bool TryParseXNAKey(string key, out Keys result)
		{
			if (Enum.TryParse<Keys>(key, true, out result))
				return true;
			if (stringToXNAKey.TryGetValue(key, out result))
				return true;
			return false;
		}
	}
}
