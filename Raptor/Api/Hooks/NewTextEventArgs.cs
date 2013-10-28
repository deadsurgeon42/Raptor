using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for new text.
	/// </summary>
	public class NewTextEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		public Color Color { get; set; }
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text { get; set; }
	}
}
