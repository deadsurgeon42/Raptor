using System;

namespace Raptor.Api.Commands
{
	/// <summary>
	/// Represents an executable (chat) command.
	/// </summary>
	public class Command
	{
		EventHandler<CommandEventArgs> callback;
		string[] names;

		/// <summary>
		/// Gets the command callback.
		/// </summary>
		public EventHandler<CommandEventArgs> Callback
		{
			get { return callback; }
		}
		/// <summary>
		/// Gets or sets the command's help text.
		/// </summary>
		public string[] HelpText { get; set; }
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
		public Command(EventHandler<CommandEventArgs> callback, params string[] names)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (names == null)
				throw new ArgumentNullException("names");

			this.callback = callback;
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
