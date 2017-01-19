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
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;

namespace Raptor.Api.Hooks
{
	/// <summary>
	///   The API's net hooks.
	/// </summary>
	public static class NetHooks
	{
		#region GetData

		/// <summary>
		///   Event arguments for GetData hooks.
		/// </summary>
		public class GetDataEventArgs : HandledEventArgs
		{
			private readonly BinaryReader reader;

			internal GetDataEventArgs(int index, int length)
			{
				MsgID = (PacketTypes) NetMessage.buffer[256].readBuffer[index];
				Msg = NetMessage.buffer[256];

				Index = index;
				Length = length;
				reader = new BinaryReader(new MemoryStream(NetMessage.buffer[256].readBuffer, index + 1, length - 1));
			}

			/// <summary>
			///   Gets the message buffer.
			/// </summary>
			public MessageBuffer Msg { get; private set; }

			/// <summary>
			///   Gets the message packet type.
			/// </summary>
			public PacketTypes MsgID { get; private set; }

			/// <summary>
			///   Gets the index in the data buffer.
			/// </summary>
			public int Index { get; private set; }

			/// <summary>
			///   Gets the length of the data in the data buffer.
			/// </summary>
			public int Length { get; private set; }

			~GetDataEventArgs()
			{
				reader.Dispose();
			}

			/// <summary>
			///   Reads a byte.
			/// </summary>
			/// <returns>The byte.</returns>
			public byte ReadByte()
			{
				return reader.ReadByte();
			}

			/// <summary>
			///   Reads an XNA Color.
			/// </summary>
			/// <returns>The XNA Color.</returns>
			public Color ReadColor()
			{
				return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
			}

			/// <summary>
			///   Reads a double.
			/// </summary>
			/// <returns>The double.</returns>
			public double ReadDouble()
			{
				return reader.ReadDouble();
			}

			/// <summary>
			///   Reads a short.
			/// </summary>
			/// <returns>The short.</returns>
			public short ReadInt16()
			{
				return reader.ReadInt16();
			}

			/// <summary>
			///   Reads an int.
			/// </summary>
			/// <returns>The int.</returns>
			public int ReadInt32()
			{
				return reader.ReadInt32();
			}

			/// <summary>
			///   Reads a single (float).
			/// </summary>
			/// <returns>The single (float).</returns>
			public float ReadSingle()
			{
				return reader.ReadSingle();
			}

			/// <summary>
			///   Reads a string.
			/// </summary>
			/// <returns>The string.</returns>
			public string ReadString()
			{
				return reader.ReadString();
			}

			/// <summary>
			///   Reads a Vector2.
			/// </summary>
			/// <returns>The Vector2.</returns>
			public Vector2 ReadVector2()
			{
				return new Vector2(reader.ReadSingle(), reader.ReadSingle());
			}
		}

		/// <summary>
		///   The event that runs when the client receives network data.
		/// </summary>
		public static event EventHandler<GetDataEventArgs> GetData;

		internal static bool InvokeGetData(int index, int length)
		{
			if (GetData != null)
			{
				var args = new GetDataEventArgs(index, length);
				GetData(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region GotData

		/// <summary>
		///   Event arguments for GotData hooks.
		/// </summary>
		public class GotDataEventArgs : EventArgs
		{
			private readonly BinaryReader reader;

			internal GotDataEventArgs(int index, int length)
			{
				MsgID = (PacketTypes) NetMessage.buffer[256].readBuffer[index];
				Msg = NetMessage.buffer[256];

				Index = index;
				Length = length;
				reader = new BinaryReader(new MemoryStream(NetMessage.buffer[256].readBuffer, index, length));
			}

			/// <summary>
			///   Gets the message buffer.
			/// </summary>
			public MessageBuffer Msg { get; private set; }

			/// <summary>
			///   Gets the message packet type.
			/// </summary>
			public PacketTypes MsgID { get; private set; }

			/// <summary>
			///   Gets the index in the data buffer.
			/// </summary>
			public int Index { get; private set; }

			/// <summary>
			///   Gets the length of the data in the data buffer.
			/// </summary>
			public int Length { get; private set; }

			~GotDataEventArgs()
			{
				reader.Dispose();
			}

			/// <summary>
			///   Reads a byte.
			/// </summary>
			/// <returns>The byte.</returns>
			public byte ReadByte()
			{
				return reader.ReadByte();
			}

			/// <summary>
			///   Reads an XNA Color.
			/// </summary>
			/// <returns>The XNA Color.</returns>
			public Color ReadColor()
			{
				return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
			}

			/// <summary>
			///   Reads a double.
			/// </summary>
			/// <returns>The double.</returns>
			public double ReadDouble()
			{
				return reader.ReadDouble();
			}

			/// <summary>
			///   Reads a short.
			/// </summary>
			/// <returns>The short.</returns>
			public short ReadInt16()
			{
				return reader.ReadInt16();
			}

			/// <summary>
			///   Reads an int.
			/// </summary>
			/// <returns>The int.</returns>
			public int ReadInt32()
			{
				return reader.ReadInt32();
			}

			/// <summary>
			///   Reads a single (float).
			/// </summary>
			/// <returns>The single (float).</returns>
			public float ReadSingle()
			{
				return reader.ReadSingle();
			}

			/// <summary>
			///   Reads a string.
			/// </summary>
			/// <returns>The string.</returns>
			public string ReadString()
			{
				return reader.ReadString();
			}

			/// <summary>
			///   Reads a Vector2.
			/// </summary>
			/// <returns>The Vector2.</returns>
			public Vector2 ReadVector2()
			{
				return new Vector2(reader.ReadSingle(), reader.ReadSingle());
			}
		}

		/// <summary>
		///   The event that runs when the client received network data.
		/// </summary>
		public static event EventHandler<GetDataEventArgs> GotData;

		internal static void InvokeGotData(int index, int length)
		{
			if (GotData != null)
			{
				var args = new GetDataEventArgs(index, length);
				GotData(null, args);
			}
		}

		#endregion

		#region SendData

		/// <summary>
		///   Event arguments for SendData hooks.
		/// </summary>
		public class SendDataEventArgs : HandledEventArgs
		{
			internal SendDataEventArgs(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
			{
				MsgId = (PacketTypes) msgId;
				Text = text;
				Number = n1;
				Number2 = n2;
				Number3 = n3;
				Number4 = n4;
				Number5 = n5;
			}

			/// <summary>
			///   Gets the message packet type.
			/// </summary>
			public PacketTypes MsgId { get; private set; }

			/// <summary>
			///   Gets the first argument of the message.
			/// </summary>
			public int Number { get; private set; }

			/// <summary>
			///   Gets the second argument of the message.
			/// </summary>
			public float Number2 { get; private set; }

			/// <summary>
			///   Gets the third argument of the message.
			/// </summary>
			public float Number3 { get; private set; }

			/// <summary>
			///   Gets the fourth argument of the message.
			/// </summary>
			public float Number4 { get; private set; }

			/// <summary>
			///   Gets the fifth argument of the message.
			/// </summary>
			public int Number5 { get; private set; }

			/// <summary>
			///   Gets the message text.
			/// </summary>
			public string Text { get; private set; }
		}

		/// <summary>
		///   The event that runs when the client sends network data.
		/// </summary>
		public static event EventHandler<SendDataEventArgs> SendData;

		internal static bool InvokeSendData(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			if (SendData != null)
			{
				var args = new SendDataEventArgs(msgId, text, n1, n2, n3, n4, n5);
				SendData(null, args);
				return args.Handled;
			}
			return false;
		}

		#endregion

		#region SentData

		/// <summary>
		///   Event arguments for SendData hooks.
		/// </summary>
		public class SentDataEventArgs : EventArgs
		{
			internal SentDataEventArgs(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
			{
				MsgId = (PacketTypes) msgId;
				Text = text;
				Number = n1;
				Number2 = n2;
				Number3 = n3;
				Number4 = n4;
				Number5 = n5;
			}

			/// <summary>
			///   Gets the message packet type.
			/// </summary>
			public PacketTypes MsgId { get; private set; }

			/// <summary>
			///   Gets the first argument of the message.
			/// </summary>
			public int Number { get; private set; }

			/// <summary>
			///   Gets the second argument of the message.
			/// </summary>
			public float Number2 { get; private set; }

			/// <summary>
			///   Gets the third argument of the message.
			/// </summary>
			public float Number3 { get; private set; }

			/// <summary>
			///   Gets the fourth argument of the message.
			/// </summary>
			public float Number4 { get; private set; }

			/// <summary>
			///   Gets the fifth argument of the message.
			/// </summary>
			public int Number5 { get; private set; }

			/// <summary>
			///   Gets the message text.
			/// </summary>
			public string Text { get; private set; }
		}

		/// <summary>
		///   The event that runs when the client sent network data.
		/// </summary>
		public static event EventHandler<SentDataEventArgs> SentData;

		internal static void InvokeSentData(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			if (SentData != null)
			{
				var args = new SentDataEventArgs(msgId, text, n1, n2, n3, n4, n5);
				SentData(null, args);
			}
		}

		#endregion
	}
}