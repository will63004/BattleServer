using System;
using System.Net.Sockets;

namespace TcpService.Service
{
    public class AsyncUserToken
    {
        public Socket AcceptSocket { get; set; }
        public SocketAsyncEventArgs AcceptAsyncEventArgs { get; set; }
        public SocketAsyncEventArgs ReadAsyncEventArgs { get; set; }
        public ulong ID { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public DateTime ActiveDateTime { get; set; }
    }
}
