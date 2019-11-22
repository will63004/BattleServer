using System;
using System.Collections.Generic;

namespace Game.Service.HeartBeat
{
    public interface IUsers
    {
        bool GetUsersActiveDateTime(ref Dictionary<ulong, DateTime> times);

        void ShutdownUser(ulong id);
    }
}
