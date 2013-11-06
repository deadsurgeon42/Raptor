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
			{ "DrawChat", "NPCChat" },
			{ "DrawPlayerChat", "PlayerChat" },
			{ "DrawInterface", "Interface" },
			{ "DrawMenu", "Menu" },
			{ "DrawMap", "Map" },
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

			var asm = AssemblyDefinition.ReadAssembly(Path.Combine(path, "Terraria.exe"));
			var mod = asm.MainModule;

			try
			{
				// Steam.SteamAPI_Init() check.
				var steamApiInit = asm.GetMethod("Steam", "SteamAPI_Init");
				if (!steamApiInit.IsPInvokeImpl || steamApiInit.PInvokeInfo.Module.Name != "steam_api.dll")
				{
					MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Steam.Init() check.
				var steamInit = asm.GetMethod("Steam", "Init");
				if (!steamInit.HasSameInstructions(0,
					Instruction.Create(OpCodes.Call, steamApiInit),
					Instruction.Create(OpCodes.Stsfld, asm.GetField("Steam", "SteamInit")),
					Instruction.Create(OpCodes.Ret)))
				{
					MessageBox.Show("Sorry, you do not appear to have a legitimate copy of Terraria.", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Program.Main() check.
				bool foundCheck = false;
				var programMain = asm.GetMethod("Program", "Main");
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
			var itemSetDefaults = asm.GetMethod("Item", "SetDefaults", new[] { "Int32", "Boolean" });
			// ItemHooks.InvokeSetDefaults(this);
			itemSetDefaults.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(ItemHooks).GetMethod("InvokeSetDefaults", allFlags))));
			
			var keyinPreFilterMessage = asm.GetType("keyBoardInput").NestedTypes[0].Methods[0];
			// Input.FilterMessage(m); return false;
			keyinPreFilterMessage.InsertStart(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(Input).GetMethod("FilterMessage", allFlags))),
				Instruction.Create(OpCodes.Ldc_I4_0),
				Instruction.Create(OpCodes.Ret));

			#region Draw
			var mainDraw = asm.GetMethod("Main", "Draw");
			// if (GameHooks.InvokeDraw(this.spriteBatch, "")) { base.Draw(gameTime); return; }
			mainDraw.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
				Instruction.Create(OpCodes.Ldstr, ""),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDraw", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, mainDraw.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(Game).GetMethod("Draw", allFlags))),
				Instruction.Create(OpCodes.Ret));
			mainDraw.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
				Instruction.Create(OpCodes.Ldstr, ""),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDrawn", allFlags))));
			
			var mainDrawInterface = asm.GetMethod("Main", "DrawInterface");
			for (int i = mainDrawInterface.Body.Instructions.Count - 1; i >= 0; i--)
			{
				var instr = mainDrawInterface.Body.Instructions[i];

				if (instr.OpCode == OpCodes.Ldfld && ((FieldReference)instr.Operand).Name == "spriteBatch" &&
					instr.Next.OpCode == OpCodes.Ldsfld && ((FieldReference)instr.Next.Operand).Name == "cursorTexture")
				{
					var target = mainDrawInterface.Body.Instructions[i];
					while (target.OpCode != OpCodes.Callvirt || ((MethodReference)target.Operand).Name != "Draw")
						target = target.Next;
					mainDrawInterface.InsertAfter(instr,
						Instruction.Create(OpCodes.Call, mod.Import(typeof(Raptor).GetMethod("DrawCursor", allFlags))),
						Instruction.Create(OpCodes.Br_S, target.Next));
				}
			}

			foreach (KeyValuePair<string, string> kvp in drawHooks)
			{
				var draw = asm.GetMethod("Main", kvp.Key);
				draw.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, kvp.Value),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDraw", allFlags))),
					Instruction.Create(OpCodes.Brfalse_S, draw.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				draw.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, kvp.Value),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDrawn", allFlags))));
			}
			#endregion

			var mainInitialize = asm.GetMethod("Main", "Initialize");
			// GameHooks.InvokeInitialized();
			mainInitialize.InsertEnd(
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeInitialized", allFlags))));
			
			var mainInputText = asm.GetMethod("Main", "GetInputText");
			// return GameHooks.InvokeInputText(oldString);
			mainInputText.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(Input).GetMethod("GetInputText", allFlags))),
				Instruction.Create(OpCodes.Ret));

			var mainLoadContent = asm.GetMethod("Main", "LoadContent");
			// GameHooks.InvokeLoadedContent(this.Content);
			mainLoadContent.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Callvirt, mod.Import(typeof(Game).GetMethod("get_Content"))),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeLoadedContent", allFlags))));
			
			var mainNewText = asm.GetMethod("Main", "NewText");
			// GameHooks.InvokeNewText(text, r, g, b); return;
			mainNewText.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Ldarg_2),
				Instruction.Create(OpCodes.Ldarg_3),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeNewText", allFlags))),
				Instruction.Create(OpCodes.Ret));

			var mainUpdate = asm.GetMethod("Main", "Update");
			// GameHooks.InvokeUpdate();
			mainUpdate.InsertStart(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeUpdate", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, mainUpdate.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(Game).GetMethod("Update", allFlags))),
				Instruction.Create(OpCodes.Ret));

			var disabledKeyboard = mod.Import(typeof(Input).GetField("DisabledKeyboard"));
			var disabledMouse = mod.Import(typeof(Input).GetField("DisabledMouse"));

			for (int i = mainUpdate.Body.Instructions.Count - 1; i >= 0; i--)
			{
				var instr = mainUpdate.Body.Instructions[i];
				if (instr.OpCode == OpCodes.Call && ((MethodReference)instr.Operand).ReturnType.Name == "MouseState")
				{
					mainUpdate.InsertBefore(instr,
						Instruction.Create(OpCodes.Ldsfld, disabledMouse),
						Instruction.Create(OpCodes.Brtrue_S, instr.Next.Next));
				}
				else if (instr.OpCode == OpCodes.Call && ((MethodReference)instr.Operand).ReturnType.Name == "KeyboardState")
				{
					mainUpdate.InsertBefore(instr,
						Instruction.Create(OpCodes.Ldsfld, disabledKeyboard),
						Instruction.Create(OpCodes.Brtrue_S, instr.Next.Next));
				}
			}

			// GameHooks.InvokeUpdated();
			mainUpdate.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeUpdated", allFlags))));
			
			var messageBufferGetData = asm.GetMethod("messageBuffer", "GetData");
			// if (NetHooks.InvokeGetData(start, length)) return;
			messageBufferGetData.InsertStart(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Ldarg_2),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeGetData", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, messageBufferGetData.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ret));

			// NetHooks.InvokeGotData(start, length);
			messageBufferGetData.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_1),
				Instruction.Create(OpCodes.Ldarg_2),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeGotData", allFlags))));
			
			var netMessageSendData = asm.GetMethod("NetMessage", "SendData");
			// if (NetHooks.InvokeSendData(msgType, text, number, number2, number3, number4, number5)) return;
			netMessageSendData.InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_3),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[4]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[5]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[6]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[7]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[8]),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeSendData", allFlags))),
				Instruction.Create(OpCodes.Brfalse_S, netMessageSendData.Body.Instructions[0]),
				Instruction.Create(OpCodes.Ret));

			// NetHooks.InvokeSentData(msgType, text, number, number2, number3, number4, number5));
			netMessageSendData.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Ldarg_3),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[4]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[5]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[6]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[7]),
				Instruction.Create(OpCodes.Ldarg_S, netMessageSendData.Parameters[8]),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeSentData", allFlags))));
			
			// NpcHooks.InvokeProcessAI(this);
			asm.GetMethod("NPC", "AI").InsertStart(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeProcessAI", allFlags))));

			var npcSetDefaults = asm.GetMethod("NPC", "SetDefaults", new[] { "Int32", "Single" });
			// NpcHooks.InvokeSetDefaults(this);
			npcSetDefaults.InsertEnd(
				Instruction.Create(OpCodes.Ldarg_0),
				Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeSetDefaults", allFlags))));
			
			// return;
			asm.GetMethod("Steam", "Kill").InsertStart(
				Instruction.Create(OpCodes.Ret));

			// Force everything public
			foreach (var type in mod.Types)
			{
				type.IsPublic = true;
				foreach (var field in type.Fields)
					field.IsPublic = true;
				foreach (var method in type.Methods)
					method.IsPublic = true;
			}

			GameHooks.InvokeILModified(asm);
			#endregion

			var ms = new MemoryStream();
			asm.Write(ms);
#if DEBUG
			asm.Write("debug.exe");
#endif
			terraria = Assembly.Load(ms.GetBuffer());

			AppDomain.CurrentDomain.AssemblyResolve += (o, args) =>
			{
				if (args.Name.Split(',')[0] == "Terraria")
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
			Directory.CreateDirectory("Raptor").CreateSubdirectory("Scripts");

			ClientApi.Initialize();
			Run(path);
			ClientApi.DeInitialize();
		}
		static void Run(string path)
		{
			using (ClientApi.Main = new Main())
			{
				try
				{
					ClientApi.Main.Content.RootDirectory = Path.Combine(path, "Content");
					Directory.SetCurrentDirectory(path);
					ClientApi.Main.Run();
				}
				catch (Exception e)
				{
					MessageBox.Show("An unhandled exception occurred: " + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
