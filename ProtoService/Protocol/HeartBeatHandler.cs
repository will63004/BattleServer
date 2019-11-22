using Google.Protobuf;
using ProtoService.ProtoBuff;
using Protocol;

namespace ProtoService.Protocol
{
    public class HeartBeatHandler
    {
        public IMessage ReqHeartBeat(ulong id, byte[] buffer)
        {
            return ProtocolBuffConvert.Deserialize<HeartBeatReq>(buffer);
        }
    }
}
