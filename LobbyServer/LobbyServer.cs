using System;
using System.Diagnostics;
using System.Linq;
using SmartEngine.Network;
using SmartEngine.Core;
using SagaBNS.LobbyServer.Manager;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Network;
using SagaBNS.LobbyServer.Network.AccountServer;
using SagaBNS.LobbyServer.Network.CharacterServer;
using log4net;
using System.Reflection;
using SmartEngine.Network.VirtualFileSystem;

namespace SagaBNS.LobbyServer
{
    internal static class LobbyServer
    {
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutingDown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.InitializeLogger(LogManager.GetLogger(typeof(LobbyServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Lobby Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/LobbyServer.xml");

            Network<LobbyPacketOpcode>.SuppressUnknownPackets = !Debugger.IsAttached;

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

            LobbyClientManager.Instance.Port = Configuration.Instance.Port;
            Encryption.Implementation = new Common.Encryption.BNSAESEncryption();
            Network<LobbyPacketOpcode>.Implementation = new Common.BNSGameNetwork<LobbyPacketOpcode>();
            if (!LobbyClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                LobbyClientManager.Instance.Stop();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }

            Logger.Log.Info("Listening on port:" + LobbyClientManager.Instance.Port);
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
                        case "printband":
                            int sendTotal = 0;
                            int receiveTotal = 0;
                            Logger.Log.Warn("Bandwidth usage information:");
                            try
                            {
                                foreach (Session<LobbyPacketOpcode> i in LobbyClientManager.Instance.Clients.ToArray())
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
            LobbyClientManager.Instance.Stop();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
            Logger.Log.Error("Error Message:" + ex.Message);
            Logger.Log.Error("Call Stack:" + ex.StackTrace);
            Logger.Log.Error("Trying to save all data");

            LobbyClientManager.Instance.Stop();
        }
    }
}
