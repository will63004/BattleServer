using Google.Protobuf;
using Protocol;
using System;

namespace ProtoService.ProtoBuff
{
    public interface IProtocolParser
    {
        void RegisterReceiveMessage(ProtoID protoID, Action<ulong, IMessage> onReceiveMessage);
        void UnregisterReceiveMessage(ProtoID protoID, Action<ulong, IMessage> onReceiveMessage);

        void Send(ulong id, IMessage message);
        void SendAll(IMessage message);
    }
}
