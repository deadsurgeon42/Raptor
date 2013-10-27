using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Raptor.Api
{
	/// <summary>
	/// Event arguments for windows forms message filter hooks.
	/// </summary>
	public class FilterMessageEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the message.
		/// </summary>
		public Message Message { get; internal set; }
	}
}
