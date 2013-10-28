using System;
using System.IO;

namespace Raptor
{
	/// <summary>
	/// Manages the logs.
	/// </summary>
	public static class Log
	{
		static string LogPath { get { return Path.Combine("Logs", DateTime.Now.ToString(@"yyyy-MM-dd.\tx\t")); } }
		
		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogError(string format, params object[] args)
		{
			using (var writer = new StreamWriter(LogPath, true))
			{
				writer.WriteLine("[{0}]   ({1}) {2}", "ERROR", DateTime.Now.ToString("hh:mm:ss"), String.Format(format, args));
			}
		}
		/// <summary>
		/// Logs a fatal error.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogFatal(string format, params object[] args)
		{
			using (var writer = new StreamWriter(LogPath, true))
			{
				writer.WriteLine("[{0}]   ({1}) {2}", "FATAL", DateTime.Now.ToString("hh:mm:ss"), String.Format(format, args));
			}
		}
		/// <summary>
		/// Logs a notice.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		public static void LogNotice(string format, params object[] args)
		{
			using (var writer = new StreamWriter(LogPath, true))
			{
				writer.WriteLine("[{0}]  ({1}) {2}", "NOTICE", DateTime.Now.ToString("hh:mm:ss"), String.Format(format, args));
			}
		}
	}
}
