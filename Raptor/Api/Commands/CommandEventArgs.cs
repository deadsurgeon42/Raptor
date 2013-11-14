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
		/// Creates new command event arguments from a string.
		/// </summary>
		/// <param name="str">The string.</param>
		public CommandEventArgs(string str)
		{
			var parameters = new List<string>();
			var sb = new StringBuilder();

			bool quote = false;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];

				if (c == '\\' && ++i < str.Length)
				{
					if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
						sb.Append('\\');
					sb.Append(str[i]);
				}
				else if (c == '"')
				{
					quote = !quote;
					if (!quote || sb.Length > 0)
					{
						parameters.Add(sb.ToString());
						sb.Clear();
					}
				}
				else if (Char.IsWhiteSpace(c) && !quote)
				{
					if (sb.Length > 0)
					{
						parameters.Add(sb.ToString());
						sb.Clear();
					}
				}
				else
					sb.Append(c);
			}
			if (sb.Length > 0)
				parameters.Add(sb.ToString());

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
