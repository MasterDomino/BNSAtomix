using System;
using System.Linq;

using SmartEngine.Network;
using SmartEngine.Core;

using SagaBNS.ChatServer.Packets;
using SagaBNS.ChatServer.Manager;
using log4net;
using System.Reflection;
using System.Diagnostics;
using SmartEngine.Network.VirtualFileSystem;

namespace SagaBNS.ChatServer
{
    internal static class ChatServer
    {
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutingDown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.InitializeLogger(LogManager.GetLogger(typeof(ChatServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Chat Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/ChatServer.xml");

            ChatClientManager.Instance.Port = Configuration.Instance.Port;
            Network<BNSChatOpcodes>.Implementation = new Common.BNSChatNetwork<BNSChatOpcodes>();
            if (!ChatClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                ChatClientManager.Instance.Stop();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }

            Logger.Log.Info("Listening on port:" + ChatClientManager.Instance.Port);
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
                                foreach (Session<BNSChatOpcodes> i in ChatClientManager.Instance.Clients.ToArray())
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
            ChatClientManager.Instance.Stop();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
            Logger.Log.Error("Error Message:" + ex.Message);
            Logger.Log.Error("Call Stack:" + ex.StackTrace);
            Logger.Log.Error("Trying to save all player's data");

            ChatClientManager.Instance.Stop();
        }
    }
}
