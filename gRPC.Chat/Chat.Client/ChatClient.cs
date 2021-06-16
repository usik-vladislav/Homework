using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Abstractions;
using Grpc.Core;

namespace Chat.Client
{
	//Объект чата-клиента, подключается к серверу через gRPC
	public class ChatClient : IChat, IDisposable
	{
		public ChatClient(string ip, int port, string name)
		{
			Name = name;
			Channel = new Channel(ip, port, ChannelCredentials.Insecure);
			var client = new ChatRoom.ChatRoomClient(Channel);
			Chat = client.Join();
		}

		private Channel Channel { get; }
		private AsyncDuplexStreamingCall<Message, Message> Chat { get; }
		private Action<MessageViewModel> GetMessageHandle { get; set; }
		public string Name { get; set; }

		public async Task SendMessageAsync(MessageViewModel messageViewModel)
		{
			var message = new Message
			{
				Author = messageViewModel.Author,
				Text = messageViewModel.Text,
				Time = messageViewModel.Time
			};
			await Chat.RequestStream.WriteAsync(message);
			GetMessageHandle(new MessageViewModel(message.Author, message.Text, message.Time));
		}

		public async Task DisconnectAsync()
		{
			await SendMessageAsync(new MessageViewModel("System", $"{Name} отключился.")).ConfigureAwait(false);
			await Chat.RequestStream.CompleteAsync();
			Chat.Dispose();
			await Channel.ShutdownAsync();
		}

		public void Dispose()
		{
			Chat.Dispose();
		}

		public async Task SetHandlers(Action<MessageViewModel> getMessageHandle, Action disconnectHandle)
		{
			GetMessageHandle = getMessageHandle;
			await SendMessageAsync(new MessageViewModel("System", $"{Name} подключился.")).ConfigureAwait(false);

			_ = Task.Run(async () =>
			{
				while (Channel.State != ChannelState.Shutdown)
				{
					if (!await Chat.ResponseStream.MoveNext(CancellationToken.None)) continue;
					var message = Chat.ResponseStream.Current;
					if (message.Author == "System" && message.Text == "Abort") break;
					GetMessageHandle(new MessageViewModel(message.Author, message.Text, message.Time));
				}

				await DisconnectAsync();
				disconnectHandle();
			});
		}
	}
}