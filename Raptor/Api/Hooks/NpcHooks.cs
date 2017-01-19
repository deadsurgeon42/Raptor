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
using System.ComponentModel;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	///   The API's NPC hooks.
	/// </summary>
	public static class NpcHooks
	{
		#region DropLoot

		/// <summary>
		///   Event arguments for DropLoot hooks.
		/// </summary>
		public class DropLootEventArgs : HandledEventArgs
		{
			internal DropLootEventArgs(NPC npc)
			{
				Npc = npc;
			}

			/// <summary>
			///   Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
		}

		/// <summary>
		///   The event that runs when an NPC drops loot.
		/// </summary>
		public static event EventHandler<DropLootEventArgs> DropLoot;

		internal static bool InvokeDropLoot(object npc)
		{
			if (DropLoot != null)
			{
				var args = new DropLootEventArgs((NPC) npc);
				DropLoot(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region ProcessAI

		/// <summary>
		///   Event arguments for ProcessAI hooks.
		/// </summary>
		public class ProcessAIEventArgs : HandledEventArgs
		{
			internal ProcessAIEventArgs(NPC npc)
			{
				Npc = npc;
			}

			/// <summary>
			///   Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
		}

		/// <summary>
		///   The event that runs when an NPC's AI is processed.
		/// </summary>
		public static event EventHandler<ProcessAIEventArgs> ProcessAI;

		internal static bool InvokeProcessAI(object npc)
		{
			if (ProcessAI != null)
			{
				var args = new ProcessAIEventArgs((NPC) npc);
				ProcessAI(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region SetDefaults

		/// <summary>
		///   Event arguments for SetDefaults hooks.
		/// </summary>
		public class SetDefaultsEventArgs : EventArgs
		{
			internal SetDefaultsEventArgs(NPC npc)
			{
				Npc = npc;
			}

			/// <summary>
			///   Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
		}

		/// <summary>
		///   The event that runs when an NPC's default values are set.
		/// </summary>
		public static event EventHandler<SetDefaultsEventArgs> SetDefaults;

		internal static void InvokeSetDefaults(object npc)
		{
			SetDefaults?.Invoke(null, new SetDefaultsEventArgs((NPC) npc));
		}

		#endregion
	}
}