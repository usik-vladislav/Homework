using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Chat.Window
{
	public partial class InputPort
	{
		private readonly ChatViewModel _chatViewModel;

		public InputPort(ChatViewModel chatViewModel)
		{
			_chatViewModel = chatViewModel;
			InitializeComponent();
			DataContext = _chatViewModel;
			IpText.Visibility = _chatViewModel.IsClient ? Visibility.Visible : Visibility.Collapsed;
			IpInput.Visibility = _chatViewModel.IsClient ? Visibility.Visible : Visibility.Collapsed;
		}

		private async void BOk_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				_chatViewModel.Port = int.Parse(PortInput.Text);
				_chatViewModel.UserName = NameInput.Text;

				if (_chatViewModel.IsClient)
				{
					_chatViewModel.Ip = IpInput.Text;
				}
				else
				{
					using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
					socket.Connect("8.8.8.8", 65530);
					var endPoint = socket.LocalEndPoint as IPEndPoint;
					_chatViewModel.Ip = endPoint?.Address.ToString();
				}

				await _chatViewModel.CreateChat();
				Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}
	}
}