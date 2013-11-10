using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raptor.Api;
using Raptor.Api.TShock;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// Contains various utility methods.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Gets whether the client has the specified TShock permission.
		/// </summary>
		/// <param name="permission">The permission.</param>
		/// <returns>Whether the client has the permission.</returns>
		public static bool HasTShockPermission(string permission)
		{
			var permissions = new List<string> { permission };
			string[] nodes = permission.Split('.');
			for (int i = nodes.Length - 1; i >= 0; i--)
			{
				nodes[i] = "*";
				permissions.Add(String.Join(".", nodes, 0, i + 1));
			}

			foreach (string p in Raptor.NegatedPermissions)
			{
				foreach (string p2 in permissions)
				{
					if (String.Equals(p, p2, StringComparison.OrdinalIgnoreCase))
						return false;
				}
			}

			foreach (string p in Raptor.Permissions)
			{
				foreach (string p2 in permissions)
				{
					if (String.Equals(p, p2, StringComparison.OrdinalIgnoreCase))
						return true;
				}				
			}
			return false;
		}
		/// <summary>
		/// Prints an error message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewErrorText(string format, params object[] args)
		{
			Main.NewText(String.Format(format, args), 255, 0, 0);
		}
		/// <summary>
		/// Prints an info message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewInfoText(string format, params object[] args)
		{
			Main.NewText(String.Format(format, args), 255, 255, 0);
		}
		/// <summary>
		/// Prints a success message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewSuccessText(string format, params object[] args)
		{
			Main.NewText(String.Format(format, args), 0, 128, 0);
		}
		/// <summary>
		/// Sends the Acknowledge packet.
		/// </summary>
		public static void SendAcknowledge()
		{
			ClientSock cs = Netplay.clientSock;
			byte[] data = new byte[] { 2, 0, 0, 0, (byte)PacketTypes.Raptor, (byte)RaptorPacketTypes.Acknowledge };
			cs.networkStream.BeginWrite(data, 0, 6, cs.ClientWriteCallBack, cs.networkStream);
		}
		/// <summary>
		/// Sends the Region packet.
		/// </summary>
		/// <param name="region">The region to send.</param>
		public static void SendRegion(Region region)
		{
			lock (NetMessage.buffer[256].writeBuffer)
			{
				int length = 0;

				using (var writer = new BinaryWriter(new MemoryStream(NetMessage.buffer[256].writeBuffer, true)))
				{
					writer.BaseStream.Position = 4;

					writer.Write((byte)PacketTypes.Raptor);
					writer.Write((byte)RaptorPacketTypes.Region);

					writer.Write(region.Area.X);
					writer.Write(region.Area.Y);
					writer.Write(region.Area.Width - 1);
					writer.Write(region.Area.Height - 1);
					writer.Write(region.Name);

					length = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = 0;
					writer.Write(length - 4);
				}

				ClientSock cs = Netplay.clientSock;
				cs.networkStream.BeginWrite(NetMessage.buffer[256].writeBuffer, 0, length, cs.ClientWriteCallBack, cs.networkStream);
			}
		}
		/// <summary>
		/// Sends the RegionDelete packet.
		/// </summary>
		/// <param name="region">The region to delete.</param>
		public static void SendRegionDelete(Region region)
		{
			lock (NetMessage.buffer[256].writeBuffer)
			{
				int length = 0;

				using (var writer = new BinaryWriter(new MemoryStream(NetMessage.buffer[256].writeBuffer, true)))
				{
					writer.BaseStream.Position = 4;

					writer.Write((byte)PacketTypes.Raptor);
					writer.Write((byte)RaptorPacketTypes.RegionDelete);

					writer.Write(region.Name);

					length = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = 0;
					writer.Write(length - 4);
				}

				ClientSock cs = Netplay.clientSock;
				cs.networkStream.BeginWrite(NetMessage.buffer[256].writeBuffer, 0, length, cs.ClientWriteCallBack, cs.networkStream);
			}
		}
		/// <summary>
		/// Sends the Warp packet.
		/// </summary>
		/// <param name="warp">The warp to send.</param>
		public static void SendWarp(Warp warp)
		{
			lock (NetMessage.buffer[256].writeBuffer)
			{
				int length = 0;

				using (var writer = new BinaryWriter(new MemoryStream(NetMessage.buffer[256].writeBuffer, true)))
				{
					writer.BaseStream.Position = 4;

					writer.Write((byte)PacketTypes.Raptor);
					writer.Write((byte)RaptorPacketTypes.Warp);

					writer.Write(warp.Position.X);
					writer.Write(warp.Position.Y);
					writer.Write(warp.Name);

					length = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = 0;
					writer.Write(length - 4);
				}

				ClientSock cs = Netplay.clientSock;
				cs.networkStream.BeginWrite(NetMessage.buffer[256].writeBuffer, 0, length, cs.ClientWriteCallBack, cs.networkStream);
			}
		}
		/// <summary>
		/// Sends the WarpDelete packet.
		/// </summary>
		/// <param name="warp">The warp to delete.</param>
		public static void SendWarpDelete(Warp warp)
		{
			lock (NetMessage.buffer[256].writeBuffer)
			{
				int length = 0;

				using (var writer = new BinaryWriter(new MemoryStream(NetMessage.buffer[256].writeBuffer, true)))
				{
					writer.BaseStream.Position = 4;

					writer.Write((byte)PacketTypes.Raptor);
					writer.Write((byte)RaptorPacketTypes.WarpDelete);

					writer.Write(warp.Name);

					length = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = 0;
					writer.Write(length - 4);
				}

				ClientSock cs = Netplay.clientSock;
				cs.networkStream.BeginWrite(NetMessage.buffer[256].writeBuffer, 0, length, cs.ClientWriteCallBack, cs.networkStream);
			}
		}
	}
}
