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
