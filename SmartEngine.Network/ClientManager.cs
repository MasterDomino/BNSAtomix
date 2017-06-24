using SmartEngine.Core;
using SmartEngine.Network.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace SmartEngine.Network
{
    /// <summary>
    /// 客户端管理器主类，不应继承此类而是ClientManager(T)，包含了关键区同步实现"/&gt;
    /// </summary>
    public class ClientManager
    {
        #region Members

        internal static Dictionary<string, Thread> Threads = new Dictionary<string, Thread>();
        private static readonly HashSet<Thread> blockedThread = new HashSet<Thread>();
        private static readonly bool noCheckDeadLock;
        private static readonly AutoResetEvent waitressQueue = new AutoResetEvent(true);
        private static Thread currentBlocker;
        private static bool enteredcriarea;
        private static DateTime timestamp;

        #endregion

        #region Properties

        /// <summary>
        /// 主锁是否已闭塞
        /// </summary>
        public static bool Blocked
        {
            get
            {
                return blockedThread.Contains(Thread.CurrentThread);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 添加线程
        /// </summary>
        /// <param name="thread">线程</param>
        public static void AddThread(Thread thread)
        {
            AddThread(thread.Name, thread);
        }

        /// <summary>
        /// 添加线程
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="thread">线程</param>
        public static void AddThread(string name, Thread thread)
        {
            if (!Threads.ContainsKey(name))
            {
                lock (Threads)
                {
                    try
                    {
                        Threads.Add(name, thread);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(ex);
                        Logger.Log.Debug("Threads count:" + Threads.Count);
                    }
                }
            }
        }

        /// <summary>
        /// 进入关键区
        /// </summary>
        public static void EnterCriticalArea()
        {
            if (blockedThread.Contains(Thread.CurrentThread))
            {
                Logger.Log.Debug("Current thread is already blocked, skip blocking to avoid deadlock!");
            }
            else
            {
                //Global.clientMananger.AddWaitingWaitress();
                waitressQueue.WaitOne();
                timestamp = DateTime.Now;
                enteredcriarea = true;
                blockedThread.Add(Thread.CurrentThread);
                currentBlocker = Thread.CurrentThread;
            }
        }

        /// <summary>
        /// 取得线程
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>线程</returns>
        public static Thread GetThread(string name)
        {
            if (Threads.ContainsKey(name))
            {
                lock (Threads)
                {
                    return Threads[name];
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 离开关键区
        /// </summary>
        public static void LeaveCriticalArea()
        {
            LeaveCriticalArea(Thread.CurrentThread);
        }

        /// <summary>
        /// 强制某个线程离开关键区
        /// </summary>
        /// <param name="blocker">上锁的线程</param>
        public static void LeaveCriticalArea(Thread blocker)
        {
            if (blockedThread.Contains(blocker) || blockedThread.Count != 0)
            {
                int sec = (DateTime.Now - timestamp).Seconds;
                if (sec > 5)
                {
                    Logger.Log.Debug(string.Format("Thread({0}) used unnormal time till unlock({1} sec)", blocker.Name, sec));
                }
                enteredcriarea = false;
                if (blockedThread.Contains(blocker))
                {
                    blockedThread.Remove(blocker);
                }
                /*else
{
   if (blockedThread.Count > 0)
       blockedThread.RemoveAt(0);
}*/
                currentBlocker = null;
                timestamp = DateTime.Now;
                waitressQueue.Set();
            }
            else
            {
                Logger.Log.Debug("Current thread isn't blocked while trying unblock, skiping");
            }
        }

        /// <summary>
        /// 在控制台打印出当前线程运行情况，以及其调用堆栈
        /// </summary>
        public static void PrintAllThreads()
        {
            Logger.Log.Warn("Call Stack of all blocking Threads:");
            foreach (Thread j in blockedThread.ToArray())
            {
                try
                {
                    Logger.Log.Warn("Thread name:" + GetThreadName(j));
                    j.Suspend();
                    StackTrace running = new StackTrace(j, true);
                    j.Resume();
                    foreach (StackFrame i in running.GetFrames())
                    {
                        Logger.Log.Warn("at " + i.GetMethod().ReflectedType.FullName + "." + i.GetMethod().Name + " " + i.GetFileName() + ":" + i.GetFileLineNumber());
                    }
                }
                catch { }
                Console.WriteLine();
            }
            Logger.Log.Warn("Call Stack of all Threads:");
            string[] keys = new string[Threads.Keys.Count];
            Threads.Keys.CopyTo(keys, 0);
            foreach (string k in keys)
            {
                try
                {
                    Thread j = GetThread(k);
                    j.Suspend();
                    StackTrace running = new StackTrace(j, true);
                    j.Resume();
                    Logger.Log.Warn("Thread name:" + k);
                    foreach (StackFrame i in running.GetFrames())
                    {
                        Logger.Log.Warn("at " + i.GetMethod().ReflectedType.FullName + "." + i.GetMethod().Name + " " + i.GetFileName() + ":" + i.GetFileLineNumber());
                    }
                }
                catch
                {
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 删除线程
        /// </summary>
        /// <param name="name">名称</param>
        public static void RemoveThread(string name)
        {
            if (Threads.ContainsKey(name))
            {
                lock (Threads)
                {
                    Threads.Remove(name);
                }
            }
        }

        /// <summary>
        /// 死锁探测器
        /// </summary>
        internal void CheckCriticalArea()
        {
            while (true)
            {
                if (enteredcriarea)
                {
                    TimeSpan span = DateTime.Now - timestamp;
                    if (span.TotalSeconds > 10 && !noCheckDeadLock && !Debugger.IsAttached)
                    {
                        Logger.Log.Error("Deadlock detected");
                        Logger.Log.Error("Automatically unlocking....");
                        StackTrace running;
                        try
                        {
                            if (currentBlocker != null)
                            {
                                Logger.Log.Error("Call Stack of current blocking Thread:");
                                Logger.Log.Error("Thread name:" + GetThreadName(currentBlocker));
                                if (currentBlocker.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    Logger.Log.Warn("Unexpected thread state:" + currentBlocker.ThreadState.ToString());
                                }

                                currentBlocker.Suspend();
                                running = new StackTrace(currentBlocker, true);
                                currentBlocker.Resume();
                                foreach (StackFrame i in running.GetFrames())
                                {
                                    Logger.Log.Error("at " + i.GetMethod().ReflectedType.FullName + "." + i.GetMethod().Name + " " + i.GetFileName() + ":" + i.GetFileLineNumber());
                                }
                            }
                        }
                        catch (Exception ex) { Logger.Log.Error(ex); }
                        Console.WriteLine();
                        Logger.Log.Error("Call Stack of all blocking Threads:");
                        foreach (Thread j in blockedThread.ToArray())
                        {
                            try
                            {
                                Logger.Log.Error("Thread name:" + GetThreadName(j));
                                if (j.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    Logger.Log.Warn("Unexpected thread state:" + j.ThreadState.ToString());
                                }

                                j.Suspend();
                                running = new StackTrace(j, true);
                                j.Resume();
                                foreach (StackFrame i in running.GetFrames())
                                {
                                    Logger.Log.Error("at " + i.GetMethod().ReflectedType.FullName + "." + i.GetMethod().Name + " " + i.GetFileName() + ":" + i.GetFileLineNumber());
                                }
                            }
                            catch (Exception ex) { Logger.Log.Error(ex); }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        Logger.Log.Error("Call Stack of all Threads:");
                        string[] keys = new string[Threads.Keys.Count];
                        Threads.Keys.CopyTo(keys, 0);
                        foreach (string k in keys)
                        {
                            try
                            {
                                Thread j = GetThread(k);
                                Logger.Log.Error("Thread name:" + k);
                                if (j.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    Logger.Log.Warn("Unexpected thread state:" + j.ThreadState.ToString());
                                }

                                j.Suspend();
                                running = new StackTrace(j, true);
                                j.Resume();
                                foreach (StackFrame i in running.GetFrames())
                                {
                                    Logger.Log.Error("at " + i.GetMethod().ReflectedType.FullName + "." + i.GetMethod().Name + " " + i.GetFileName() + ":" + i.GetFileLineNumber());
                                }
                            }
                            catch
                            {
                            }
                            Console.WriteLine();
                        }
                        LeaveCriticalArea(currentBlocker);
                    }
                }
                Thread.Sleep(10000);
            }
        }

        private static string GetThreadName(Thread thread)
        {
            foreach (string i in Threads.Keys)
            {
                if (thread == Threads[i])
                {
                    return i;
                }
            }
            return string.Empty;
        }

        #endregion
    }

    /// <summary>
    /// 客户端管理器
    /// </summary>
    /// <typeparam name="T">封包Opcode枚举</typeparam>
    public abstract class ClientManager<T> : ClientManager
    {
        #region Members

        private static readonly ConcurrentQueue<SocketAsyncEventArgs> avaliableSendCompletion = new ConcurrentQueue<SocketAsyncEventArgs>();
        private static readonly List<Network<T>> pendingNetIO = new List<Network<T>>();
        private static readonly ConcurrentQueue<KeyValuePair<Network<T>, BufferBlock>> sendRequests = new ConcurrentQueue<KeyValuePair<Network<T>, BufferBlock>>();
        private static readonly AutoResetEvent sendRequestWaiter = new AutoResetEvent(false);
        private static readonly AutoResetEvent sendWaiter = new AutoResetEvent(false);
        private static int currentSendCompletionPort;
        private static Thread sender;
        private static bool shouldEnd;
        private readonly Dictionary<T, Packet<T>> commandTable = new Dictionary<T, Packet<T>>();
        private bool isUp;
        private TcpListener listener;
        private Thread mainLoop;

        #endregion

        #region Properties

        /// <summary>
        /// gets current completion port
        /// </summary>
        /// <returns></returns>
        public static int CurrentCompletionPort
        {
            get
            {
                return currentSendCompletionPort;
            }
        }

        /// <summary>
        /// gets next free completion port
        /// </summary>
        /// <returns></returns>
        public static int FreeCompletionPort
        {
            get
            {
                return avaliableSendCompletion.Count;
            }
        }

        /// <summary>
        /// initial completion port
        /// </summary>
        public static int InitialSendCompletionPort { get; set; } = 500;

        /// <summary>
        /// new completion port
        /// </summary>
        public static int NewSendCompletionPortEveryBatch { get; set; } = 200;

        /// <summary>
        /// 是否在处理封包时自动上锁，建议在不需要同步的服务期间内部通讯设置为false。由于全局锁对于死锁方面的防护比较难控制，故不建议使用自动全局锁
        /// </summary>
        [Obsolete("由于全局锁对于死锁方面的防护比较难控制，故不建议使用自动全局锁", false)]
        public bool AutoLock { get; set; }

        /// <summary>
        /// get thread
        /// </summary>
        public Thread Check { get; }

        /// <summary>
        /// 目前连线的客户端
        /// </summary>
        public HashSet<Session<T>> Clients { get; } = new HashSet<Session<T>>();

        /// <summary>
        /// Encryption
        /// </summary>
        public bool Encrypt { get; set; } = true;

        /// <summary>
        /// 一次能够接收的最大连接数
        /// </summary>
        public int MaxNewConnections { get; set; } = 10;

        /// <summary>
        /// 服务器监听的端口
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// remove tcp client
        /// </summary>
        /// <param name="client"></param>
        public virtual void RemoveClient(Session<T> client)
        {
            lock (Clients)
            {
                if (Clients.Contains(client))
                {
                    Clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// starts the listener
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            mainLoop = new Thread(new ThreadStart(NetworkLoop));
            mainLoop.Start();

            listener = new TcpListener(Port);
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return false;
            }
            isUp = true;
            return true;
        }

        /// <summary>
        /// stops the listener
        /// </summary>
        public virtual void Stop()
        {
            isUp = false;
            listener?.Stop();
            mainLoop.Abort();
            mainLoop = null;
            StopSendReceiveThreads();
            sender = null;
        }

        internal static void EnqueueSendRequest(Network<T> network, BufferBlock buffer)
        {
            if (sender == null)
            {
                shouldEnd = false;
                sender = new Thread(SendLoop);
                sender.Start();

                for (int i = 0; i < InitialSendCompletionPort - avaliableSendCompletion.Count; i++)
                {
                    SocketAsyncEventArgs res = new SocketAsyncEventArgs();
                    res.Completed += Network<T>.Send_Completed;
                    avaliableSendCompletion.Enqueue(res);
                }
                currentSendCompletionPort = avaliableSendCompletion.Count;
            }
            buffer.UserToken = network;
            KeyValuePair<Network<T>, BufferBlock> req = new KeyValuePair<Network<T>, BufferBlock>(network, buffer);
            sendRequests.Enqueue(req);
            sendRequestWaiter.Set();
        }

        internal static void FinishSendQuest(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            avaliableSendCompletion.Enqueue(e);
            sendWaiter.Set();
        }

        internal static void StopSendReceiveThreads()
        {
            shouldEnd = true;
            sendRequestWaiter.Set();
            sender = null;
        }

        /// <summary>
        /// 建立一个新的Session实例
        /// </summary>
        /// <returns>新Session</returns>
        protected abstract Session<T> NewSession();

        /// <summary>
        /// 注册封包处理类
        /// </summary>
        /// <param name="opcode">Opcode</param>
        /// <param name="packetHandler">对应的处理类</param>
        protected void RegisterPacketHandler(T opcode, Packet<T> packetHandler)
        {
            if (!commandTable.ContainsKey(opcode))
            {
                commandTable.Add(opcode, packetHandler);
            }
            else
            {
                Logger.Log.Warn(string.Format("{0} already registered", opcode));
            }
        }

        private static void SendLoop()
        {
            while (!shouldEnd)
            {
                while (sendRequests.TryDequeue(out KeyValuePair<Network<T>, BufferBlock> req))
                {
                    SocketAsyncEventArgs arg;
                    while (!avaliableSendCompletion.TryDequeue(out arg))
                    {
                        if (!sendWaiter.WaitOne(10))
                        {
                            for (int i = 0; i < NewSendCompletionPortEveryBatch; i++)
                            {
                                SocketAsyncEventArgs res = new SocketAsyncEventArgs();
                                res.Completed += Network<T>.Send_Completed;
                                avaliableSendCompletion.Enqueue(res);
                            }
                            Interlocked.Add(ref currentSendCompletionPort, NewSendCompletionPortEveryBatch);
                        }
                    }
                    Network<T> net = req.Key;
                    arg.UserToken = req.Value;
                    net.BlockHandled(req.Value);
                    arg.SetBuffer(req.Value.Buffer, req.Value.StartIndex, req.Value.UsedLength);
                    try
                    {
                        if (!net.Socket.SendAsync(arg))
                        {
                            Network<T>.Send_Completed(null, arg);
                        }
                    }
                    catch
                    {
                        FinishSendQuest(arg);
                    }
                }
                sendRequestWaiter.WaitOne();
            }
        }

        private void CreateNewSession(TcpListener listener)
        {
            Socket sock = listener.AcceptSocket();
            sock.NoDelay = true;
            string ip = sock.RemoteEndPoint.ToString().Substring(0, sock.RemoteEndPoint.ToString().IndexOf(':'));
            Logger.Log.Info(string.Format("New Client:{0}", sock.RemoteEndPoint.ToString()));
            Session<T> client = NewSession();
            client.netIO = Network<T>.Implementation.CreateNewInstance(sock, this.commandTable, client);
            client.clientManager = this;
            client.netIO.Encrypt = Encrypt;
            client.netIO.AutoLock = AutoLock;
            client.netIO.SetMode(Mode.Server);
            client.netIO.OnConnect();
            if (!client.netIO.Disconnected)
            {
                lock (Clients)
                {
                    Clients.Add(client);
                }
            }
        }

        private void NetworkLoop()
        {
            while (true)
            {
                try
                {
                    // let new clients (max 10) connect
                    if (isUp)
                    {
                        for (int i = 0; listener.Pending() && i < MaxNewConnections; i++)
                        {
                            CreateNewSession(listener);
                        }
                    }
                    Thread.Sleep(1);
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        #endregion
    }
}