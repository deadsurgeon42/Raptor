//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
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
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Raptor.Api.Hooks
{
	/// <summary>
	///   The API's game hooks.
	/// </summary>
	public static class GameHooks
	{
		#region Camera

		/// <summary>
		///   The event that runs after the game has already calculated screenPosition,
		///   but didn't start drawing anything yet.
		/// </summary>
		public static event EventHandler<EventArgs> Camera;

		internal static void InvokeCamera()
		{
			if (Camera != null)
				Camera(null, new EventArgs());
		}

		#endregion

		#region Initialized

		/// <summary>
		///   The event that runs after the game is initialized.
		/// </summary>
		public static event EventHandler Initialized;

		internal static void InvokeInitialized()
		{
			Raptor.Initialize();

			if (Initialized != null)
				Initialized(null, EventArgs.Empty);
		}

		#endregion

		#region LoadedContent

		/// <summary>
		///   Event arguments for LoadedContent hooks.
		/// </summary>
		public class LoadedContentEventArgs : EventArgs
		{
			internal LoadedContentEventArgs(ContentManager cm)
			{
				Content = cm;
			}

			/// <summary>
			///   Gets the content manager used for loading content.
			/// </summary>
			public ContentManager Content { get; private set; }
		}

		/// <summary>
		///   The event that runs after the game loaded content.
		/// </summary>
		public static event EventHandler<LoadedContentEventArgs> LoadedContent;

		internal static void InvokeLoadedContent(ContentManager cm)
		{
			Raptor.LoadedContent(cm);

			if (LoadedContent != null)
				LoadedContent(null, new LoadedContentEventArgs(cm));
		}

		#endregion

		#region Update

		/// <summary>
		///   Event arguments for Update hooks.
		/// </summary>
		public class UpdateEventArgs : HandledEventArgs
		{
			internal UpdateEventArgs(GameTime gt)
			{
				GameTime = gt;
			}

			/// <summary>
			///   Gets the game time.
			/// </summary>
			public GameTime GameTime { get; private set; }
		}

		/// <summary>
		///   The event that runs before the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdateEventArgs> Update;

		internal static bool InvokeUpdate(GameTime gt)
		{
			Raptor.Update();

			if (Update != null)
			{
				var args = new UpdateEventArgs(gt);
				Update(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region Updated

		/// <summary>
		///   Event arguments for Updated hooks.
		/// </summary>
		public class UpdatedEventArgs : EventArgs
		{
			internal UpdatedEventArgs(GameTime gt)
			{
				GameTime = gt;
			}

			/// <summary>
			///   Gets the game time.
			/// </summary>
			public GameTime GameTime { get; private set; }
		}

		/// <summary>
		///   The event that runs after the game is updated each frame.
		/// </summary>
		public static event EventHandler<UpdatedEventArgs> Updated;

		internal static void InvokeUpdated(GameTime gt)
		{
			if (Updated != null)
				Updated(null, new UpdatedEventArgs(gt));
		}

		#endregion
	}
}