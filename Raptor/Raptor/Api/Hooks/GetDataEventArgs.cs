using System;
using System.ComponentModel;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for get data hooks.
	/// </summary>
	public class GetDataEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets or sets the message buffer.
		/// </summary>
		public messageBuffer Msg { get; set; }
		/// <summary>
		/// Gets or sets the message packet type.
		/// </summary>
		public PacketTypes MsgID { get; set; }
		/// <summary>
		/// Gets or sets the index in the data buffer.
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// Gets or sets the length of the data in the data buffer.
		/// </summary>
		public int Length { get; set; }
	}
}
