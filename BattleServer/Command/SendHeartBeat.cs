using ProtoService.ProtoBuff;
using Protocol;

namespace TcpService.Command
{
    public class SendHeartBeat: ICommand
    {
        private IProtocolParser m_protocolParser;

        public SendHeartBeat(IProtocolParser protocolParser)
        {
            m_protocolParser = protocolParser;
        }

        public void Execute()
        {
            for(int i = 0; i < 2; ++i)
            {
                HeartBeatAck ack = new HeartBeatAck();
                ack.Header = new Header();
                ack.Header.ProtoID = ProtoID.AckHeartBeat;

                m_protocolParser.Send(1, ack);
            }
        }
    }
}
