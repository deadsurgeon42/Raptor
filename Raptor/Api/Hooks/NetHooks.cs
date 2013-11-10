﻿using System;
using System.ComponentModel;
using Terraria;

namespace Raptor.Api.Hooks
{
	#region GetDataEventArgs
	/// <summary>
	/// Event arguments for GetData hooks.
	/// </summary>
	public class GetDataEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the message buffer.
		/// </summary>
		public messageBuffer Msg
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the message packet type.
		/// </summary>
		public PacketTypes MsgID
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the index in the data buffer.
		/// </summary>
		public int Index
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the length of the data in the data buffer.
		/// </summary>
		public int Length
		{
			get;
			private set;
		}

		internal GetDataEventArgs(int index, int length)
		{
			MsgID = (PacketTypes)NetMessage.buffer[256].readBuffer[index];
			Msg = NetMessage.buffer[256];

			Index = index;
			Length = length;
		}
	}
	#endregion
	#region GotDataEventArgs
	/// <summary>
	/// Event arguments for GotData hooks.
	/// </summary>
	public class GotDataEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the message buffer.
		/// </summary>
		public messageBuffer Msg
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the message packet type.
		/// </summary>
		public PacketTypes MsgID
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the index in the data buffer.
		/// </summary>
		public int Index
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the length of the data in the data buffer.
		/// </summary>
		public int Length
		{
			get;
			private set;
		}

		internal GotDataEventArgs(int index, int length)
		{
			MsgID = (PacketTypes)NetMessage.buffer[256].readBuffer[index];
			Msg = NetMessage.buffer[256];

			Index = index;
			Length = length;
		}
	}
	#endregion
	#region SendDataEventArgs
	/// <summary>
	/// Event arguments for SendData hooks.
	/// </summary>
	public class SendDataEventArgs : HandledEventArgs
	{
		/// <summary>
		/// Gets the message packet type.
		/// </summary>
		public PacketTypes MsgId
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the first argument of the message.
		/// </summary>
		public int Number
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the second argument of the message.
		/// </summary>
		public float Number2
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the third argument of the message.
		/// </summary>
		public float Number3
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the fourth argument of the message.
		/// </summary>
		public float Number4
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the fifth argument of the message.
		/// </summary>
		public int Number5
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the message text.
		/// </summary>
		public string Text
		{
			get;
			private set;
		}

		internal SendDataEventArgs(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			MsgId = (PacketTypes)msgId;
			Text = text;
			Number = n1;
			Number2 = n2;
			Number3 = n3;
			Number4 = n4;
			Number5 = n5;
		}
	}
	#endregion
	#region SentDataEventArgs
	/// <summary>
	/// Event arguments for SentData hooks.
	/// </summary>
	public class SentDataEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the message packet type.
		/// </summary>
		public PacketTypes MsgId
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the first argument of the message.
		/// </summary>
		public int Number
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the second argument of the message.
		/// </summary>
		public float Number2
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the third argument of the message.
		/// </summary>
		public float Number3
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the fourth argument of the message.
		/// </summary>
		public float Number4
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the fifth argument of the message.
		/// </summary>
		public int Number5
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the message text.
		/// </summary>
		public string Text
		{
			get;
			private set;
		}

		internal SentDataEventArgs(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			MsgId = (PacketTypes)msgId;
			Text = text;
			Number = n1;
			Number2 = n2;
			Number3 = n3;
			Number4 = n4;
			Number5 = n5;
		}
	}
	#endregion

	/// <summary>
	/// The API's net hooks.
	/// </summary>
	public static class NetHooks
	{
		#region GetData
		/// <summary>
		/// The event that runs when the client receives network data.
		/// </summary>
		public static event EventHandler<GetDataEventArgs> GetData;

		internal static bool InvokeGetData(int index, int length)
		{
			if (Raptor.GetData(index, length))
				return true;

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
		/// The event that runs when the client received network data.
		/// </summary>
		public static event EventHandler<GetDataEventArgs> GotData;

		internal static void InvokeGotData(int index, int length)
		{
			Raptor.GotData(index, length);

			if (GotData != null)
			{
				var args = new GetDataEventArgs(index, length);
				GotData(null, args);
			}
		}
		#endregion

		#region SendData
		/// <summary>
		/// The event that runs when the client sends network data.
		/// </summary>
		public static event EventHandler<SendDataEventArgs> SendData;

		internal static bool InvokeSendData(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			if (SendData == null)
				return false;

			var args = new SendDataEventArgs(msgId, text, n1, n2, n3, n4, n5);
			SendData(null, args);
			return args.Handled;
		}
		#endregion
		#region SentData
		/// <summary>
		/// The event that runs when the client sent network data.
		/// </summary>
		public static event EventHandler<SentDataEventArgs> SentData;

		internal static void InvokeSentData(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			if (SentData == null)
				return;

			var args = new SentDataEventArgs(msgId, text, n1, n2, n3, n4, n5);
			SentData(null, args);
		}
		#endregion
	}
}
