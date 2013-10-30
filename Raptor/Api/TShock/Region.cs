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
		/// Gets or sets the list of allowed user IDs.
		/// </summary>
		public List<int> AllowedIDs
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the area of the region.
		/// </summary>
		public Rectangle Area
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the owner's account name.
		/// </summary>
		public string Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a new region.
		/// </summary>
		public Region()
		{
			AllowedIDs = new List<int>();
			Area = new Rectangle();
			Name = "";
			Owner = "";
		}
	}
}
