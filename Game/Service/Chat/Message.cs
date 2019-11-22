namespace Game.Service.Chat
{
    public class Message
    {
        public ulong PlayerID { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
