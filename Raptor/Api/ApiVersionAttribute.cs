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
	///   Specifies the API version of a Raptor plugin.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ApiVersionAttribute : Attribute
	{
		/// <summary>
		///   The API version.
		/// </summary>
		public Version ApiVersion;

		/// <summary>
		///   Creates a new API version attribute.
		/// </summary>
		/// <param name="version">The version to use.</param>
		public ApiVersionAttribute(Version version)
		{
			ApiVersion = version;
		}

		/// <summary>
		///   Creates a new API version attribute.
		/// </summary>
		/// <param name="major">The major version of the attribute.</param>
		/// <param name="minor">The minor version of the attribute.</param>
		public ApiVersionAttribute(int major, int minor)
			: this(new Version(major, minor))
		{
		}
	}
}