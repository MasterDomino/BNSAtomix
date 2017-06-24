using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SmartEngine.Core;
namespace SmartEngine.Network
{
    /// <summary>
    /// 用于连接的默认客户端
    /// </summary>
    /// <typeparam name="T">封包Opcode枚举</typeparam>
    public class DefaultClient<T> : Session<T>
    {
        private readonly Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int port;
        private bool encrypt = true, autoLock;
        private readonly Dictionary<T, Packet<T>> commandTable = new Dictionary<T, Packet<T>>();

        /// <summary>
        /// 服务器的Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 服务器的端口
        /// </summary>
        public int Port { get { return port; } set { this.port = value; } }

        /// <summary>
        /// 封包是否会被加密，默认为True
        /// </summary>
        public bool Encrypt { get { return encrypt; } set { this.encrypt = value; } }

        /// <summary>
        /// 处理封包时，是否自动上同步锁，默认为false
        /// </summary>
        public bool AutoLock { get { return autoLock; } set { this.autoLock = value; } }

        /// <summary>
        /// 尝试连接服务器
        /// </summary>
        /// <param name="times">重试次数</param>
        /// <returns>是否成功</returns>
        public bool Connect(int times)
        {
            bool Connected = false;
            do
            {
                if (times < 0)
                {
                    return false;
                }
                try
                {
                    sock.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(Host), port));
                    Connected = true;
                }
                catch (Exception e)
                {
                    Logger.Log.Error("Failed... Trying again in 5sec");
                    Logger.Log.Error(e.ToString());
                    System.Threading.Thread.Sleep(5000);
                    Connected = false;
                }
                times--;
            } while (!Connected);

            try
            {
                this.netIO = Network<T>.Implementation.CreateNewInstance(sock, this.commandTable, this);
                this.netIO.Encrypt = encrypt;
                this.netIO.AutoLock = autoLock;
                this.netIO.SetMode(Mode.Client);
                SendInitialPacket();
            }
            catch (Exception ex)
            {
                Logger.Log.Warn(ex.StackTrace);
            }
            return true;
        }

        public override void OnDisconnect()
        {
            base.OnDisconnect();
            ClientManager<T>.StopSendReceiveThreads();
        }

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

        /// <summary>
        /// 发送初始化封包（第一个封包），如没有请重载后直接返回
        /// </summary>
        protected virtual void SendInitialPacket()
        {
            Packet<T> p = new Packet<T>(8);
            p.data[7] = 0x10;
            this.netIO.SendPacket(p, true);
        }
    }
}
