using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// Contains various utility methods.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Prints an error message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewErrorText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 255, 0, 0);
		}
		/// <summary>
		/// Prints an info message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewInfoText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 255, 255, 0);
		}
		/// <summary>
		/// Prints a success message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewSuccessText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 0, 128, 0);
		}
	}
}
