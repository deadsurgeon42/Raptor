//  Raptor - a client API for Terraria
//  Copyright (C) 2013 MarioE
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raptor.Api.Commands
{
	/// <summary>
	/// Represents a command's event arguments.
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		List<string> parameters;
		
		/// <summary>
		/// Gets the parameter at the specified index, with no quotes. An index of -1 is the command itself.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The specified parameter.</returns>
		public string this[int index]
		{
			get
			{
				string param = parameters[index + 1];
				if (param.StartsWith("\"") && param.EndsWith("\"") && param.Length > 1)
					return param.Substring(1, param.Length - 2);
				return param;
			}
		}
		/// <summary>
		/// Gets the number of the parameters, not including the command itself.
		/// </summary>
		public int Length
		{
			get { return parameters.Count - 1; }
		}

		/// <summary>
		/// Creates new command event arguments from a list of parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		public CommandEventArgs(List<string> parameters)
		{
			this.parameters = parameters;
		}

		/// <summary>
		/// Gets the combined parameters after a certain index. Note that quotes are included!
		/// </summary>
		/// <param name="index">The index. An index of -1 includes the command itself.</param>
		/// <returns>The combined parameters.</returns>
		public string Eol(int index)
		{
			return String.Join(" ", parameters.ToArray(), index + 1, parameters.Count - index - 1);
		}
	}
}
