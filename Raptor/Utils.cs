using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raptor.Api;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// Contains various utility methods.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Prints an error message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewErrorText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 255, 0, 0);
		}
		/// <summary>
		/// Prints an info message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewInfoText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 255, 255, 0);
		}
		/// <summary>
		/// Prints a success message.
		/// </summary>
		/// <param name="msg">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewSuccessText(string msg, params object[] args)
		{
			Main.NewText(String.Format(msg, args), 0, 128, 0);
		}
		/// <summary>
		/// Sends custom packet data.
		/// </summary>
		/// <param name="msg">The custom packet type.</param>
		/// <param name="text">The text to send.</param>
		public static void SendCustomData(RaptorPacketTypes msg, string text = "")
		{
			ClientSock cs = Netplay.clientSock;
			switch (msg)
			{
				case RaptorPacketTypes.Acknowledge:
					{
						byte[] data = new byte[] { 2, 0, 0, 0, (byte)PacketTypes.Placeholder, (byte)msg };
						cs.networkStream.BeginWrite(data, 0, 6, cs.ClientWriteCallBack, cs.networkStream);
					}
					break;
			}
		}
	}
}
