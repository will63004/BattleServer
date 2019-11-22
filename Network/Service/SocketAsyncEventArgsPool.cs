using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TcpService.Service
{
    /// <summary>
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    /// 表示可重用的SocketAsyncEventArgs对象的集合。
    /// </summary>
    public class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {                
                m_pool.Push(item);
                Console.WriteLine("EventArgsPool Push, RemainCount {0}.", m_pool.Count);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                SocketAsyncEventArgs eventArgs = m_pool.Pop();
                Console.WriteLine("EventArgsPool Pop, RemainCount {0}.", m_pool.Count);
                return eventArgs;
            }
        }

        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
