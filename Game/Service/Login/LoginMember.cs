using System;
using System.Collections.Generic;

namespace Game.Service.Login
{
    public class LoginMember: ILoginMember
    {
        private Dictionary<ulong, Member> container = new Dictionary<ulong, Member>();

        public IReadOnlyDictionary<ulong, Member> Container
        {
            get
            {
                return container;
            }
        }

        public Dictionary<ulong, Member>.Enumerator GetContainer()
        {            
            return container.GetEnumerator();
        }

        public void AddMember(ulong playerID, Member member)
        {
            if (container.ContainsKey(playerID))
            {
                Console.WriteLine("Add Same Member");
                return;
            }   

            container.Add(playerID, member);            
        }

        public void RemoveMember(ulong playerID)
        {
            container.Remove(playerID);
        }

        public bool UpdateMember(ulong playerID, DateTime activeDateTime)
        {
            Member member;
            if(container.TryGetValue(playerID, out member))
            {
                member.ActiveDateTime = activeDateTime;
                return true;
            }

            return false;
        }
    }
}
