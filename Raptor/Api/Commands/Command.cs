﻿//  Raptor - a client API for Terraria
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

namespace Raptor.Api.Commands
{
	/// <summary>
	///   Represents an executable (chat) command.
	/// </summary>
	public class Command
	{
		/// <summary>
		///   Creates a new command.
		/// </summary>
		/// <param name="callback">The command callback.</param>
		/// <param name="names">The command names.</param>
		public Command(EventHandler<CommandEventArgs> callback, params string[] names)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));
			if (names == null)
				throw new ArgumentNullException(nameof(names));

			Callback = callback;
			HelpText = new[] {"Sorry, no help is available."};
			Names = names;
		}

		/// <summary>
		///   Gets the command callback.
		/// </summary>
		public EventHandler<CommandEventArgs> Callback { get; }

		/// <summary>
		///   Gets or sets the command's help text.
		/// </summary>
		public string[] HelpText { get; set; }

		/// <summary>
		///   Gets the command's primary name.
		/// </summary>
		public string Name => Names[0];

		/// <summary>
		///   Gets all of the command's name.
		/// </summary>
		public string[] Names { get; }

		/// <summary>
		///   Invokes the command call back.
		/// </summary>
		/// <param name="e">The command event arguments.</param>
		public void Invoke(CommandEventArgs e)
		{
			Callback(this, e);
		}
	}
}