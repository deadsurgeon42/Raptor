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
		const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		static Dictionary<string, string> drawHooks = new Dictionary<string, string>
		{
			{ "DrawInterface", "Interface" },
			{ "DrawInventory", "Inventory" },
			{ "DrawMap", "Map" },
			{ "DrawMenu", "Menu" },
			{ "DrawChat", "NPCChat" },
			{ "DrawPlayerChat", "PlayerChat" },
		};
		static Assembly terraria;
		const string PIRACY_MSG = "You do not appear to have a legitimate copy of Terraria. If this is not the case, perhaps try re-installing.";
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
				asm.GetType("keyBoardInput").NestedTypes[0].Methods[0].InsertStart(
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(Input).GetMethod("FilterMessage", FLAGS))),
					Instruction.Create(OpCodes.Ldc_I4_0),
					Instruction.Create(OpCodes.Ret));
			}
			#endregion
			#region Lighting
			{
				var doColors = asm.GetMethod("Lighting", "doColors");
				for (int i = doColors.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = doColors.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Callvirt && ((MethodReference)instr.Operand).Name == "Restart")
					{
						doColors.InsertAfter(instr,
							Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColor", FLAGS))));
					}
				}

				var colorR = asm.GetMethod("Lighting", "LightColor");
				colorR.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorR", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorR.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var colorR2 = asm.GetMethod("Lighting", "LightColor2");
				colorR2.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorR", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorR2.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var colorG = asm.GetMethod("Lighting", "LightColorG");
				colorG.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorG", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorG.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var colorG2 = asm.GetMethod("Lighting", "LightColorG2");
				colorG2.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorG", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorG2.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var colorB = asm.GetMethod("Lighting", "LightColorB");
				colorB.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorB", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorB.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
				var colorB2 = asm.GetMethod("Lighting", "LightColorB2");
				colorB2.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(LightingHooks).GetMethod("InvokeColorB", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, colorB2.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ret));
			}
			#endregion
			#region Main
			{
				var draw = asm.GetMethod("Main", "Draw");
				// if (GameHooks.InvokeDraw(this.spriteBatch, "")) { base.Draw(gameTime); return; }
				draw.InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, ""),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDraw", FLAGS))),
					Instruction.Create(OpCodes.Brfalse_S, draw.Body.Instructions[0]),
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldarg_1),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(Game).GetMethod("Draw", FLAGS))),
					Instruction.Create(OpCodes.Ret));
				// GameHooks.InvokeDrawn(this.spriteBatch, "");
				draw.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Ldfld, asm.GetField("Main", "spriteBatch")),
					Instruction.Create(OpCodes.Ldstr, ""),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(GameHooks).GetMethod("InvokeDrawn", FLAGS))));

				var drawInterface = asm.GetMethod("Main", "DrawInterface");
				for (int i = drawInterface.Body.Instructions.Count - 1; i >= 0; i--)
				{
					var instr = drawInterface.Body.Instructions[i];

					if (instr.OpCode == OpCodes.Ldfld && ((FieldReference)instr.Operand).Name == "spriteBatch" &&
						instr.Next.OpCode == OpCodes.Ldsfld && ((FieldReference)instr.Next.Operand).Name == "cursorTexture")
					{
						var target = drawInterface.Body.Instructions[i];
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
			#region messageBuffer
			{
				var getData = asm.GetMethod("messageBuffer", "GetData");
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
				// NpcHooks.InvokeProcessAI(this);
				asm.GetMethod("NPC", "AI").InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(NpcHooks).GetMethod("InvokeProcessAI", FLAGS))));

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
				for (int i = 0; i < update.Body.Instructions.Count; i++)
				{
					var instr = update.Body.Instructions[i];
					if (instr.OpCode == OpCodes.Stfld && ((FieldReference)instr.Operand).Name == "rangedCrit" &&
						instr.Previous.OpCode == OpCodes.Add)
					{
						// PlayerHooks.InvokeUpdateVars(this);
						update.InsertAfter(instr,
							Instruction.Create(OpCodes.Ldarg_0),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdateVars", FLAGS))));
					}
					if (instr.OpCode == OpCodes.Stfld && ((FieldReference)instr.Operand).Name == "lifeRegenCount")
					{
						// PlayerHooks.InvokeUpdatedVars(this);
						update.InsertAfter(instr,
							Instruction.Create(OpCodes.Ldarg_0),
							Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdatedVars", FLAGS))));
						break;
					}
				}
				// PlayerHooks.InvokeUpdated(this);
				update.InsertEnd(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(PlayerHooks).GetMethod("InvokeUpdated", FLAGS))));
			}
			#endregion
			#region Projectile
			{
				// ProjectileHooks.InvokeProcessAI(this);
				asm.GetMethod("Projectile", "AI").InsertStart(
					Instruction.Create(OpCodes.Ldarg_0),
					Instruction.Create(OpCodes.Call, mod.Import(typeof(ProjectileHooks).GetMethod("InvokeProcessAI", FLAGS))));

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
			// Force everything public
			foreach (var type in mod.Types)
			{
				type.IsPublic = true;
				foreach (var field in type.Fields)
					field.IsPublic = true;
				foreach (var method in type.Methods)
					method.IsPublic = true;
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
			ClientApi.Initialize();
			Run(path);
			Raptor.DeInitialize();
			ClientApi.DeInitialize();
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
