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
		public NPC Npc { get; internal set; }
	}
}
