using Grpc.Core;
using InterfaceAdapters.Service.Login;
using Protocol;
using System;
using System.Threading.Tasks;

namespace ProtoService.Service.Impl
{
    public class LoginImpl: EchoLogin.EchoLoginBase
    {
        private LoginMemberAdapter loginMemberAdapter;

        public LoginImpl(LoginMemberAdapter loginMemberAdapter)
        {
            this.loginMemberAdapter = loginMemberAdapter;
        }

        public override Task<LoginAck> Login(LoginReq request, ServerCallContext context)
        {
            loginMemberAdapter.AddMember(request.Header.PlayerID, DateTime.Now);
            return Task.FromResult(new LoginAck 
            { 
                Header = new Header() { ProtoID = ProtoID.AckLogin, PlayerID = request.Header.PlayerID } 
            });
        }
    }
}
