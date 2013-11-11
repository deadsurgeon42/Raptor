using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's player hooks.
	/// </summary>
	public static class PlayerHooks
	{
		#region Hurt
		/// <summary>
		/// Event arguments for Hurt hooks.
		/// </summary>
		public class HurtEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the damage.
			/// </summary>
			public int Damage { get; private set; }
			/// <summary>
			/// Gets whether the damage was critical.
			/// </summary>
			public bool IsCrit { get; private set; }
			/// <summary>
			/// Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal
			{
				get { return Main.myPlayer == Player.whoAmi; }
			}
			/// <summary>
			/// Gets whether the damage was dealt via PvP.
			/// </summary>
			public bool IsPvP { get; private set; }
			/// <summary>
			/// Gets the player.
			/// </summary>
			public Player Player { get; private set; }
			internal HurtEventArgs(Player player, int damage, bool isPvP, bool isCrit)
			{
				Player = player;
				Damage = damage;
				IsPvP = isPvP;
				IsCrit = isCrit;
			}
		}
		/// <summary>
		/// The event that runs when a player is hurt.
		/// </summary>
		public static event EventHandler<HurtEventArgs> Hurt;
		internal static bool InvokeHurt(object player, int damage, bool isPvP, bool isCrit)
		{
			if (Hurt != null)
			{
				var args = new HurtEventArgs((Player)player, damage, isPvP, isCrit);
				Hurt(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Kill
		/// <summary>
		/// Event arguments for Kill hooks.
		/// </summary>
		public class KillEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the kill damage.
			/// </summary>
			public int Damage { get; private set; }
			/// <summary>
			/// Gets the death text.
			/// </summary>
			public string DeathText { get; private set; }
			/// <summary>
			/// Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal
			{
				get { return Main.myPlayer == Player.whoAmi; }
			}
			/// <summary>
			/// Gets whether the kill was dealt via PvP.
			/// </summary>
			public bool IsPvP { get; private set; }
			/// <summary>
			/// Gets the player.
			/// </summary>
			public Player Player { get; private set; }
			internal KillEventArgs(Player player, int damage, bool isPvP, string deathText)
			{
				Player = player;
				Damage = damage;
				IsPvP = isPvP;
				DeathText = deathText;
			}
		}
		/// <summary>
		/// The event that runs when a player is killed.
		/// </summary>
		public static event EventHandler<KillEventArgs> Kill;
		internal static bool InvokeKill(object player, double damage, bool isPvP, string deathText)
		{
			if (Kill != null)
			{
				var args = new KillEventArgs((Player)player, (int)damage, isPvP, deathText);
				Kill(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion

		#region Update
		/// <summary>
		/// Event arguments for Update-related hooks.
		/// </summary>
		public class UpdateEventArgs : EventArgs
		{
			/// <summary>
			/// Gets whether the player is the local player.
			/// </summary>
			public bool IsLocal
			{
				get { return Main.myPlayer == Player.whoAmi; }
			}
			/// <summary>
			/// Gets the player.
			/// </summary>
			public Player Player { get; private set; }
			internal UpdateEventArgs(Player player)
			{
				Player = player;
			}
		}
		/// <summary>
		/// The event that runs before a player is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Update;
		internal static void InvokeUpdate(object player)
		{
			if (Update != null)
				Update(null, new UpdateEventArgs((Player)player));
		}
		#endregion
		#region UpdateVars
		/// <summary>
		/// The event that runs before updating a player's variables.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> UpdateVars;
		internal static void InvokeUpdateVars(object player)
		{
			if (UpdateVars != null)
				UpdateVars(null, new UpdateEventArgs((Player)player));
		}
		#endregion
		#region UpdatedVars
		/// <summary>
		/// The event that runs after updating a player's variables.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> UpdatedVars;
		internal static void InvokeUpdatedVars(object player)
		{
			if (UpdatedVars != null)
				UpdatedVars(null, new UpdateEventArgs((Player)player));
		}
		#endregion
		#region Updated
		/// <summary>
		/// The event that runs after a player is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Updated;
		internal static void InvokeUpdated(object player)
		{
			if (Updated != null)
				Updated(null, new UpdateEventArgs((Player)player));
		}
		#endregion
	}
}
