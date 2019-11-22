using System;
namespace GlobalDefine
{
    public interface IServerDefine
    {
        string IP { get; }
        int Port { get; }

        int GrpcPort { get; }
    }
}
