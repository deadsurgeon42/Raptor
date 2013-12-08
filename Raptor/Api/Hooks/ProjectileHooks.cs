//  Raptor - a client API for Terraria
//  Copyright (C) 2013 MarioE
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	/// The API's projectile hooks.
	/// </summary>
	public static class ProjectileHooks
	{
		#region ProcessAI
		/// <summary>
		/// Event arguments for ProcessAI hooks.
		/// </summary>
		public class ProcessAIEventArgs : HandledEventArgs
		{
			/// <summary>
			/// Gets the NPC instance.
			/// </summary>
			public Projectile Projectile { get; private set; }
			internal ProcessAIEventArgs(Projectile projectile)
			{
				Projectile = projectile;
			}
		}
		/// <summary>
		/// The event that runs when an projectile's AI is processed.
		/// </summary>
		public static event EventHandler<ProcessAIEventArgs> ProcessAI;
		internal static bool InvokeProcessAI(object projectile)
		{
			if (ProcessAI != null)
			{
				var args = new ProcessAIEventArgs((Projectile)projectile);
				ProcessAI(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region Kill
		/// <summary>
		/// Event arguments for Kill hooks.
		/// </summary>
		public class KillEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the NPC instance.
			/// </summary>
			public Projectile Projectile { get; private set; }
			internal KillEventArgs(Projectile projectile)
			{
				Projectile = projectile;
			}
		}
		/// <summary>
		/// The event that runs when an projectile is killed.
		/// </summary>
		public static event EventHandler<KillEventArgs> Kill;
		internal static void InvokeKill(object projectile)
		{
			if (Kill != null)
				Kill(null, new KillEventArgs((Projectile)projectile));
		}
		#endregion
		#region SetDefaults
		/// <summary>
		/// Event arguments for SetDefaults hooks.
		/// </summary>
		public class SetDefaultsEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the projectile instance.
			/// </summary>
			public Projectile Projectile { get; private set; }
			internal SetDefaultsEventArgs(Projectile projectile)
			{
				Projectile = projectile;
			}
		}
		/// <summary>
		/// The event that runs when an projectile's default values are set.
		/// </summary>
		public static event EventHandler<SetDefaultsEventArgs> SetDefaults;
		internal static void InvokeSetDefaults(object projectile)
		{
			if (SetDefaults != null)
				SetDefaults(null, new SetDefaultsEventArgs((Projectile)projectile));
		}
		#endregion
	}
}
