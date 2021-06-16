using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Chat.Abstractions;
using Chat.Api;

namespace Chat.Window
{
	public class ChatViewModel : INotifyPropertyChanged
	{
		private IChat _chat;

		private string _chatText;

		private string _inputText;

		private string _ip;

		private bool _isConnected;

		private int _port;

		public string Ip
		{
			get => _ip;
			set
			{
				_ip = value;
				OnPropertyChanged(nameof(EndPoint));
			}
		}

		public int Port
		{
			get => _port;
			set
			{
				_port = value;
				OnPropertyChanged(nameof(EndPoint));
			}
		}

		public string EndPoint => IsServer ? Ip + ":" + Port : "";

		public bool IsConnected
		{
			get => _isConnected;
			set
			{
				_isConnected = value;
				if (!_isConnected)
				{
					IsClient = false;
					IsServer = false;
				}

				OnPropertyChanged(nameof(IsConnected));
				OnPropertyChanged(nameof(IsDisconnected));
			}
		}

		public bool IsDisconnected => !_isConnected;

		public string ChatText
		{
			get => _chatText;
			set
			{
				_chatText = value;
				OnPropertyChanged(nameof(ChatText));
			}
		}

		public string UserName { get; set; }

		public bool IsClient { get; set; }

		public bool IsServer { get; set; }

		public string InputText
		{
			get => _inputText;
			set
			{
				_inputText = value;
				OnPropertyChanged(nameof(InputText));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public async Task CreateChat()
		{
			_chat = !IsClient
				? ChatFactory.CreateChatServer(Port, UserName, WriteMessageHandle, DisconnectHandle)
				: await ChatFactory.CreateChatClient(Ip, Port, UserName, WriteMessageHandle, DisconnectHandle);
			IsConnected = true;
		}

		public async Task SendMessageAsync()
		{
			var message = new MessageViewModel(UserName, InputText);
			await _chat.SendMessageAsync(message).ConfigureAwait(false);
			InputText = "";
		}

		public async Task DisconnectAsync()
		{
			await _chat.DisconnectAsync();
		}

		private void DisconnectHandle()
		{
			IsConnected = false;
			ChatText = "";
			Ip = "";
			Port = 0;
			_chat = null;
		}

		private void WriteMessageHandle(MessageViewModel message)
		{
			ChatText += message.Time + " " + message.Author + ": " + message.Text + "\r\n";
		}
	}
}