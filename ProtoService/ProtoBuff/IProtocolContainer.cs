using Google.Protobuf;
using Protocol;
using System;

namespace ProtoService.ProtoBuff
{
    public interface IProtocolContainer
    {
        bool TryGetValue(ProtoID protoID, out Func<ulong, byte[], IMessage> value);
    }
}
