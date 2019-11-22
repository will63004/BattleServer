using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using TcpService.Service;

namespace ProtoService.ProtoBuff
{
    public class ProtocolParser: IProtocolParser
    {
        private readonly ITcpServer m_server;
        private readonly IProtocolContainer m_protocolContainer;

        private Dictionary<ProtoID, Action<ulong, IMessage>> registerContainer = new Dictionary<ProtoID, Action<ulong, IMessage>>();

        public ProtocolParser(ITcpServer server, IProtocolContainer container)
        {
            m_server = server;
            m_protocolContainer = container;

            m_server.OnReceiveHandle += ReceiveHandle;
        }

        ~ProtocolParser()
        {
            m_server.OnReceiveHandle -= ReceiveHandle;
        }

        public void RegisterReceiveMessage(ProtoID protoID, Action<ulong, IMessage> onReceiveMessage)
        {
            Action<ulong, IMessage> actions;
            if (registerContainer.TryGetValue(protoID, out actions))
                actions += onReceiveMessage;
            else
                registerContainer.Add(protoID, onReceiveMessage);
        }

        public void UnregisterReceiveMessage(ProtoID protoID, Action<ulong, IMessage> onReceiveMessage)
        {
            Action<ulong, IMessage> actions;
            if (registerContainer.TryGetValue(protoID, out actions))
                actions -= onReceiveMessage;
        }

        private void ReceiveHandle(ulong id, byte[] buffer)
        {
            if (buffer.Length < 2)
                return;

            //ProtocolBuff 數據結構Tag為2byte
            Header header = Header.Parser.ParseFrom(buffer, 2, buffer.Length - 2);

            Console.WriteLine("Receive Player {0}, ProtoID {1}.", id, header.ProtoID.ToString());

            Func<ulong, byte[], IMessage> resp;

            if (m_protocolContainer.TryGetValue(header.ProtoID, out resp))
            {
                IMessage message = resp?.Invoke(id, buffer);

                Action<ulong, IMessage> actions;
                if (registerContainer.TryGetValue(header.ProtoID, out actions))
                    actions?.Invoke(id, message);
            }
        }

        public void Send(ulong id, IMessage message)
        {
            m_server.Send(id, ProtocolBuffConvert.Serialize(message));
        }

        public void SendAll(IMessage message)
        {
            m_server.SendAll(ProtocolBuffConvert.Serialize(message));
        }
    }
}
