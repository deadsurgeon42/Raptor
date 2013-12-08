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
using Microsoft.Xna.Framework;

namespace Raptor.Api.TShock
{
	/// <summary>
	/// Represents a TShock warp.
	/// </summary>
	public class Warp
	{
		/// <summary>
		/// Gets or sets the name of the warp.
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		/// <summary>
		/// The position of the warp.
		/// </summary>
		public Point Position;

		/// <summary>
		/// Creates a new warp.
		/// </summary>
		public Warp()
		{
			Name = "";
			Position = Point.Zero;
		}
	}
}
