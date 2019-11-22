using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpService.Service
{
    /// <summary>
    /// Implements the connection logic for the socket server.  
    /// After accepting a connection, all data read from the client 
    /// is sent back to the client. The read and echo back to the client pattern 
    /// is continued until the client disconnects.
    ///
    /// 实现套接字服务器的连接逻辑。
    /// 接受连接后，从客户端读取的所有数据都将发送回客户端。
    /// 读取和回传到客户端模式将继续，直到客户端断开连接。
    /// </summary>
    public class TcpServer: ITcpServer
    {
        /// <summary>
        /// the maximum number of connections the sample is designed to handle simultaneously 例子中设计的同时处理的最大连接数
        /// </summary>
        private int m_numConnections;

        /// <summary>
        /// buffer size to use for each socket I/O operation 
        /// 用于每个Socket I/O 操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;

        /// <summary>
        /// represents a large reusable set of buffers for all socket operations    
        /// 表示用于所有套接字操作的大量可重用的缓冲区
        /// </summary>
        private BufferManager m_bufferManager;

        /// <summary>
        /// read, write (don't alloc buffer space for accepts)  
        /// 读，写（不为接受连接分配缓冲区空间）
        /// </summary>
        const int opsToPreAlloc = 2;

        /// <summary>
        /// the socket used to listen for incoming connection requests
        /// </summary>
        Socket listenSocket;

        /// <summary>
        /// pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        /// 可重用SocketAsyncEventArgs对象的池，用于写入，读取和接受套接字操作
        /// </summary>
        SocketAsyncEventArgsPool m_readWritePool;

        /// <summary>
        /// 信号量
        /// </summary>
        Semaphore m_maxNumberAcceptedClients;

        private ConcurrentDictionary<ulong, AsyncUserToken> m_clients = new ConcurrentDictionary<ulong, AsyncUserToken>();
        public ConcurrentDictionary<ulong, AsyncUserToken> Clients
        {
            get
            {
                return m_clients;
            }
        }

        public event Action<ulong, byte[]> OnReceiveHandle;
        public event Action<ulong> OnClientClosed;

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests call the Init method followed by Start method 
        /// 创建一个未初始化的服务器实例。
        /// 要开始监听连接请求的服务器，请调用Init方法，然后调用Start方法
        /// </summary>
        /// <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
        /// <param name="receivesBufferSize">buffer size to use for each socket I/O operation</param>
        public TcpServer(ITcpDefine serviceDefine)
        {
            m_numConnections = serviceDefine.MaxConnectNum;
            m_receiveBufferSize = serviceDefine.ReceiveBuffSize;

            // allocate buffers such that the maximum number of sockets can have one outstanding read and write posted to the socket simultaneously
            // 分配缓冲区，使得最大数量的套接字可以同时具有一个未完成的读写
            m_bufferManager = new BufferManager(serviceDefine.ReceiveBuffSize * serviceDefine.MaxConnectNum * opsToPreAlloc, serviceDefine.ReceiveBuffSize);

            m_readWritePool = new SocketAsyncEventArgsPool(serviceDefine.MaxConnectNum);
            m_maxNumberAcceptedClients = new Semaphore(serviceDefine.MaxConnectNum, serviceDefine.MaxConnectNum);

            Init();
        }

        /// <summary>
        /// Initializes the server by preallocating reusable buffers and context objects.  
        /// These objects do not need to be preallocated or reused, 
        /// but it is done this way to illustrate how the API can easily be used to create reusable objects to increase server performance.
        /// 通过预先分配可重用的缓冲区和上下文对象来初始化服务器。
        /// 这些对象不需要预先分配或重用，但是这样做是为了说明如何轻松地使用API来创建可重用的对象来提高服务器性能。
        /// </summary>
        private void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds against memory fragmentation
            // 分配一个大字节缓冲区, 所有的 I/O 操作都使用一个。 这样对抗内存分裂
            m_bufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects
            // 预先分配SocketAsyncEventArgs对象的池
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_numConnections; i++)
            {
                // Pre-allocate a set of reusable SocketAsyncEventArgs
                // 预先分配一组可重用的SocketAsyncEventArgs
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                AsyncUserToken userToken =  new AsyncUserToken();
                userToken.ID = (ulong)i + 1;
                readWriteEventArg.UserToken = userToken;

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                // 给SocketAsyncEventArg对象从缓冲池分配一个字节缓冲区
                m_bufferManager.SetBuffer(readWriteEventArg);

                // add SocketAsyncEventArg to the pool
                m_readWritePool.Push(readWriteEventArg);
            }
        }

        /// <summary>
        /// Starts the server such that it is listening for incoming connection requests. 
        /// </summary>
        /// <param name="localEndPoint">The endpoint which the server will listening for connection requests on</param>
        public void Start(string ip, int port)
        {
            IPAddress ipAdress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAdress, port);

            // create the socket which listens for incoming connections
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // start the server with a listen backlog of 100 connections
            listenSocket.Listen(m_numConnections);

            // post accepts on the listening socket 在接收端口上接收
            StartAccept(null);

            Console.WriteLine("Server Started!");
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client 
        /// </summary>
        /// <param name="p">The context object to use when issuing the accept operation on the server's listening socket</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArgs_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        ///  This method is the callback method associated with Socket.AcceptAsync operations and is invoked when an accept operation is complete
        /// </summary>
        private void AcceptEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                StartAccept(e);
                return;
            }

            if(m_readWritePool.Count == 0)
            {
                e.AcceptSocket = null;
                m_maxNumberAcceptedClients.Release();
                StartAccept(e);
                return;
            }
                        
            SocketAsyncEventArgs eventArgs = m_readWritePool.Pop();
            AsyncUserToken accpetUserToken = (AsyncUserToken)eventArgs.UserToken;
            if (!m_clients.TryAdd(accpetUserToken.ID, accpetUserToken))
            {
                m_readWritePool.Push(eventArgs);
                throw new Exception("Add same ID into m_clients");
            }

            // Get the socket for the accepted client connection and put it into the 
            // ReadEventArg object user token
            accpetUserToken.AcceptSocket = e.AcceptSocket;
            accpetUserToken.AcceptAsyncEventArgs = e;
            accpetUserToken.ReadAsyncEventArgs = eventArgs;
            accpetUserToken.ConnectDateTime = DateTime.Now;

            string ipClient = accpetUserToken.AcceptSocket.RemoteEndPoint.ToString();
            Console.WriteLine("Server : Client [ {0} ] connected. ID is [ {1} ] There are {2} clients connected to server", ipClient, accpetUserToken.ID, m_clients.Count);
                        
            // As soon as the client is connected, post a receive to the connection
            bool willRaiseEvent = accpetUserToken.AcceptSocket.ReceiveAsync(eventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(eventArgs);
            }

            // Accept the next connection request
            StartAccept(e);
        }

        /// <summary>
        /// This method is called whenever a receive or send operation is completed on a socket 
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            lock (e)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;            
            // e.BytesTransferred获取套接字操作中传输的字节数。 
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                token.ActiveDateTime = DateTime.Now;

                int lengthBuffer = e.BytesTransferred;
                byte[] receiveBuffer = e.Buffer;
                byte[] buffer = new byte[lengthBuffer];
                Buffer.BlockCopy(receiveBuffer, e.Offset, buffer, 0, lengthBuffer);
                OnReceiveHandle?.Invoke(token.ID, buffer);

                bool willRaiseEvent = token.AcceptSocket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional data sent from the client
        /// </summary>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client     完成回传数据回到客户端
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // read the next block of data send from the client     读取从客户端发送的下一个数据块
                bool willRaiseEvent = token.AcceptSocket.ReceiveAsync(e);                
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            // close the socket associated with the client
            try
            {
                token.AcceptSocket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) {}

            token.AcceptSocket.Close();
            if (m_clients.TryRemove(token.ID, out _))
            {
                // Free the SocketAsyncEventArg so they can be reused by another client            
                m_maxNumberAcceptedClients.Release();

                Console.WriteLine("ID [ {0} ] client has been disconnected from the Server. There are {1} clients connected to the server", token.ID, m_clients.Count);

                // Free the SocketAsyncEventArg so they can be reused by another client
                m_readWritePool.Push(e);

                OnClientClosed?.Invoke(token.ID);
            }
        }

        public void CloseClientSocket(ulong ID)
        {
            lock (m_clients)
            {
                AsyncUserToken userToken;
                if (m_clients.TryGetValue(ID, out userToken))
                    CloseClientSocket(userToken.AcceptAsyncEventArgs);
            }
        }

        public void Send(ulong playerID, byte[] buff)
        {
            AsyncUserToken userToken;
            if(m_clients.TryGetValue(playerID, out userToken))
            {
                SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.SetBuffer(buff, 0, buff.Length);
                bool willRaiseEvent = userToken.AcceptSocket.SendAsync(asyncEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessSend(asyncEventArgs);
                }
            }
        }

        public void SendAll(byte[] buff)
        {
            foreach(var client in m_clients)
            {
                SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.SetBuffer(buff, 0, buff.Length);
                bool willRaiseEvent = client.Value.AcceptSocket.SendAsync(asyncEventArgs);
                if (!willRaiseEvent)
                    ProcessSend(asyncEventArgs);
            }
        }
    }
}
