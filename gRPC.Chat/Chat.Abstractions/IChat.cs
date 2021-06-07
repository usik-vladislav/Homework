using System.Threading.Tasks;

namespace Chat.Abstractions
{
	public interface IChat
	{
		string Name { get; set; }
		Task SendMessageAsync(MessageViewModel message);
		Task DisconnectAsync();
	}
}
