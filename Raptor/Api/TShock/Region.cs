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
