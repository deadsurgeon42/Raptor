using System;

namespace Raptor.Api
{
	/// <summary>
	/// A Raptor plugin.
	/// </summary>
	public abstract class TerrariaPlugin : IDisposable
	{
		/// <summary>
		/// Gets the plugin's author.
		/// </summary>
		public virtual string Author { get { return ""; } }
		/// <summary>
		/// Gets the plugin's description.
		/// </summary>
		public virtual string Description { get { return ""; } }
		/// <summary>
		/// Gets the plugin's name.
		/// </summary>
		public virtual string Name { get { return ""; } }
		/// <summary>
		/// Gets or sets the plugin's order.
		/// </summary>
		public int Order { get; set; }
		/// <summary>
		/// Gets the plugin's version.
		/// </summary>
		public virtual Version Version { get { return new Version(1, 0); } }

		/// <summary>
		/// Creates a new plugin.
		/// </summary>
		protected TerrariaPlugin()
		{
			Order = 0;
		}

		/// <summary>
		/// Disposes the plugin.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Disposes the plugin.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
		}
		/// <summary>
		/// Initializes the plugin.
		/// </summary>
		public abstract void Initialize();
	}
}
