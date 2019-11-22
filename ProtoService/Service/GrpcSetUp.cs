using Grpc.Core;
using ProtoService.Service.Impl;
using Protocol;
using Game.Service.Chat;
using System;
using InterfaceAdapters.Service.Login;
using InterfaceAdapters.Service.Chat;

namespace ProtoService.Service
{
    public class GrpcSetUp
    {
        private ChatAdapter chatAdapter;
        private LoginMemberAdapter loginMemberAdapter;

        private Server gServer;

        public GrpcSetUp(ChatAdapter chatAdapter, LoginMemberAdapter loginMemberAdapter)
        {
            this.chatAdapter = chatAdapter;
            this.loginMemberAdapter = loginMemberAdapter;
        }

        public void SetUp(string ip, int port)
        {
            gServer = new Server
            {
                Services =
                {
                    EchoLogin.BindService(new LoginImpl(loginMemberAdapter)),
                    EchoChat.BindService(new ChatImpl(chatAdapter)),
                    EchoHeartBeat.BindService(new HeartBeatImpl(loginMemberAdapter)),
                },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
            };
            gServer.Start();

            Console.WriteLine("Grpc server listening on port " + port);
        }
    }
}
