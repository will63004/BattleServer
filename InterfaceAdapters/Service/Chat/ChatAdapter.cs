using Game.Service.Chat;

namespace InterfaceAdapters.Service.Chat
{
    public class ChatAdapter
    {
        private IChat chat;

        public ChatAdapter(IChat chat)
        {
            this.chat = chat;
        }

        public void AddMessage(ulong playerID, string content)
        {
            chat.AddMessage(playerID, content);
        }
    }
}
