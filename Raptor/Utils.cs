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
using System.IO;
using System.Linq;
using System.Text;
using Raptor.Api;
using Raptor.Api.TShock;
using Terraria;

namespace Raptor
{
	/// <summary>
	/// Contains various utility getters/setters and methods.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Gets the local player.
		/// </summary>
		public static Player LocalPlayer
		{
			get { return Main.player[Main.myPlayer]; }
		}
		/// <summary>
		/// Prints an error message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewErrorText(string format, params object[] args)
		{
			if (!Main.gameMenu)
				Main.NewText(String.Format(format, args), 255, 0, 0);
		}
		/// <summary>
		/// Prints an info message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewInfoText(string format, params object[] args)
		{
			if (!Main.gameMenu)
				Main.NewText(String.Format(format, args), 255, 255, 0);
		}
		/// <summary>
		/// Prints a success message.
		/// </summary>
		/// <param name="format">The message format.</param>
		/// <param name="args">The arguments.</param>
		public static void NewSuccessText(string format, params object[] args)
		{
			if (!Main.gameMenu)
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
