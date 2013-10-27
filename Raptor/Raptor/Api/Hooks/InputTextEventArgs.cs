using System;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Input text event arguments.
	/// </summary>
	public class InputTextEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text { get; set; }
	}
}
