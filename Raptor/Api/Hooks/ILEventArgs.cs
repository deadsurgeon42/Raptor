using System;
using Mono.Cecil;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// Event arguments for IL hooks.
	/// </summary>
	public class ILEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the assembly definition.
		/// </summary>
		public AssemblyDefinition Assembly
		{
			get;
			internal set;
		}
	}
}
