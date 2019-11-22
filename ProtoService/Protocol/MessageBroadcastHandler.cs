using Google.Protobuf;
using ProtoService.ProtoBuff;

namespace ProtoService.Protocol
{
    public class MessageBroadcastHandler
    {
        public void AckMessageBroadcast(ulong playerID, IMessage message)
        {
            ProtocolBuffConvert.Serialize(message);
        }
    }
}
