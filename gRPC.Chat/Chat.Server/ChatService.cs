using System;
using System.Threading.Tasks;
using Chat.Abstractions;
using Grpc.Core;

namespace Chat.Server
{
	// Класс реализующий сервис gRPC
	public class ChatService : ChatRoom.ChatRoomBase
	{
		public ChatService(Action<MessageViewModel> getMessageHandle)
		{
			GetMessageHandle = getMessageHandle;
		}

		private ServerCallContext Context { get; set; }
		private IServerStreamWriter<Message> ResponseStream { get; set; }
		private Action<MessageViewModel> GetMessageHandle { get; }

		public async Task SendMessage(Message message)
		{
			if (Context != null && ResponseStream != null && !Context.CancellationToken.IsCancellationRequested)
			{
				await ResponseStream.WriteAsync(message);
				GetMessageHandle(new MessageViewModel(message.Author, message.Text, message.Time));
			}
		}

		public override async Task Join(IAsyncStreamReader<Message> requestStream,
			IServerStreamWriter<Message> responseStream, ServerCallContext context)
		{
			if (Context != null)
			{
				await responseStream.WriteAsync(new Message
				{
					Author = "System", Text = "Abort", Time = DateTime.Now.ToShortTimeString()
				});

				while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested) { }
			}
			else
			{
				Context = context;
				ResponseStream = responseStream;

				while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
				{
					var message = requestStream.Current;
					GetMessageHandle(new MessageViewModel(message.Author, message.Text, message.Time));
				}

				Context = null;
				ResponseStream = null;
			}
		}
	}
}