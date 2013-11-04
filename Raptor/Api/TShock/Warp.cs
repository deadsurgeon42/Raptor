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
		/// Gets or sets whether the warp is private.
		/// </summary>
		public bool IsPrivate
		{
			get;
			set;
		}
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
			IsPrivate = false;
			Name = "";
			Position = Point.Zero;
		}
	}
}
