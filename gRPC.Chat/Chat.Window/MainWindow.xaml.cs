using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Chat.Abstractions;
using Chat.Api;

namespace Chat.Window
{
	public partial class MainWindow : System.Windows.Window
	{
		public string Ip { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }

		private IChat _chat;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void StartServer_Click(object sender, RoutedEventArgs e)
		{
			var portInput = new InputPort(true);
			portInput.Owner = this;
			portInput.Show();
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			var portInput = new InputPort(false);
			portInput.Owner = this;
			portInput.Show();
		}

		private void DisconnectHandle()
		{
			Dispatcher.Invoke(new ThreadStart(() =>
			{
				BStartServer.IsEnabled = true;
				BConnect.IsEnabled = true;
				BDisconnect.IsEnabled = false;
				BSend.IsEnabled = false;
				ChatText.Text = "";
				IpText.Text = "";
				_chat = null;
			}));
		}

		private async void Disconnect_Click(object sender, RoutedEventArgs e)
		{
			try 
			{
				await _chat.DisconnectAsync();
				MessageBox.Show("Вы были отключены.");
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		private async void Send_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var message = new MessageViewModel(UserName, MessageTb.Text);
				await _chat.SendMessageAsync(message);
				MessageTb.Clear();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		public async Task CreateChat(bool isServer)
		{
			try
			{
				_chat = isServer
					? ChatFactory.CreateChatServer(Port, UserName, WriteMessageHandle, DisconnectHandle)
					: await ChatFactory.CreateChatClient(Ip, Port, UserName, WriteMessageHandle, DisconnectHandle);
				BStartServer.IsEnabled = false;
				BConnect.IsEnabled = false;
				BDisconnect.IsEnabled = true;
				BSend.IsEnabled = true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void WriteMessageHandle(MessageViewModel message)
		{
			Dispatcher.Invoke(new ThreadStart(() =>
				WriteMessage(message.Time + " " + message.Author + ": " + message.Text + "\r\n")));
		}

		private void WriteMessage(string message)
		{
			ChatText.Text += message;
		}
	}
}
