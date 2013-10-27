using System;
using Microsoft.Xna.Framework.Content;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for content loading hooks.
	/// </summary>
	public sealed class LoadContentEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the content manager used for loading content.
		/// </summary>
		public ContentManager Content { get; internal set; }
	}
}
