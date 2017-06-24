using log4net;
using SagaBNS.Common.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Manager;
using SagaBNS.LoginServer.Network.AccountServer;
using SagaBNS.LoginServer.Network.CharacterServer;
using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.VirtualFileSystem;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SagaBNS.LoginServer
{
    public static class LoginServer
    {
        #region Methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
            Logger.Log.Error("Error Message:" + ex.Message);
            Logger.Log.Error("Call Stack:" + ex.StackTrace);
            Logger.Log.Error("Trying to save all player's data");

            LoginClientManager.Instance.Stop();
        }

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutingDown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.InitializeLogger(LogManager.GetLogger(typeof(LoginServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Login Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/LoginServer.xml");

            ClientManager<AccountPacketOpcode>.InitialSendCompletionPort = 10;
            ClientManager<AccountPacketOpcode>.NewSendCompletionPortEveryBatch = 2;
            ClientManager<CharacterPacketOpcode>.InitialSendCompletionPort = 10;
            ClientManager<CharacterPacketOpcode>.NewSendCompletionPortEveryBatch = 2;

            Logger.Log.Info(string.Format("Connecting account server at {0}:{1}", Configuration.Instance.AccountHost, Configuration.Instance.AccountPort));
            if (!AccountSession.Instance.Connect(5))
            {
                Logger.Log.Error("Cannot connect to account server");
                Logger.Log.Error("Shutting down in 20sec.");
                System.Threading.Thread.Sleep(20000);
                return;
            }
            while (AccountSession.Instance.State == SESSION_STATE.NOT_IDENTIFIED
                || AccountSession.Instance.State == SESSION_STATE.CONNECTED
                || AccountSession.Instance.State == SESSION_STATE.DISCONNECTED)
            {
                System.Threading.Thread.Sleep(100);
            }
            if (AccountSession.Instance.State == SESSION_STATE.REJECTED
                || AccountSession.Instance.State == SESSION_STATE.FAILED)
            {
                if (AccountSession.Instance.State == SESSION_STATE.REJECTED)
                {
                    Logger.Log.Error("Account server refused login request, please check the password");
                }

                Logger.Log.Info("Shutting down in 20sec.");
                AccountSession.Instance.Network.Disconnect();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }
            Logger.Log.Info("Login to account server successful");

            Logger.Log.Info(string.Format("Connecting character server at {0}:{1}", Configuration.Instance.CharacterHost, Configuration.Instance.CharacterPort));
            if (!CharacterSession.Instance.Connect(5))
            {
                Logger.Log.Error("Cannot connect to character server");
                Logger.Log.Error("Shutting down in 20sec.");
                System.Threading.Thread.Sleep(20000);
                return;
            }
            while (CharacterSession.Instance.State == SESSION_STATE.NOT_IDENTIFIED
                || CharacterSession.Instance.State == SESSION_STATE.CONNECTED
                || CharacterSession.Instance.State == SESSION_STATE.DISCONNECTED)
            {
                System.Threading.Thread.Sleep(100);
            }
            if (CharacterSession.Instance.State == SESSION_STATE.REJECTED
                || CharacterSession.Instance.State == SESSION_STATE.FAILED)
            {
                if (CharacterSession.Instance.State == SESSION_STATE.REJECTED)
                {
                    Logger.Log.Error("Character server refused login request, please check the password");
                }

                Logger.Log.Info("Shutting down in 20sec.");
                AccountSession.Instance.Network.Disconnect();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }
            Logger.Log.Info("Login to character server successful");

            LoginClientManager.Instance.Port = Configuration.Instance.Port;
            Encryption.KeyExchangeImplementation = new Common.Encryption.BNSKeyExchange();
            Encryption.Implementation = new Common.Encryption.BNSXorEncryption();
            Network<LoginPacketOpcode>.Implementation = new Common.BNSLoginNetwork<LoginPacketOpcode>();
            if (!LoginClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                LoginClientManager.Instance.Stop();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }

            Logger.Log.Info("Listening on port:" + LoginClientManager.Instance.Port);
            Logger.Log.Info("Accepting clients...");

            // cmd argument handling
            while (true)
            {
                try
                {
                    string cmd = Console.ReadLine();
                    if (cmd == null)
                    {
                        break;
                    }

                    args = cmd.Split(' ');
                    switch (args[0].ToLower())
                    {
                        case "printthreads":
                            ClientManager.PrintAllThreads();
                            break;

                        case "printband":
                            int sendTotal = 0;
                            int receiveTotal = 0;
                            Logger.Log.Warn("Bandwidth usage information:");
                            try
                            {
                                foreach (Session<LoginPacketOpcode> i in LoginClientManager.Instance.Clients.ToArray())
                                {
                                    sendTotal += i.Network.UpStreamBand;
                                    receiveTotal += i.Network.DownStreamBand;
                                    Logger.Log.Warn($"Client:{i} Receive:{(float)i.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)i.Network.UpStreamBand / 1024:0.##}KB/s");
                                }
                            }
                            catch
                            {
                                Logger.Log.Warn($"Total: Receive:{(float)receiveTotal / 1024:0.##}KB/s Send:{(float)sendTotal / 1024:0.##}KB/s");
                            }
                            break;

                        case "status":
                            {
                                Logger.Log.Warn($"BufferManager:\r\n       TotalAllocatedMemory:{(float)SmartEngine.Network.Memory.BufferManager.Instance.TotalAllocatedMemory / 1024 / 1024:0.00}MB FreeMemory:{(float)SmartEngine.Network.Memory.BufferManager.Instance.FreeMemory / 1024 / 1024:0.##}MB");
                                Logger.Log.Warn("Network Status:");
                                Logger.Log.Warn("LoginServer:");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{LoginClientManager.CurrentCompletionPort} Free IOCPs:{LoginClientManager.FreeCompletionPort}");
                                Logger.Log.Warn("AccountSession:");
                                Logger.Log.Warn($"       Receive:{(float)AccountSession.Instance.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)AccountSession.Instance.Network.UpStreamBand / 1024:0.##}KB/s");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{ClientManager<AccountPacketOpcode>.CurrentCompletionPort} Free IOCPs:{ClientManager<AccountPacketOpcode>.FreeCompletionPort}");
                                Logger.Log.Warn("CharacterSession:");
                                Logger.Log.Warn($"       Receive:{(float)CharacterSession.Instance.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)CharacterSession.Instance.Network.UpStreamBand / 1024:0.##}KB/s");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{ClientManager<CharacterPacketOpcode>.CurrentCompletionPort} Free IOCPs:{ClientManager<CharacterPacketOpcode>.FreeCompletionPort}");
                                Logger.Log.Warn($"Total OnlinePlayer:{LoginClientManager.Instance.Clients.Count}");
                                Logger.Log.Warn($"Total Managed Ram:{(float)GC.GetTotalMemory(false) / 1024 / 1024:0.00} MB, Total Process Ram:{(float)Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024:0.00}MB");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        private static void ShutingDown(object sender, ConsoleCancelEventArgs args)
        {
            Logger.Log.Info("Closing.....");
            LoginClientManager.Instance.Stop();
        }

        #endregion
    }
}