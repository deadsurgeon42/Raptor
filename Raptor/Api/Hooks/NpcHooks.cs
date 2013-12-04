using System;
using System.ComponentModel;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's NPC hooks.
	/// </summary>
	public static class NpcHooks
	{
		#region DropLoot
		/// <summary>
		/// Event arguments for DropLoot hooks.
		/// </summary>
		public class DropLootEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
			internal DropLootEventArgs(NPC npc)
			{
				Npc = npc;
			}
		}
		/// <summary>
		/// The event that runs when an NPC drops loot.
		/// </summary>
		public static event EventHandler<DropLootEventArgs> DropLoot;
		internal static bool InvokeDropLoot(object npc)
		{
			if (DropLoot != null)
			{
				var args = new DropLootEventArgs((NPC)npc);
				DropLoot(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region ProcessAI
		/// <summary>
		/// Event arguments for ProcessAI hooks.
		/// </summary>
		public class ProcessAIEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
			internal ProcessAIEventArgs(NPC npc)
			{
				Npc = npc;
			}
		}
		/// <summary>
		/// The event that runs when an NPC's AI is processed.
		/// </summary>
		public static event EventHandler<ProcessAIEventArgs> ProcessAI;
		internal static bool InvokeProcessAI(object npc)
		{
			if (ProcessAI != null)
			{
				var args = new ProcessAIEventArgs((NPC)npc);
				ProcessAI(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region SetDefaults
		/// <summary>
		/// Event arguments for SetDefaults hooks.
		/// </summary>
		public class SetDefaultsEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the NPC instance.
			/// </summary>
			public NPC Npc { get; private set; }
			internal SetDefaultsEventArgs(NPC npc)
			{
				Npc = npc;
			}
		}
		/// <summary>
		/// The event that runs when an NPC's default values are set.
		/// </summary>
		public static event EventHandler<SetDefaultsEventArgs> SetDefaults;
		internal static void InvokeSetDefaults(object npc)
		{
			if (SetDefaults != null)
				SetDefaults(null, new SetDefaultsEventArgs((NPC)npc));
		}
		#endregion
	}
}
