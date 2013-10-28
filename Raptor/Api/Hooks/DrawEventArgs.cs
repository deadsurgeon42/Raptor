using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for drawing hooks.
	/// </summary>
	public class DrawEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the sprite batch used for drawing.
		/// </summary>
		public SpriteBatch SpriteBatch { get; internal set; }
	}
}
