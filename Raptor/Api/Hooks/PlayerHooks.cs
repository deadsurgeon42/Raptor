﻿//  Raptor - a client API for Terraria
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
using System.ComponentModel;
using System.IO;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	///   The API's player hooks.
	/// </summary>
	public static class PlayerHooks
	{
		#region Hurt

		/// <summary>
		///   Event arguments for Hurt hooks.
		/// </summary>
		public class HurtEventArgs : HandledEventArgs
		{
			internal HurtEventArgs(Player player, int damage, bool isPvP, bool isCrit)
			{
				Player = player;
				Damage = damage;
				IsPvP = isPvP;
				IsCrit = isCrit;
			}

			/// <summary>
			///   Gets the damage.
			/// </summary>
			public int Damage { get; private set; }

			/// <summary>
			///   Gets whether the damage was critical.
			/// </summary>
			public bool IsCrit { get; private set; }

			/// <summary>
			///   Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal => Main.myPlayer == Player.whoAmI;

			/// <summary>
			///   Gets whether the damage was dealt via PvP.
			/// </summary>
			public bool IsPvP { get; private set; }

			/// <summary>
			///   Gets the player.
			/// </summary>
			public Player Player { get; }
		}

		/// <summary>
		///   The event that runs when a player is hurt.
		/// </summary>
		public static event EventHandler<HurtEventArgs> Hurt;

		internal static bool InvokeHurt(object player, int damage, bool isPvP, bool isCrit)
		{
			if (Hurt != null)
			{
				var args = new HurtEventArgs((Player) player, damage, isPvP, isCrit);
				Hurt(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region Kill

		/// <summary>
		///   Event arguments for Kill hooks.
		/// </summary>
		public class KillEventArgs : HandledEventArgs
		{
			internal KillEventArgs(Player player, int damage, bool isPvP, string deathText)
			{
				Player = player;
				Damage = damage;
				IsPvP = isPvP;
				DeathText = deathText;
			}

			/// <summary>
			///   Gets the kill damage.
			/// </summary>
			public int Damage { get; private set; }

			/// <summary>
			///   Gets the death text.
			/// </summary>
			public string DeathText { get; private set; }

			/// <summary>
			///   Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal => Main.myPlayer == Player.whoAmI;

			/// <summary>
			///   Gets whether the kill was dealt via PvP.
			/// </summary>
			public bool IsPvP { get; private set; }

			/// <summary>
			///   Gets the player.
			/// </summary>
			public Player Player { get; }
		}

		/// <summary>
		///   The event that runs when a player is killed.
		/// </summary>
		public static event EventHandler<KillEventArgs> Kill;

		internal static bool InvokeKill(object player, double damage, bool isPvP, string deathText)
		{
			if (Kill != null)
			{
				var args = new KillEventArgs((Player) player, (int) damage, isPvP, deathText);
				Kill(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region Loaded

		/// <summary>
		///   Event arguments for Load hooks.
		/// </summary>
		public class LoadedEventArgs : EventArgs
		{
			internal LoadedEventArgs(BinaryReader reader)
			{
				Reader = reader;
			}

			/// <summary>
			///   Gets the binary reader.
			/// </summary>
			public BinaryReader Reader { get; private set; }
		}

		/// <summary>
		///   Occurs after a player file has been loaded.
		/// </summary>
		public static event EventHandler<LoadedEventArgs> Loaded;

		internal static void InvokeLoaded(BinaryReader reader)
		{
			Loaded?.Invoke(null, new LoadedEventArgs(reader));
		}

		#endregion

		#region Save

		/// <summary>
		///   Occurs before a player file is saved.
		/// </summary>
		public static event EventHandler Save;

		internal static void InvokeSave()
		{
			Save?.Invoke(null, EventArgs.Empty);
		}

		#endregion

		#region Saved

		/// <summary>
		///   Event arguments for Saved hooks.
		/// </summary>
		public class SavedEventArgs : EventArgs
		{
			internal SavedEventArgs(BinaryWriter writer)
			{
				Writer = writer;
			}

			/// <summary>
			///   Gets the binary writer.
			/// </summary>
			public BinaryWriter Writer { get; private set; }
		}

		/// <summary>
		///   Occurs after a player file has been saved, but before it has been encrypted.
		/// </summary>
		public static event EventHandler<SavedEventArgs> Saved;

		internal static void InvokeSaved(BinaryWriter writer)
		{
			Saved?.Invoke(null, new SavedEventArgs(writer));
		}

		#endregion

		#region Update

		/// <summary>
		///   Event arguments for Update-related hooks.
		/// </summary>
		public class UpdateEventArgs : EventArgs
		{
			internal UpdateEventArgs(Player player)
			{
				Player = player;
			}

			/// <summary>
			///   Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal => Main.myPlayer == Player.whoAmI;

			/// <summary>
			///   Gets the player.
			/// </summary>
			public Player Player { get; }
		}

		/// <summary>
		///   The event that runs before a player is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Update;

		internal static void InvokeUpdate(object player)
		{
			Update?.Invoke(null, new UpdateEventArgs((Player) player));
		}

		#endregion

		#region UpdateVars

		/// <summary>
		///   The event that runs before updating a player's variables.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> UpdateVars;

		internal static void InvokeUpdateVars(object player)
		{
			UpdateVars?.Invoke(null, new UpdateEventArgs((Player) player));
		}

		#endregion

		#region UpdatedVars

		/// <summary>
		///   The event that runs after updating a player's variables.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> UpdatedVars;

		internal static void InvokeUpdatedVars(object player)
		{
			UpdatedVars?.Invoke(null, new UpdateEventArgs((Player) player));
		}

		#endregion

		#region Updated

		/// <summary>
		///   The event that runs after a player is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Updated;

		internal static void InvokeUpdated(object player)
		{
			Updated?.Invoke(null, new UpdateEventArgs((Player) player));
		}

		#endregion
	}
}