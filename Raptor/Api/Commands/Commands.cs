using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

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
		static List<Command> LuaCommands = new List<Command>();

		/// <summary>
		/// Executes a string as a command.
		/// </summary>
		/// <param name="text">The command text.</param>
		public static void Execute(string text)
		{
			var args = new CommandEventArgs(text);
			
			foreach (Command c in ChatCommands.Concat(LuaCommands))
			{
				if (c.Names.Contains(args[-1].ToLower()))
				{
					c.Invoke(args);
					return;
				}
			}

			Utils.NewErrorText("Invalid command.");
		}
		internal static void Init()
		{
			ChatCommands.Add(new Command(Help, "help", "?")
			{
				Description = "Provides help on commands.",
				HelpText = new[]
				{
					"Syntax: /help (command)",
					"Prints a list of commands or help about a specific command."
				}
			});
			ChatCommands.Add(new Command(Keybind, "keybind", "kb")
			{
				Description = "Manages key bindings.",
				HelpText = new[]
				{
					"Syntax: /keybind <add | clr | del | list> [arguments...]",
					"To add a key binding, use /kb add <key> <command>, without the first /.",
					"To clear all key bindings, use /kb clr.",
					"To remove a key binding, use /kb del <key>.",
					"To list all key bindings, use /kb list.",
				}
			});
			ChatCommands.Add(new Command(Lua, "lua", "l")
			{
				Description = "Executes a Lua script.",
				HelpText = new[]
				{
					"Syntax: /lua <script name>",
					"Executes the Lua script file with the given filename in the Scripts folder.",
				}
			});
			ChatCommands.Add(new Command(Reload, "reload")
			{
				Description = "Reloads various objects.",
				HelpText = new[]
				{
					"Syntax: /reload",
					"Reloads various objects such as the configuration file and lua script commands."
				}
			});

			LoadLuaCommands();
		}
		static void LoadLuaCommands()
		{
			foreach (string path in Directory.EnumerateFiles(Path.Combine("Scripts"), "*.lua"))
			{
				List<string> lines = File.ReadAllLines(path).ToList();
				List<string> names = new List<string> { Path.GetFileNameWithoutExtension(path) };

				var aliases = from s in lines
							  where s.StartsWith("-- Aliases: ")
							  select s.Substring(12);
				if (aliases.Any())
					names.AddRange(aliases.ElementAt(0).Split(','));

				var command = new Command(LuaCommand, names.ToArray());

				var description = from s in lines
								  where s.StartsWith("-- Description: ")
								  select s.Substring(16);
				if (description != null)
					command.Description = description.ElementAt(0);

				var helpTexts = from s in lines
								where s.StartsWith("-- HelpText: ")
								select s.Substring(13);
				if (helpTexts.Any())
					command.HelpText = helpTexts.ToArray();

				LuaCommands.Add(command);
			}
		}

		static void Help(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewSuccessText("Raptor commands: ");
				foreach (Command c in ChatCommands.Concat(LuaCommands))
					Utils.NewInfoText("/{0}: {1}", c.Name, c.Description);
				return;
			}

			string commandName = e.Eol(0).ToLower();
			foreach (Command c in ChatCommands.Concat(LuaCommands))
			{
				if (c.Name == commandName)
				{
					Utils.NewSuccessText("/{0} help:", c.Name);
					foreach (string line in c.HelpText)
						Utils.NewInfoText("{0}", line);
					return;
				}
			}
			Utils.NewErrorText("Invalid command: \"/{0}\".", commandName);
		}
		static void Keybind(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewErrorText("Syntax: /{0} <add | clr | del | list> [arguments...]", e[-1]);
				return;
			}

			switch (e[0].ToLower())
			{
				case "add":
					{
						if (e.Length < 3)
						{
							Utils.NewErrorText("Syntax: /{0} add <key> <command>", e[-1]);
							return;
						}

						Keys key;
						if (!Enum.TryParse<Keys>(e[1], true, out key))
						{
							Utils.NewErrorText("Invalid key '{0}'.", e[1]);
							return;
						}

						if (Raptor.Config.KeyBindings.ContainsKey(key))
						{
							Utils.NewErrorText("The key '{0}' is already bound.", key);
							return;
						}

						Raptor.Config.KeyBindings.Add(key, e.Eol(2));
						Utils.NewSuccessText("Bound the key '{0}' to '{1}'.", key, e.Eol(2));
					}
					return;
				case "clr":
					Raptor.Config.KeyBindings.Clear();
					Utils.NewSuccessText("Cleared all key bindings.");
					return;
				case "del":
					{
						if (e.Length == 1)
						{
							Utils.NewErrorText("Syntax: /{0} del <key>", e[0]);
							return;
						}

						Keys key;
						if (!Enum.TryParse<Keys>(e[1], true, out key))
						{
							Utils.NewErrorText("Invalid key '{0}'.", e[1]);
							return;
						}
						if (!Raptor.Config.KeyBindings.ContainsKey(key))
						{
							Utils.NewErrorText("The key '{0}' is not bound.", key);
							return;
						}
						Raptor.Config.KeyBindings.Remove(key);
						Utils.NewSuccessText("Unbound the key '{0}'.", key);
					}
					return;
				case "list":
					Utils.NewSuccessText("Key Bindings:");
					foreach (KeyValuePair<Keys, string> kv in Raptor.Config.KeyBindings)
					{
						Utils.NewInfoText("Key '{0}': {1}", kv.Key, kv.Value);
					}
					return;
				default:
					Utils.NewErrorText("Syntax: /{0} <add | clr | del | list> [arguments...]");
					return;
			}
		}
		static void Lua(object o, CommandEventArgs e)
		{
			if (e.Length == 0)
			{
				Utils.NewErrorText("Syntax: /{0} <script name>", e[-1]);
				return;
			}

			string scriptPath = Path.Combine("Scripts", e[-1] + ".lua");
			if (File.Exists(scriptPath))
			{
				Raptor.Lua["args"] = new CommandEventArgs(e.Eol(0));
				ThreadPool.QueueUserWorkItem(c =>
				{
					try
					{
						Raptor.Lua.DoString(File.ReadAllText(scriptPath));
					}
					catch (Exception ex)
					{
						Utils.NewErrorText(ex.ToString());
					}
				});
				return;
			}

			Utils.NewErrorText("Invalid script.");
		}
		static void LuaCommand(object o, CommandEventArgs e)
		{
			Raptor.Lua["args"] = e;
			ThreadPool.QueueUserWorkItem(c =>
			{
				try
				{
					Raptor.Lua.DoString(File.ReadAllText(Path.Combine("Scripts", ((Command)o).Name + ".lua")));
				}
				catch (Exception ex)
				{
					Utils.NewErrorText(ex.ToString());
				}
			});
		}
		static void Reload(object o, CommandEventArgs e)
		{
			string configPath = Path.Combine("Raptor", "config.json");
			Raptor.Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));

			LuaCommands.Clear();
			LoadLuaCommands();

			Utils.NewSuccessText("Reloaded configuration file & scripts.");
		}
	}
}
