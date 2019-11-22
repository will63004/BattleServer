using System;
using TcpService.Service;

namespace GlobalDefine
{
    public class TcpDefine:ITcpDefine
    {        
        public int MaxConnectNum { get; } = 2;
        public int ReceiveBuffSize { get; } = 1024;
    }
}
