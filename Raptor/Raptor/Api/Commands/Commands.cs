using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLua;
using Terraria;

namespace Raptor.Api.Commands
{
	/// <summary>
	/// Contains raptor's commands.
	/// </summary>
	public static class Commands
	{
		/// <summary>
		/// The list of chat commands.
		/// </summary>
		public static List<Command> ChatCommands = new List<Command>();
		
		internal static void Init()
		{
			ChatCommands.Add(new Command(Execute, "execute", "exec", "x")
			{
				HelpText = "Executes a script file."
			});
			ChatCommands.Add(new Command(Help, "help", "?")
			{
				HelpText = "Prints a list of commands or help about a specific command."
			});
		}

		static void Execute(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewErrorText("< Syntax: /{0} <file name>", e[-1]);
				return;
			}

			foreach (string path in Directory.EnumerateFiles(Path.Combine("Raptor", "Scripts"), e.Eol(0) + ".*"))
			{
				switch (Path.GetExtension(path))
				{
					case ".lua":
						Raptor.Lua["args"] = new CommandEventArgs(e.Eol(0));
						ThreadPool.QueueUserWorkItem(ExecLuaCallback, File.ReadAllText(path));
						return;
				}
			}

			Utils.NewErrorText("< Invalid file or scripting language.");
		}
		static void ExecLuaCallback(object code)
		{
			try
			{
				Raptor.Lua.DoString((string)code);
			}
			catch (Exception ex)
			{
				Utils.NewErrorText("< Lua error: {0}", ex);
			}
		}
		static void Help(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewSuccessText("< Raptor commands: ");
				var commandNames = from c in ChatCommands
								   select c.Name;
				Utils.NewInfoText("< {0}", String.Join(", ", commandNames));
				return;
			}

			string commandName = e.Eol(0).ToLower();
			foreach (Command c in ChatCommands)
			{
				if (c.Name == commandName)
				{
					Utils.NewSuccessText("< /{0} help:", c.Name);
					Utils.NewInfoText("< {0}", c.HelpText);
					return;
				}
			}
			Utils.NewErrorText("< Invalid command: \"/{0}\".", commandName);
		}
	}
}
