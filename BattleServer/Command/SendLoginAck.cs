using ProtoService.ProtoBuff;
using Protocol;

namespace TcpService.Command
{
    public class SendLoginAck: ICommand
    {
        private IProtocolParser m_protocolParser;

        public SendLoginAck(IProtocolParser protocolParser)
        {
            m_protocolParser = protocolParser;
        }

        public void Execute()
        {
            LoginAck ack = new LoginAck();
            ack.Header = new Header();
            ack.Header.ProtoID = ProtoID.AckLogin;

            m_protocolParser.SendAll(ack);
        }
    }
}
