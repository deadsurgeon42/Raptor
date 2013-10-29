using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raptor.Api.Commands
{
	/// <summary>
	/// Represents an executable (chat) command.
	/// </summary>
	public class Command
	{
		/// <summary>
		/// The command delegate.
		/// </summary>
		/// <param name="sender">The sender (in this case, the command).</param>
		/// <param name="e">The command event arguments.</param>
		public delegate void CommandD(object sender, CommandEventArgs e);

		private CommandD callback;
		private string[] names;

		/// <summary>
		/// Gets the command callback.
		/// </summary>
		public CommandD Callback
		{
			get { return callback; }
		}
		/// <summary>
		/// Gets or sets the command's description.
		/// </summary>
		public string Description
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the command's help text.
		/// </summary>
		public string[] HelpText
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the command's primary name.
		/// </summary>
		public string Name
		{
			get { return names[0]; }
		}
		/// <summary>
		/// Gets all of the command's name.
		/// </summary>
		public string[] Names
		{
			get { return names; }
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="callback">The command callback.</param>
		/// <param name="names">The command names.</param>
		public Command(CommandD callback, params string[] names)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (names == null || names.Length == 0)
				throw new ArgumentException("\"names\" cannot be null or have no names.");

			this.callback = callback;
			this.Description = "Sorry, no description is available.";
			this.HelpText = new[] { "Sorry, no help is available." };
			this.names = names;
		}

		/// <summary>
		/// Invokes the command call back.
		/// </summary>
		/// <param name="e">The command event arguments.</param>
		public void Invoke(CommandEventArgs e)
		{
			callback(this, e);
		}
	}
}
