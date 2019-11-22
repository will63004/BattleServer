namespace Game.Service.Chat
{
    public interface IChat
    {
        int MaxMessageCount { get; }
        void AddMessage(ulong playerID, string content);
        int MessageCount { get; }
    }
}
