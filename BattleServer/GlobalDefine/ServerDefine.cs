namespace GlobalDefine
{
    public class ServerDefine: IServerDefine
    {
        public string IP { get; } = "127.0.0.1";
        public int Port { get; } = 3000;

        public int GrpcPort { get; } = 3001;
    }
}
