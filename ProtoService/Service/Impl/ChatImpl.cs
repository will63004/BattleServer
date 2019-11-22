using Grpc.Core;
using InterfaceAdapters.Service.Chat;
using Protocol;
using System.Threading.Tasks;

namespace ProtoService.Service.Impl
{
    public class ChatImpl: EchoChat.EchoChatBase
    {
        private ChatAdapter chatAdapter;
        public ChatImpl(ChatAdapter chatAdapter)
        {
            this.chatAdapter = chatAdapter;
        }
        public override Task<ChatAck> SendChat(ChatReq request, ServerCallContext context)
        {
            chatAdapter.AddMessage(request.Header.PlayerID, request.Content);
            return Task.FromResult(new ChatAck { Content = request.Content });
        }
    }
}
