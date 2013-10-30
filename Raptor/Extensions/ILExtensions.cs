using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Raptor
{
	/// <summary>
	/// Contains IL extensions.
	/// </summary>
	public static class ILExtensions
	{
		static Dictionary<OpCode, OpCode> shortToLong = new Dictionary<OpCode,OpCode>
		{
			{ OpCodes.Beq_S, OpCodes.Beq },
			{ OpCodes.Bge_S, OpCodes.Bge },
			{ OpCodes.Bge_Un_S, OpCodes.Bge_Un },
			{ OpCodes.Bgt_S, OpCodes.Bgt },
			{ OpCodes.Bgt_Un_S, OpCodes.Bgt_Un },
			{ OpCodes.Ble_S, OpCodes.Ble },
			{ OpCodes.Ble_Un_S, OpCodes.Ble_Un },
			{ OpCodes.Blt_S, OpCodes.Blt },
			{ OpCodes.Blt_Un_S, OpCodes.Blt_Un },
			{ OpCodes.Bne_Un_S, OpCodes.Bne_Un },
			{ OpCodes.Br_S, OpCodes.Br },
			{ OpCodes.Brfalse_S, OpCodes.Brfalse },
			{ OpCodes.Brtrue_S, OpCodes.Brtrue },
			{ OpCodes.Leave_S, OpCodes.Leave }
		};
		/// <summary>
		/// Fixes short branches in a method.
		/// </summary>
		/// <param name="md">The method.</param>
		public static void FixShortBranches(this MethodDefinition md)
		{
			for (int i = 0; i < md.Body.Instructions.Count; i++)
			{
				var instr = md.Body.Instructions[i];
				if (instr.OpCode.OperandType == OperandType.ShortInlineBrTarget)
				{
					int offset = 0;
					int target = md.Body.Instructions.IndexOf((Instruction)instr.Operand);

					if (target > i)
					{
						for (int j = i + 1; j < target; j++)
							offset += md.Body.Instructions[j].GetSize();
					}
					else
					{
						for (int j = i; j >= target; j--)
							offset -= md.Body.Instructions[j].GetSize();
					}

					// Short branches can only go 127 positive and 128 negative.
					if (offset < -128 || offset > 127)
						instr.OpCode = shortToLong[instr.OpCode];
				}
			}
		}
		/// <summary>
		/// Gets a field from the assembly.
		/// </summary>
		/// <param name="asm">The assembly to get a field from.</param>
		/// <param name="type">The type name that the field is in.</param>
		/// <param name="field">The field name.</param>
		public static FieldDefinition GetField(this AssemblyDefinition asm, string type, string field)
		{
			return asm.MainModule.Types.First(td => td.Name == type).Fields.First(fd => fd.Name == field);
		}
		/// <summary>
		/// Gets a method from the assembly.
		/// </summary>
		/// <param name="asm">The assembly to get a method from.</param>
		/// <param name="type">The type name.</param>
		/// <param name="method">The method name.</param>
		/// <param name="param">The parameters' types.</param>
		public static MethodDefinition GetMethod(this AssemblyDefinition asm, string type, string method, string[] param = null)
		{
			return asm.MainModule.Types.First(td => td.Name == type).Methods.First
				(md => md.Name == method && (param == null || md.Parameters.Select(p => p.ParameterType.Name).SequenceEqual(param)));
		}
		/// <summary>
		/// Gets a type from the assembly.
		/// </summary>
		/// <param name="asm">The assembly to get a method from.</param>
		/// <param name="type">The type name.</param>
		public static TypeDefinition GetType(this AssemblyDefinition asm, string type)
		{
			return asm.MainModule.Types.First(td => td.Name == type);
		}
		/// <summary>
		/// Checks if a method has the same instructions as the ones supplied.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="index">The index.</param>
		/// <param name="instructions">The instructions.</param>
		public static bool HasSameInstructions(this MethodDefinition md, int index, params Instruction[] instructions)
		{
			for (int i = index; i < index + instructions.Length; i++)
			{
				var instr = md.Body.Instructions[i];
				if (instr.OpCode != instructions[i - index].OpCode ||
					(instr.Operand != instructions[i - index].Operand &&
					instr.Operand.ToString() != instructions[i - index].Operand.ToString()))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Inserts instructions at the end(s) of a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertEnd(this MethodDefinition md, params Instruction[] instructions)
		{
			var ret = Instruction.Create(OpCodes.Ret);

			for (int i = md.Body.Instructions.Count - 1; i >= 0; i--)
			{
				var instr = md.Body.Instructions[i];
				if (instr.OpCode == OpCodes.Ret)
				{
					var ilp = md.Body.GetILProcessor();
					ilp.InsertAfter(instr, Instruction.Create(OpCodes.Ret));
					for (int j = instructions.Length - 1; j >= 0; j--)
						ilp.InsertAfter(instr, instructions[j]);
					instr.OpCode = OpCodes.Nop;
				}
			}

			md.FixShortBranches();
		}
		/// <summary>
		/// Inserts instructions at the start of a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertStart(this MethodDefinition md, params Instruction[] instructions)
		{
			Instruction instr = md.Body.Instructions[0];
			foreach (Instruction i in instructions)
			{
				md.Body.GetILProcessor().InsertBefore(instr, i);
			}
		}
	}
}
