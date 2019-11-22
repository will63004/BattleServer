using System.Collections.Generic;

namespace Game.Service.Chat
{
    public class Chat: IChat
    {
        public int MaxMessageCount { get; private set; }

        private readonly List<Message> messages = new List<Message>();

        private readonly MessageObjectPool op = new MessageObjectPool();

        public Chat(int maxMessageCount)
        {
            MaxMessageCount = maxMessageCount;
        }

        public void AddMessage(ulong playerID, string content)
        {
            if(MessageCount >= MaxMessageCount)
                messages.RemoveAt(0);

            Message ms = op.Get();
            ms.PlayerID = playerID;
            ms.Content = content;
            messages.Add(ms);
        }

        public int MessageCount
        {
            get { return messages.Count; }
        }
    }
}
