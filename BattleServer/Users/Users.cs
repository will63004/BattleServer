using Game.Service.HeartBeat;
using Game.Service.Login;
using InterfaceAdapters.Service.Login;
using System;
using System.Collections.Generic;

namespace ServerNormal.Users
{
    public class Users : IUsers
    {
        private LoginMemberAdapter loginMemberAdapter;

        public Users(LoginMemberAdapter loginMemberAdapter)
        {
            this.loginMemberAdapter = loginMemberAdapter;
        }

        public bool GetUsersActiveDateTime(ref Dictionary<ulong, DateTime> times)
        {
            bool isAdd = false;
            times.Clear();
            foreach(ulong key in loginMemberAdapter.Container.Keys)
            {
                Member member;
                if(loginMemberAdapter.Container.TryGetValue(key, out member))
                {
                    times.Add(key, member.ActiveDateTime);
                    isAdd = true;
                }
            }

            return isAdd;
        }

        public void ShutdownUser(ulong id)
        {
            //TODO
        }
    }
}
