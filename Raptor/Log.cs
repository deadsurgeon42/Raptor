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
using System.IO;

namespace Raptor
{
	/// <summary>
	/// Manages the logs.
	/// </summary>
	public static class Log
	{
		static StreamWriter Writer;

		internal static void DeInitialize()
		{
			Writer.Dispose();
		}
		internal static void Initialize()
		{
			Writer = new StreamWriter(Path.Combine("Logs", DateTime.Now.ToString(@"yyyy-MM-dd_hh-mm-ss.lo\g")));
		}
		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogError(string format, params object[] args)
		{
			Writer.WriteLine("[{0}] ERROR: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), String.Format(format, args));
		}
		/// <summary>
		/// Logs a fatal error.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogFatal(string format, params object[] args)
		{
			Writer.WriteLine("[{0}] FATAL: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), String.Format(format, args));
		}
		/// <summary>
		/// Logs information.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogInfo(string format, params object[] args)
		{
			Writer.WriteLine("[{0}]  INFO: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), String.Format(format, args));
		}
	}
}
