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
