using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raptor.Api.TShock
{
	/// <summary>
	/// Custom packet types for use with TShock.
	/// </summary>
	public enum RaptorPacketTypes : byte
	{
		/// <summary>
		/// The packet sent to the server to be acknowledged as a Raptor client.
		/// </summary>
		Acknowledge = 0,
		/// <summary>
		/// The packet sent to the client which dictates its permissions.
		/// </summary>
		Permissions,
		/// <summary>
		/// The packet sent to the client which gives region info.
		/// </summary>
		RegionInfo,
		/// <summary>
		/// The packet sent to modify a region.
		/// </summary>
		RegionModify,
	}
}
