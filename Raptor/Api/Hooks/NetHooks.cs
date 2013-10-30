using System;
using Terraria;

namespace Raptor.Api.Hooks
{
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
			if (GetData != null)
			{
				byte msgId = NetMessage.buffer[256].readBuffer[index];

				GetDataEventArgs args = new GetDataEventArgs
				{
					MsgID = (PacketTypes)msgId,
					Msg = NetMessage.buffer[256],
					Index = index,
					Length = length
				};
				GetData(null, args);
				return args.Handled;
			}
			return false;
		}
		#endregion

		#region SendData
		/// <summary>
		/// The event that runs when the client sends network data.
		/// </summary>
		public static event EventHandler<SendDataEventArgs> SendData;

		internal static bool InvokeSendData(int msgId, string text, int n1, float n2, float n3, float n4, int n5)
		{
			if (Raptor.SendData(msgId, text, n1, n2, n3, n4, n5))
				return true;

			if (SendData == null)
				return false;

			SendDataEventArgs args = new SendDataEventArgs
			{
				MsgId = (PacketTypes)msgId,
				Text = text,
				Number = n1,
				Number2 = n2,
				Number3 = n3,
				Number4 = n4,
				Number5 = n5
			};

			SendData(null, args);

			return args.Handled;
		}
		#endregion
	}
}
