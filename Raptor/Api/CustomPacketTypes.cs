using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raptor.Api
{
	/// <summary>
	/// Custom packet types for use with TShock.
	/// </summary>
	public enum CustomPacketTypes : byte
	{
		/// <summary>
		/// The packet type which the client sends to the server to be acknowledged as a Raptor client.
		/// </summary>
		Acknowledge = 0,
	}
}
