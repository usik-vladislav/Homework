using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.Window
{
	public partial class InputPort : System.Windows.Window
	{
		private readonly bool _isServer;

		public InputPort(bool isServer)
		{
			_isServer = isServer;
			InitializeComponent();
			IpText.Visibility = !_isServer ? Visibility.Visible : Visibility.Collapsed;
			IpInput.Visibility = !_isServer ? Visibility.Visible : Visibility.Collapsed;
		}

		private async void BOk_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var owner = (MainWindow) Owner;
				owner.Port = int.Parse(PortInput.Text);
				owner.UserName = NameInput.Text;
				if (!_isServer)
				{
					owner.Ip = IpInput.Text;
				}
				else
				{
					using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
					socket.Connect("8.8.8.8", 65530);
					IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
					owner.IpText.Text = endPoint?.Address + ":" + PortInput.Text;
				}
				await owner.CreateChat(_isServer);
				Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}
	}
}
