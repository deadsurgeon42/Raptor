//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
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

namespace Raptor.Api
{
	/// <summary>
	///   A Raptor plugin.
	/// </summary>
	public abstract class TerrariaPlugin : IDisposable
	{
		/// <summary>
		///   Creates a new plugin.
		/// </summary>
		protected TerrariaPlugin()
		{
			Order = 0;
		}

		/// <summary>
		///   Gets the plugin's author.
		/// </summary>
		public virtual string Author
		{
			get { return ""; }
		}

		/// <summary>
		///   Gets the plugin's description.
		/// </summary>
		public virtual string Description
		{
			get { return ""; }
		}

		/// <summary>
		///   Gets the plugin's name.
		/// </summary>
		public virtual string Name
		{
			get { return ""; }
		}

		/// <summary>
		///   Gets or sets the plugin's order.
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		///   Gets the plugin's version.
		/// </summary>
		public virtual Version Version
		{
			get { return new Version(1, 0); }
		}

		/// <summary>
		///   Disposes the plugin.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TerrariaPlugin()
		{
			Dispose(false);
		}

		/// <summary>
		///   Disposes the plugin.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		///   Initializes the plugin.
		/// </summary>
		public abstract void Initialize();
	}
}