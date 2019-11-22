using Grpc.Core;
using InterfaceAdapters.Service.Login;
using Protocol;
using System;
using System.Threading.Tasks;

namespace ProtoService.Service.Impl
{
    public class HeartBeatImpl:EchoHeartBeat.EchoHeartBeatBase
    {
        private LoginMemberAdapter loginMemberAdapter;

        public HeartBeatImpl(LoginMemberAdapter loginMemberAdapter)
        {
            this.loginMemberAdapter = loginMemberAdapter;
        }

        public override Task<HeartBeatAck> HeartBeat (HeartBeatReq request, ServerCallContext context)
        {
            loginMemberAdapter.UpdateMember(request.Header.PlayerID, DateTime.Now);
            return Task.FromResult(new HeartBeatAck { });
        }
    }
}
