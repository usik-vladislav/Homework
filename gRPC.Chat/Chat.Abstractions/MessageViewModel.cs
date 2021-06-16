using System;

namespace Chat.Abstractions
{
	public class MessageViewModel
	{
		public MessageViewModel(string author, string text)
		{
			Author = author;
			Text = text;
			Time = DateTime.Now.ToShortTimeString();
		}

		public MessageViewModel(string author, string text, string time)
		{
			Author = author;
			Text = text;
			Time = time;
		}

		public string Time { get; }
		public string Author { get; }
		public string Text { get; }
	}
}