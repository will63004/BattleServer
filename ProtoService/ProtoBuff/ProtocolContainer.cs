using Google.Protobuf;
using ProtoService.Protocol;
using Protocol;
using System;
using System.Collections.Generic;

namespace ProtoService.ProtoBuff
{
    public class ProtocolContainer: IProtocolContainer
    {
        private Dictionary<ProtoID, Func<ulong, byte[], IMessage>> m_protoContainer = new Dictionary<ProtoID, Func<ulong,byte[], IMessage>>();

        public ProtocolContainer()
        {
            m_protoContainer.Add(ProtoID.ReqHeartBeat, new HeartBeatHandler().ReqHeartBeat);
            m_protoContainer.Add(ProtoID.ReqLogin, new LoginHandler().ReqLogin);
        }

        public bool TryGetValue(ProtoID protoID, out Func<ulong, byte[], IMessage> value)
        {
            return m_protoContainer.TryGetValue(protoID, out value);
        }
    }
}
