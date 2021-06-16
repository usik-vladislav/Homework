using System;
using System.Windows;

namespace Chat.Window
{
	public partial class MainWindow
	{
		private readonly ChatViewModel _chatViewModel;

		public MainWindow()
		{
			InitializeComponent();
			_chatViewModel = new ChatViewModel();
			DataContext = _chatViewModel;
		}

		private void StartServer_Click(object sender, RoutedEventArgs e)
		{
			_chatViewModel.IsServer = true;
			var portInput = new InputPort(_chatViewModel);
			portInput.Show();
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			_chatViewModel.IsClient = true;
			var portInput = new InputPort(_chatViewModel);
			portInput.Show();
		}

		private async void Disconnect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				await _chatViewModel.DisconnectAsync();
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
				await _chatViewModel.SendMessageAsync();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}
	}
}