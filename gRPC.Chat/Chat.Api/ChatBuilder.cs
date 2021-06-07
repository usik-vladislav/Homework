using System;
using System.Threading.Tasks;
using Chat.Abstractions;
using Chat.Client;
using Chat.Server;

namespace Chat.Api
{
	public static class ChatFactory
	{
		public static IChat CreateChatServer(int port, string name, 
			Action<MessageViewModel> getMessageHandle, Action disconnectHandle)
		{
			return new ChatServer(port, name, getMessageHandle, disconnectHandle);
		}

		public static async Task<IChat> CreateChatClient(string ip, int port, string name, 
			Action<MessageViewModel> getMessageHandle, Action disconnectHandle)
		{
			var chatClient = new ChatClient(ip, port, name);
			await chatClient.SetHandlers(getMessageHandle, disconnectHandle).ConfigureAwait(false);
			return chatClient;
		}
	}
}
