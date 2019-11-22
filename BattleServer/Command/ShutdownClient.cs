using TcpService.Service;

namespace TcpService.Command
{
    public class ShutdownClient : ICommand
    {
        private ITcpServer m_server;

        public ShutdownClient(ITcpServer server)
        {
            m_server = server;
        }

        public void Execute()
        {
            m_server.CloseClientSocket(2);
        }
    }
}
