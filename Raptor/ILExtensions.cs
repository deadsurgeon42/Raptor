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
		static readonly Dictionary<OpCode, OpCode> shortToLong = new Dictionary<OpCode, OpCode>
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
		internal static List<MethodDefinition> shortToLongMethods = new List<MethodDefinition>();

		/// <summary>
		/// Creates a shallow copy of an instruction.
		/// </summary>
		/// <param name="instr">The instruction.</param>
		public static Instruction Copy(this Instruction instr)
		{
			switch (instr.OpCode.OperandType)
			{
				case OperandType.InlineArg:
				case OperandType.ShortInlineArg:
					return Instruction.Create(instr.OpCode, (ParameterDefinition)instr.Operand);
				case OperandType.InlineBrTarget:
				case OperandType.ShortInlineBrTarget:
					return Instruction.Create(instr.OpCode, (Instruction)instr.Operand);
				case OperandType.InlineField:
					return Instruction.Create(instr.OpCode, (FieldReference)instr.Operand);
				case OperandType.InlineI:
					return Instruction.Create(instr.OpCode, (int)instr.Operand);
				case OperandType.InlineI8:
					return Instruction.Create(instr.OpCode, (long)instr.Operand);
				case OperandType.InlineMethod:
					return Instruction.Create(instr.OpCode, (MethodReference)instr.Operand);
				case OperandType.InlineNone:
					return Instruction.Create(instr.OpCode);
				case OperandType.InlineR:
					return Instruction.Create(instr.OpCode, (double)instr.Operand);
				case OperandType.InlineString:
					return Instruction.Create(instr.OpCode, (string)instr.Operand);
				case OperandType.InlineSwitch:
					return Instruction.Create(instr.OpCode, (Instruction[])instr.Operand);
				case OperandType.InlineType:
					return Instruction.Create(instr.OpCode, (TypeDefinition)instr.Operand);
				case OperandType.InlineVar:
				case OperandType.ShortInlineVar:
					return Instruction.Create(instr.OpCode, (VariableDefinition)instr.Operand);

				case OperandType.ShortInlineI:
					return Instruction.Create(instr.OpCode, (sbyte)instr.Operand);
				case OperandType.ShortInlineR:
					return Instruction.Create(instr.OpCode, (float)instr.Operand);

				case OperandType.InlineTok:
					if (instr.Operand is FieldDefinition)
						return Instruction.Create(instr.OpCode, (FieldReference)instr.Operand);
					if (instr.Operand is MethodDefinition)
						return Instruction.Create(instr.OpCode, (MethodReference)instr.Operand);
					return Instruction.Create(instr.OpCode, (TypeReference)instr.Operand);
			}
			return null;
		}
		/// <summary>
		/// Fixes all short branches.
		/// </summary>
		public static void FixShortBranches()
		{
			foreach (MethodDefinition md in shortToLongMethods)
				md.FixShortBranches();
		}
		/// <summary>
		/// Fixes short branches in a method.
		/// </summary>
		/// <param name="md">The method.</param>
		public static void FixShortBranches(this MethodDefinition md)
		{
			foreach (Instruction instr in md.Body.Instructions)
			{
				if (instr.OpCode.OperandType == OperandType.ShortInlineBrTarget)
					instr.OpCode = shortToLong[instr.OpCode];
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
		/// Inserts instructions after a target in a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="target">The instruction target.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertAfter(this MethodDefinition md, Instruction target, params Instruction[] instructions)
		{
			var ilp = md.Body.GetILProcessor();
			for (int j = instructions.Length - 1; j >= 0; j--)
				ilp.InsertAfter(target, instructions[j]);

			if (!shortToLongMethods.Contains(md))
				shortToLongMethods.Add(md);
		}
		/// <summary>
		/// Inserts instructions before a target in a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="target">The instruction target.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertBefore(this MethodDefinition md, Instruction target, params Instruction[] instructions)
		{
			var ilp = md.Body.GetILProcessor();

			ilp.InsertAfter(target, target.Copy());
			target.OpCode = OpCodes.Nop;

			for (int i = instructions.Length - 1; i >= 0; i--)
				ilp.InsertAfter(target, instructions[i]);

			if (!shortToLongMethods.Contains(md))
				shortToLongMethods.Add(md);
		}
		/// <summary>
		/// Inserts instructions at the end(s) of a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertEnd(this MethodDefinition md, params Instruction[] instructions)
		{
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

			if (!shortToLongMethods.Contains(md))
				shortToLongMethods.Add(md);
		}
		/// <summary>
		/// Inserts instructions at the start of a method.
		/// </summary>
		/// <param name="md">The method.</param>
		/// <param name="instructions">The instructions.</param>
		public static void InsertStart(this MethodDefinition md, params Instruction[] instructions)
		{
			Instruction instr = md.Body.Instructions[0];
			var ilp = md.Body.GetILProcessor();
			foreach (Instruction i in instructions)
				ilp.InsertBefore(instr, i);
		}
	}
}
