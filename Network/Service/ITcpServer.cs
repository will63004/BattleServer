using System;
using System.Collections.Concurrent;

namespace TcpService.Service
{
    public interface ITcpServer
    {
        ConcurrentDictionary<ulong, AsyncUserToken> Clients { get; }

        event Action<ulong, byte[]> OnReceiveHandle;

        event Action<ulong> OnClientClosed;

        void Start(string ip, int port);

        void CloseClientSocket(ulong ID);
        void Send(ulong playerID, byte[] buff);
        void SendAll(byte[] buff);
    }
}
