using System.Threading.Tasks;

namespace Chat.Abstractions
{
	//Интерфейс чата. Позволяет задавать имя, отправлять сообщения и отключаться.
	public interface IChat
	{
		string Name { get; set; }
		Task SendMessageAsync(MessageViewModel message);
		Task DisconnectAsync();
	}
}