using System;
using System.Collections.Generic;

namespace Game.Service.Login
{
    public interface ILoginMember
    {
        IReadOnlyDictionary<ulong, Member> Container { get; }
        void AddMember(ulong playerID, Member member);
        void RemoveMember(ulong playerID);

        bool UpdateMember(ulong playerID, DateTime activeDateTime);
    }
}
