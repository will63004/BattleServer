using Microsoft.Extensions.Hosting;
using ProtoService.ProtoBuff;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpService.Service;

namespace TcpService.Command
{
    public class CommandSystem: IHostedService, ICommand
    {
        private IProtocolParser m_protocolParser;

        private Dictionary<string, ICommand> m_commandContainer;

        public CommandSystem(IProtocolParser protocolParser, ITcpServer server)
        {
            m_protocolParser = protocolParser;

            m_commandContainer = new Dictionary<string, ICommand>();

            //m_commandContainer.Add("Stop Service", new StopService());
            m_commandContainer.Add("Show Command Code", this);
            m_commandContainer.Add("Send HeartBeat", new SendHeartBeat(protocolParser));
            m_commandContainer.Add("ShutdownClient", new ShutdownClient(server));
            m_commandContainer.Add("Send LoginAck", new SendLoginAck(protocolParser));
        }

        public void Command(string commandCode)
        {
            ICommand command;
            if (m_commandContainer.TryGetValue(commandCode, out command))
                command.Execute();
            else
                Console.WriteLine("Error Command Code! You can get all command by Show Command Code");
        }

        public void Execute()
        {
            ShowCommandCode();
        }

        private void ShowCommandCode()
        {
            Console.WriteLine("=========================================");
            var e = m_commandContainer.GetEnumerator();
            while (e.MoveNext())
            {
                string commandCode = e.Current.Key;
                Console.WriteLine(commandCode);
            }
            Console.WriteLine("=========================================");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Start()
        {
            Thread thread = new Thread(new ThreadStart(inputCommand));
            thread.IsBackground = true;
            thread.Start();
        }

        private void inputCommand()
        {
            do
            {
                Command(Console.ReadLine());
            } while (true);
        }
    }
}
