//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2014 MarioE
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
using System.IO;
using System.Linq;
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
		const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		static Dictionary<string, string> drawHooks = new Dictionary<string, string>
		{
			{ "Draw", "" },
			{ "DrawInterface", "Interface" },
			{ "DrawInventory", "Inventory" },
			{ "DrawMap", "Map" },
			{ "DrawMenu", "Menu" },
			{ "DrawChat", "NPCChat" },
			{ "DrawPlayerChat", "PlayerChat" },
			{ "DrawTiles", "Tiles" },
			{ "DrawWalls", "Walls" },
			{ "DrawWires", "Wires" },
		};
		static Assembly terraria;
		const string PIRACY_MSG = "You do not appear to have a legitimate copy of Terraria. If this is not the case, then try re-installing it and then running it";
		const string REGISTRY = @"SOFTWARE\Re-Logic\Terraria";
		
		[STAThread]
		static void Main()
		{
			RegistryKey rk = Registry.LocalMachine.OpenSubKey(REGISTRY);
			if (rk == null)
			{
				MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string path = (string)rk.GetValue("Install_Path", null);

			#region Piracy checks
			if (path == null || !File.Exists(Path.Combine(path, "Terraria.exe")))
			{
				MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (File.Exists("Terraria.exe"))
			{
				MessageBox.Show("Do not run this program in the same folder as Terraria.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Steam.Init() check.
				var steamInit = asm.GetMethod("Steam", "Init");
				if (!steamInit.HasSameInstructions(0,
					Instruction.Create(OpCodes.Call, steamApiInit),
					Instruction.Create(OpCodes.Stsfld, asm.GetField("Steam", "SteamInit")),
					Instruction.Create(OpCodes.Ret)))
				{
					MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			catch
			{
				// Exception here means piracy
				MessageBox.Show(PIRACY_MSG, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			#endregion

			#region Item
			{
				var itemSetDefaults = asm.GetMethod("Item", "SetDefaults", new[] { "Int32", "Boolean" });
				// ItemHooks.InvokeSetDefaults(this);
				itemSetDefaults.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(ItemHooks).GetMethod("InvokeSetDefaults", FLAGS))));
			}
			#endregion
			#region keyBoardInput
			{
				// Input.FilterMessage(m); return false;
				var filterMessage = asm.GetType("keyBoardInput").NestedTypes[0].Methods[0];
				filterMessage.InsertStart(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(Input).GetMethod("FilterMessage", FLAGS))),
					Instruction.Create(OpCodes.Ldc_I4_0),
					Instruction.Create(OpCodes.Ret));
			}
			#endregion
			#region Lighting
			{
				// Single core lighting
				var doColors = asm.GetMethod("Lighting", "doColors");
				for (int i = doColors.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = doColors.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Callvirt && ((MethodReference)instr.Operand).Name == "Invoke")
					{
						var target = instr;
						while (target.OpCode != OpCodes.Ldfld || ((FieldReference)target.Operand).Name != "function")
							target = target.Previous;
						
						// LightingHooks.InvokeColor(Lighting.swipe);
						doColors.InsertBefore(target,
							Instruction.Create(OpCodes.Dup),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColor", FLAGS))));
						break;
					}
				}

				// Multicore lighting
				var callback = asm.GetMethod("Lighting", "callback_LightingSwipe");
				for (int i = callback.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = callback.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Callvirt && ((MethodReference)instr.Operand).Name == "Invoke")
					{
						var target = instr;
						while (target.OpCode != OpCodes.Ldfld || ((FieldReference)target.Operand).Name != "function")
							target = target.Previous;

						// LightingHooks.InvokeColor(Lighting.swipe);
						callback.InsertBefore(target,
							Instruction.Create(OpCodes.Dup),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColor", FLAGS))));
						break;
					}
				}
			}
			#endregion
			#region Main
			{
				var drawInterface = asm.GetMethod("Main", "DrawInterface");
				for (int i = drawInterface.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = drawInterface.Body.Instructions[i];

					if (instr.OpCode == OpCodes.Ldsfld && ((FieldReference)instr.Operand).Name == "spriteBatch" &&
						instr.Next.OpCode == OpCodes.Ldsfld && (((FieldReference)instr.Next.Operand).Name == "cursorTexture" || ((FieldReference)instr.Next.Operand).Name == "cursor2Texture"))
					{
						var target = instr;
						while (target.OpCode != OpCodes.Callvirt || ((MethodReference)target.Operand).Name != "Draw")
							target = target.Next;

						// Raptor.DrawCursor();
						drawInterface.InsertAfter(instr,
							Instruction.Create(OpCodes.Call, mod.Import(typeof(Raptor).GetMethod("DrawCursor", FLAGS))),
							Instruction.Create(OpCodes.Br_S, target.Next));
					}
				}

				foreach (KeyValuePair<string, string> kvp in drawHooks)
				{
					var method = asm.GetMethod("Main", kvp.Key);
					method.InsertStart(
						Instruction.Create(OpCodes.Ldarg_0),
						Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
						Instruction.Create(OpCodes.Ldstr, kvp.Value),
						Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDraw", FLAGS))),
						Instruction.Create(OpCodes.Brfalse_S, method.Body.Instructions[0]),
						Instruction.Create(OpCodes.Ret));
					method.InsertEnd(
						Instruction.Create(OpCodes.Ldarg_0),
						Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
						Instruction.Create(OpCodes.Ldstr, kvp.Value),
						Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDrawn", FLAGS))));
				}

				// GameHooks.InvokeInitialized();
				asm.GetMethod("Main", "Initialize").InsertEnd(
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeInitialized", FLAGS))));

				// return GameHooks.InvokeInputText(oldString);
				asm.GetMethod("Main", "GetInputText").InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(Input).GetMethod("GetInputText", FLAGS))),
					Instruction.Create(OpCodes.Ret));

				// GameHooks.InvokeLoadedContent(this.Content);
				asm.GetMethod("Main", "LoadContent").InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Callvirt, mod.Import(typeof(Game).GetMethod("get_Content"))),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeLoadedContent", FLAGS))));

				// GameHooks.InvokeNewText(text, r, g, b); return;
				asm.GetMethod("Main", "NewText").InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Ldarg_2),
					Instruction.Create(OpCodes.Ldarg_3),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeNewText", FLAGS))),
					Instruction.Create(OpCodes.Ret));

				// I know this is weird, but FindWaterfalls() is called in the perfectly...
				// ... right spot to manipulate Main.screenPosition.
				// Simon311.
				asm.GetMethod("WaterfallManager", "FindWaterfalls").InsertStart( 
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeCamera", FLAGS)))
				);

				var update = asm.GetMethod("Main", "Update");
				// GameHooks.InvokeUpdate();
				update.InsertStart(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeUpdate", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, update.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(Game).GetMethod("Update", FLAGS))),
					Instruction.Create(OpCodes.Ret));

				var disabledKeyboard = mod.Import(typeof(Input).GetField("DisabledKeyboard"));
				var disabledMouse = mod.Import(typeof(Input).GetField("DisabledMouse"));

				for (int i = update.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = update.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Call && ((MethodReference)instr.Operand).ReturnType.Name == "MouseState")
					{
						update.InsertBefore(instr,
							Instruction.Create(OpCodes.Ldsfld, disabledMouse),
							Instruction.Create(OpCodes.Brtrue_S, instr.Next.Next));
					}
					else if (instr.OpCode == OpCodes.Call && ((MethodReference)instr.Operand).ReturnType.Name == "KeyboardState")
					{
						update.InsertBefore(instr,
							Instruction.Create(OpCodes.Ldsfld, disabledKeyboard),
							Instruction.Create(OpCodes.Brtrue_S, instr.Next.Next));
					}
				}

				// GameHooks.InvokeUpdated();
				update.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeUpdated", FLAGS))));
			}
			#endregion
			#region MessageBuffer
			{
				var getData = asm.GetMethod("MessageBuffer", "GetData");
				// if (NetHooks.InvokeGetData(start, length)) return;
				getData.InsertStart(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Ldarg_2),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeGetData", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, getData.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));

				// NetHooks.InvokeGotData(start, length);
				getData.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Ldarg_2),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeGotData", FLAGS))));
			}
			#endregion
			#region NetMessage
			{
				var sendData = asm.GetMethod("NetMessage", "SendData");
				// if (NetHooks.InvokeSendData(msgType, text, number, number2, number3, number4, number5)) return;
				sendData.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_3),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[4]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[5]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[6]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[7]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[8]),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeSendData", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, sendData.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));

				// NetHooks.InvokeSentData(msgType, text, number, number2, number3, number4, number5));
				sendData.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_3),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[4]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[5]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[6]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[7]),
					Instruction.Create(OpCodes.Ldarg_S, sendData.Parameters[8]),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NetHooks).GetMethod("InvokeSentData", FLAGS))));
			}
			#endregion
			#region NPC
			{
				var ai = asm.GetMethod("NPC", "AI");
				// if (NPCHooks.InvokeProcessAI(this)) return;
				ai.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeProcessAI", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, ai.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var npcLoot = asm.GetMethod("NPC", "NPCLoot");
				// if (NPCHooks.InvokeDropLoot(this)) return;
				npcLoot.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeDropLoot", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, npcLoot.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var setDefaults = asm.GetMethod("NPC", "SetDefaults", new[] { "Int32", "Single" });
				// NpcHooks.InvokeSetDefaults(this);
				setDefaults.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeSetDefaults", FLAGS))));
			}
			#endregion
			#region Player
			{
				var hurt = asm.GetMethod("Player", "Hurt");
				hurt.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Ldarg_3),
					Instruction.Create(OpCodes.Ldarg_S, hurt.Parameters[5]),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeHurt", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, hurt.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ldc_R8, 0.0),
					Instruction.Create(OpCodes.Ret));

				var killMe = asm.GetMethod("Player", "KillMe");
				killMe.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Ldarg_3),
					Instruction.Create(OpCodes.Ldarg_S, killMe.Parameters[3]),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeKill", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, killMe.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));

				var load = asm.GetMethod("Player", "LoadPlayer");
				for (int i = 0; i < load.Body.Instructions.Count; i++)
				{
					var instr = load.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Callvirt && ((MethodReference)instr.Operand).Name == "Close")
					{
						// PlayerHooks.InvokeLoaded(binaryReader);
						load.InsertBefore(instr,
							Instruction.Create(OpCodes.Dup),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeLoaded", FLAGS))));
						break;
					}
				}

				var resetEffects = asm.GetMethod("Player", "ResetEffects");
				// PlayerHooks.InvokeUpdateVars();
				resetEffects.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdateVars", FLAGS))));

				var save = asm.GetMethod("Player", "SavePlayer");
				// PlayerHooks.InvokeSave();
				save.InsertStart(
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeSave", FLAGS))));
				for (int i = 0; i < save.Body.Instructions.Count; i++)
				{
					var instr = save.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Callvirt && ((MethodReference)instr.Operand).Name == "Close")
					{
						// PlayerHooks.InvokeSaved(binaryWriter);
						save.InsertBefore(instr,
							Instruction.Create(OpCodes.Dup),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeSaved", FLAGS))));
						break;
					}
				}

				var update = asm.GetMethod("Player", "UpdatePlayer");
				// PlayerHooks.InvokeUpdate(this);
				update.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdate", FLAGS))));
				// PlayerHooks.InvokeUpdated(this);
				update.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdated", FLAGS))));

				var updateJumpHeight = asm.GetMethod("Player", "UpdateJumpHeight");
				updateJumpHeight.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdatedVars", FLAGS))));
			}
			#endregion
			#region Projectile
			{
				var ai = asm.GetMethod("Projectile", "AI");
				// if (ProjectileHooks.InvokeProcessAI(this)) return;
				ai.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(ProjectileHooks).GetMethod("InvokeProcessAI", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, ai.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));

				// ProjectileHooks.InvokeKill(this);
				asm.GetMethod("Projectile", "Kill").InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(ProjectileHooks).GetMethod("InvokeKill", FLAGS))));

				// ProjectileHooks.InvokeSetDefaults(this);
				asm.GetMethod("Projectile", "SetDefaults").InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(ProjectileHooks).GetMethod("InvokeSetDefaults", FLAGS))));
			}
			#endregion
			#region Steam
			{
				// return;
				asm.GetMethod("Steam", "Kill").InsertStart(
					Instruction.Create(OpCodes.Ret));
			}
			#endregion

			ILExtensions.FixShortBranches();

			// Force everything public
			var types = new Queue<TypeDefinition>(mod.Types);
			while (types.Count > 0)
			{
				var type = types.Dequeue();
				if (type.IsNested)
				{
					type.IsNestedPublic = true;
					foreach (var field in type.Fields)
						field.IsPublic = true;
					foreach (var method in type.Methods)
						method.IsPublic = true;
				}
				else
				{
					type.IsPublic = true;
					foreach (var field in type.Fields)
						field.IsPublic = true;
					foreach (var method in type.Methods)
						method.IsPublic = true;
				}

				foreach (var type2 in type.NestedTypes)
					types.Enqueue(type2);
			}

			using (var ms = new MemoryStream())
			{
				asm.Write(ms);
#if DEBUG
				asm.Write("debug.exe");
#endif
				terraria = Assembly.Load(ms.ToArray());
			}
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

			Directory.CreateDirectory("Logs");
			Directory.CreateDirectory("Plugins");
			Directory.CreateDirectory("Scripts");

			Log.Initialize();
			Log.LogInfo("Raptor v{0} stopped.\n", ClientApi.ApiVersion);
			ClientApi.Initialize();
			Run(path);
			Raptor.DeInitialize();
			ClientApi.DeInitialize();
			Log.LogInfo("Raptor v{0} stopped.\n", ClientApi.ApiVersion);
			Log.DeInitialize();
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
					Log.LogFatal("An unhandled exception occured:");
					Log.LogFatal(e.ToString());
					MessageBox.Show("An unhandled exception occurred: " + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
