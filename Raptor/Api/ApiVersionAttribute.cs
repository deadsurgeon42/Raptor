using System;

namespace Raptor.Api
{
	/// <summary>
	/// Specifies the API version of a Raptor plugin.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ApiVersionAttribute : Attribute
	{
		/// <summary>
		/// The API version.
		/// </summary>
		public Version ApiVersion;

		/// <summary>
		/// Creates a new API version attribute.
		/// </summary>
		/// <param name="version">The version to use.</param>
		public ApiVersionAttribute(Version version)
		{
			ApiVersion = version;
		}

		/// <summary>
		/// Creates a new API version attribute.
		/// </summary>
		/// <param name="major">The major version of the attribute.</param>
		/// <param name="minor">The minor version of the attribute.</param>
		public ApiVersionAttribute(int major, int minor)
			: this(new Version(major, minor))
		{
		}
	}
}
