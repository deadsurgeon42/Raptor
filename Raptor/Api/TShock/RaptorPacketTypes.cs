//  Raptor - a client API for Terraria
//  Copyright (C) 2013 MarioE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
		/// The packet sent which sets region info.
		/// </summary>
		Region,
		/// <summary>
		/// The packet sent to delete a region.
		/// </summary>
		RegionDelete,
		/// <summary>
		/// The packet sent which sets warp info.
		/// </summary>
		Warp,
		/// <summary>
		/// The packet sent to delete a warp.
		/// </summary>
		WarpDelete
	}
}
