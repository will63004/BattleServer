using Google.Protobuf;
using ProtoService.ProtoBuff;
using Protocol;

namespace ProtoService.Protocol
{
    public class LoginHandler
    {
        public IMessage ReqLogin(ulong id, byte[] buffer)
        {
            return ProtocolBuffConvert.Deserialize<LoginReq>(buffer);
        }

        public void AckLogin(ulong playerID, IMessage message)
        {
             ProtocolBuffConvert.Serialize(message);
        }
    }
}
