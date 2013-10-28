﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Input;

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

		/// <summary>
		/// Executes the string as a command.
		/// </summary>
		/// <param name="text">The command text.</param>
		public static void Execute(string text)
		{
			var args = new CommandEventArgs(text);
			bool found = false;
			foreach (Command c in Commands.ChatCommands)
			{
				if (c.Names.Contains(args[-1].ToLower()))
				{
					found = true;
					c.Invoke(args);
					break;
				}
			}
			if (!found)
				Utils.NewErrorText("< Invalid command.");
		}
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
			ChatCommands.Add(new Command(Keybind, "keybind", "kb")
			{
				HelpText = "Manages command key bindings."
			});
		}

		static void Execute(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewErrorText("< Syntax: /{0} <file name>", e[-1]);
				return;
			}

			foreach (string path in Directory.EnumerateFiles("Scripts", e.Eol(0) + ".*"))
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
		static void Keybind(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewErrorText("< Syntax: /{0} <add | clr | del | list> [arguments...]", e[-1]);
				return;
			}

			switch (e[0].ToLower())
			{
				case "add":
					{
						if (e.Length < 3)
						{
							Utils.NewErrorText("< Syntax: /{0} add <key> <command>", e[-1]);
							return;
						}

						Keys key;
						if (!Enum.TryParse<Keys>(e[1], true, out key))
						{
							Utils.NewErrorText("< Invalid key '{0}'.", e[1]);
							return;
						}

						if (Raptor.Config.KeyBindings.ContainsKey(key))
						{
							Utils.NewErrorText("< The key '{0}' is already bound.", key);
							return;
						}

						Raptor.Config.KeyBindings.Add(key, e.Eol(2));
						Utils.NewSuccessText("< Bound the key '{0}' to '{1}'.", key, e.Eol(2));
					}
					return;
				case "clr":
					Raptor.Config.KeyBindings.Clear();
					Utils.NewSuccessText("< Cleared all key bindings.");
					return;
				case "del":
					{
						if (e.Length == 1)
						{
							Utils.NewErrorText("< Syntax: /{0} del <key>", e[0]);
							return;
						}

						Keys key;
						if (!Enum.TryParse<Keys>(e[1], true, out key))
						{
							Utils.NewErrorText("< Invalid key '{0}'.", e[1]);
							return;
						}
						if (!Raptor.Config.KeyBindings.ContainsKey(key))
						{
							Utils.NewErrorText("< The key '{0}' is not bound.", key);
							return;
						}
						Raptor.Config.KeyBindings.Remove(key);
						Utils.NewSuccessText("< Unbound the key '{0}'.", key);
					}
					return;
				case "list":
					Utils.NewSuccessText("< Raptor Keybinds:");
					foreach (KeyValuePair<Keys, string> kv in Raptor.Config.KeyBindings)
					{
						Utils.NewInfoText("< Key '{0}': {1}", kv.Key, kv.Value);
					}
					return;
				default:
					Utils.NewErrorText("< Syntax: /{0} <add | clr | del | list> [arguments...]");
					return;
			}
		}
	}
}
