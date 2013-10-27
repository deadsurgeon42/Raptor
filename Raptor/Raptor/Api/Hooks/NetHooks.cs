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

		internal static bool InvokeSendData(ref int msgId, ref string text, ref int number, ref float number2, ref float number3, ref float number4, ref int number5)
		{
			if (SendData == null)
			{
				return false;
			}

			SendDataEventArgs args = new SendDataEventArgs
			{
				MsgId = (PacketTypes)msgId,
				Text = text,
				Number = number,
				Number2 = number2,
				Number3 = number3,
				Number4 = number4,
				Number5 = number5
			};

			SendData(null, args);

			msgId = (int)args.MsgId;
			text = args.Text;
			number = args.Number;
			number2 = args.Number2;
			number3 = args.Number3;
			number4 = args.Number4;
			number5 = args.Number5;
			return args.Handled;
		}
		#endregion
	}
}
