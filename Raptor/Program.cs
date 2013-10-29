using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Terraria;
using Raptor.Api;
using Raptor.Api.Hooks;

namespace Raptor
{
	class Program
	{
		const BindingFlags allFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		static Dictionary<string, string> drawHooks = new Dictionary<string, string>
		{
			{ "Draw", "" },
			{ "DrawChat", "NPCChat" },
			{ "DrawPlayerChat", "PlayerChat" },
		};
		static Assembly terraria;
		const string registry = @"SOFTWARE\Re-Logic\Terraria";
		
		[STAThread]
		static void Main()
		{
			string path = (string)Registry.LocalMachine.OpenSubKey(registry).GetValue("Install_Path", null);

			#region Piracy checks
			if (path == null || !File.Exists(Path.Combine(path, "Terraria.exe")))
			{
				MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (Directory.GetCurrentDirectory() == path)
			{
				MessageBox.Show("Please do not run this program in the same folder as Terraria.", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var assembly = AssemblyDefinition.ReadAssembly(Path.Combine(path, "Terraria.exe"));
			var module = assembly.MainModule;

			try
			{
				// Steam.SteamAPI_Init() check.
				var steamApiInit = assembly.GetMethod("Steam", "SteamAPI_Init");
				if (!steamApiInit.IsPInvokeImpl || steamApiInit.PInvokeInfo.Module.Name != "steam_api.dll")
				{
					MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Steam.Init() check.
				var steamInit = assembly.GetMethod("Steam", "Init");
				if (!steamInit.HasSameInstructions(0,
					Instruction.Create(OpCodes.Call, steamApiInit),
					Instruction.Create(OpCodes.Stsfld, assembly.GetField("Steam", "SteamInit")),
					Instruction.Create(OpCodes.Ret)))
				{
					MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Program.Main() check.
				bool foundCheck = false;
				var programMain = assembly.GetMethod("Program", "Main");
				for (int i = 0; i < programMain.Body.Instructions.Count; i++)
				{
					var instr = programMain.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Call &&
						((MethodReference)instr.Operand).FullName == "System.Void Terraria.Steam::Init()" &&

						instr.Next.OpCode == OpCodes.Ldsfld &&
						((FieldReference)instr.Next.Operand).FullName == "System.Boolean Terraria.Steam::SteamInit" &&

						instr.Next.Next.OpCode == OpCodes.Brfalse_S && ((Instruction)instr.Next.Next.Operand).Next.OpCode == OpCodes.Ldstr)
					{
						foundCheck = true;
					}
				}

				if (!foundCheck)
				{
					MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			catch
			{
				// Exception here means piracy
				MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			#endregion
			#region IL injection
			// ItemHooks.InvokeSetDefaults(this);
			assembly.GetMethod("Item", "SetDefaults", new[] { "Int32", "Boolean" }).InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, module.Import(typeof(ItemHooks).GetMethod("InvokeSetDefaults", allFlags))));

			var keyinPreFilterMessage = assembly.GetType("keyBoardInput").NestedTypes[0].Methods[0];
			// GameHooks.InvokeFilterMessage(m); return;
			keyinPreFilterMessage.InsertStart(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, module.Import(typeof(Input).GetMethod("FilterMessage", allFlags))),
				Instruction.Create(OpCodes.Ldc_I4_0),
				Instruction.Create(OpCodes.Ret));

			foreach (KeyValuePair<string, string> kvp in drawHooks)
			{
				var draw = assembly.GetMethod("Main", kvp.Key);
				draw.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, assembly.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, "Pre" + kvp.Value),
					Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeDraw", allFlags))),
					Instruction.Create(OpCodes.Brfalse_S, draw.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				draw.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, assembly.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, "Post" + kvp.Value),
					Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeDraw", allFlags))),
					Instruction.Create(OpCodes.Pop));
			}

			// GameHooks.InvokeInitialized();
			assembly.GetMethod("Main", "Initialize").InsertEnd(
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeInitialized", allFlags))));

			var mainInputText = assembly.GetMethod("Main", "GetInputText");
			// return GameHooks.InvokeInputText(oldString);
			mainInputText.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeInputText", allFlags))),
				Instruction.Create(OpCodes.Ret));

			// GameHooks.InvokeLoadedContent(this.Content);
			assembly.GetMethod("Main", "LoadContent").InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Callvirt, module.Import(typeof(Game).GetMethod("get_Content"))),
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeLoadedContent", allFlags))));

			var mainNewText = assembly.GetMethod("Main", "NewText");
			// if (GameHooks.InvokeNewText(text, r, g, b)) return;
			mainNewText.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Ldarg_2),
				Instruction.Create(OpCodes.Ldarg_3),
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeNewText", allFlags))),
				Instruction.Create(OpCodes.Ret));

			var mainUpdate = assembly.GetMethod("Main", "Update");
			// GameHooks.InvokePreUpdate();
			mainUpdate.InsertStart(
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokePreUpdate", allFlags))));
			// GameHooks.InvokePostUpdate();
			mainUpdate.InsertEnd(
				Instruction.Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokePostUpdate", allFlags))));

			var messageBufferGetData = assembly.GetMethod("messageBuffer", "GetData");
			// if (NetHooks.InvokeGetData(start, length)) return;
			messageBufferGetData.InsertStart(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Ldarg_2),
				Instruction.Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeGetData", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, messageBufferGetData.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ret));

			var netMessageSendData = assembly.GetMethod("NetMessage", "SendData");
			// if (NetHooks.InvokeSendData(msgType, text, number, number2, number3, number4, number5)) return;
			netMessageSendData.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_3),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[4]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[5]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[6]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[7]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[8]),
				Instruction.Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeSendData", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, netMessageSendData.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ret));

			// NpcHooks.InvokeAI(this);
			assembly.GetMethod("NPC", "AI").InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, module.Import(typeof(NpcHooks).GetMethod("InvokeAI", allFlags))));

			// NpcHooks.InvokeSetDefaults(this);
			assembly.GetMethod("NPC", "SetDefaults", new[] { "Int32", "Single" }).InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, module.Import(typeof(NpcHooks).GetMethod("InvokeSetDefaults", allFlags))));

			// return;
			assembly.GetMethod("Steam", "Kill").InsertStart(
				Instruction.Create(OpCodes.Ret));

			// Force internal types public
			assembly.GetType("Lang").IsPublic = true;
			assembly.GetType("WorldGen").IsPublic = true;
			#endregion

			var ms = new MemoryStream();
			assembly.Write(ms);
			terraria = Assembly.Load(ms.GetBuffer());

			AppDomain.CurrentDomain.AssemblyResolve += (o, args) =>
			{
				string name = args.Name.Split(',')[0];
				if (name == "Terraria")
					return terraria;

				foreach (string dll in Directory.EnumerateFiles("Plugins", "*.dll"))
				{
					try
					{
						if (AssemblyName.GetAssemblyName(dll).FullName == args.Name)
							return Assembly.LoadFrom(dll);
					}
					catch (BadImageFormatException)
					{
					}
				}
				return null;
			};

			// Delete local Terraria.exe copy, if it exists, forcing AssemblyResolve event later on
			File.Delete("Terraria.exe");
			Directory.CreateDirectory("Logs");
			Directory.CreateDirectory("Plugins");
			Directory.CreateDirectory("Raptor");
			Directory.CreateDirectory("Scripts");

			// Separate method so that JIT compiler doesn't get angry at us for referencing Terraria here
			Run(path);
		}
		static void Run(string path)
		{
			using (ClientApi.Main = new Main())
			{
				ClientApi.Initialize();

				ClientApi.Main.Content.RootDirectory = Path.Combine(path, "Content");
				Directory.SetCurrentDirectory(path);

				try
				{
					ClientApi.Main.Run();
				}
				catch (Exception e)
				{
					MessageBox.Show("An unhandled exception occurred: " + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				ClientApi.DeInitialize();
			}
		}
	}
}
