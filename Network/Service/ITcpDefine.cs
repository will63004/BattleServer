using System;
using System.Collections.Generic;
namespace TcpService.Service
{
    public interface ITcpDefine
    {
        int MaxConnectNum { get; }
        int ReceiveBuffSize { get; }        
    }
}
