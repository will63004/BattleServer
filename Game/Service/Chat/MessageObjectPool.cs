using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

namespace Game.Service.Chat
{
    public class MessageObjectPool : ObjectPool<Message>
    {
        private Stack<Message> stack = new Stack<Message>();

        public override Message Get()
        {
            if (stack.Count > 0)
                return stack.Pop();

            return new Message();
        }

        public override void Return(Message obj)
        {
            stack.Push(obj);
        }
    }
}
