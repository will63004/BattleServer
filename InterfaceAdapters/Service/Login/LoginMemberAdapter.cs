using Game.Service.Login;
using System;
using System.Collections.Generic;

namespace InterfaceAdapters.Service.Login
{
    public class LoginMemberAdapter
    {
        private ILoginMember loginMember;

        public LoginMemberAdapter(ILoginMember loginMember)
        {
            this.loginMember = loginMember;
        }

        public IReadOnlyDictionary<ulong, Member> Container { get { return loginMember.Container; } }

        public void AddMember(ulong playerID, DateTime activeDateTime)
        {
            loginMember.AddMember(playerID, new Member() 
            {
                PlayerID = playerID,
                ActiveDateTime = activeDateTime,
            });
        }

        public void RemoveMember(ulong playerID)
        {
            loginMember.RemoveMember(playerID);
        }

        public bool UpdateMember(ulong playerID, DateTime activeDateTime)
        {
            return loginMember.UpdateMember(playerID, activeDateTime);
        }
    }
}
