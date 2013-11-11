using System;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's NPC hooks.
	/// </summary>
	public static class NpcHooks
	{
		#region ProcessAI
		/// <summary>
		/// Event arguments for ProcessAI hooks.
		/// </summary>
		public class ProcessAIEventArgs : EventArgs
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
		internal static void InvokeProcessAI(object npc)
		{
			if (ProcessAI != null)
				ProcessAI(null, new ProcessAIEventArgs((NPC)npc));
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
