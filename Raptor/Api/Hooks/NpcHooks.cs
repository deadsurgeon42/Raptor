using System;
using Terraria;

namespace Raptor.Api.Hooks
{
	#region NpcEventArgs
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
			private set;
		}

		internal NpcEventArgs(NPC npc)
		{
			Npc = npc;
		}
	}
	#endregion

	/// <summary>
	/// The API's NPC hooks.
	/// </summary>
	public static class NpcHooks
	{
		#region ProcessAI
		/// <summary>
		/// The event that runs when an NPC's AI is processed.
		/// </summary>
		public static event EventHandler<NpcEventArgs> ProcessAI;

		internal static void InvokeProcessAI(object npc)
		{
			if (ProcessAI != null)
				ProcessAI(null, new NpcEventArgs((NPC)npc));
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
				SetDefaults(null, new NpcEventArgs((NPC)npc));
		}
		#endregion
	}
}
