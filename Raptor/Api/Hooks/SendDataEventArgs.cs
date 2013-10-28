using System;
using System.ComponentModel;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for send data hooks.
	/// </summary>
	public class SendDataEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets or sets the message packet type.
		/// </summary>
		public PacketTypes MsgId { get; set; }
		/// <summary>
		/// Gets or sets the first argument of the message.
		/// </summary>
		public int Number { get; set; }
		/// <summary>
		/// Gets or sets the second argument of the message.
		/// </summary>
		public float Number2 { get; set; }
		/// <summary>
		/// Gets or sets the third argument of the message.
		/// </summary>
		public float Number3 { get; set; }
		/// <summary>
		/// Gets or sets the fourth argument of the message.
		/// </summary>
		public float Number4 { get; set; }
		/// <summary>
		/// Gets or sets the fifth argument of the message.
		/// </summary>
		public int Number5 { get; set; }
		/// <summary>
		/// Gets or sets the message text.
		/// </summary>
		public string Text { get; set; }
	}
}
