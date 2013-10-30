using System;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for NPC hooks.
	/// </summary>
	public class NpcEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the NPC instance.
		/// </summary>
		public NPC Npc
		{
			get;
			internal set;
		}
	}

	/// <summary>
	/// The API's NPC hooks.
	/// </summary>
	public static class NpcHooks
	{
		#region AI
		/// <summary>
		/// The event that runs when an NPC's AI is processed.
		/// </summary>
		public static event EventHandler<NpcEventArgs> ProcessAI;

		internal static void InvokeProcessAI(object npc)
		{
			if (ProcessAI != null)
				ProcessAI(null, new NpcEventArgs { Npc = (NPC)npc });
		}
		#endregion

		#region SetDefaults
		/// <summary>
		/// The event that runs when an NPC's default values are set.
		/// </summary>
		public static event EventHandler<NpcEventArgs> SetDefaults;

		internal static void InvokeSetDefaults(object npc)
		{
			if (SetDefaults != null)
				SetDefaults(null, new NpcEventArgs { Npc = (NPC)npc });
		}
		#endregion
	}
}
