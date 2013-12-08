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
	/// Represents a region -- an area of tiles which may be protected.
	/// </summary>
	public class Region
	{
		/// <summary>
		/// The area of the region.
		/// </summary>
		public Rectangle Area;
		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a new region.
		/// </summary>
		public Region()
		{
			Area = new Rectangle();
			Name = "";
		}
	}
}
