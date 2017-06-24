using System;
using System.Linq;
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.AccountServer.Manager;
using log4net;
using System.Reflection;
using System.Diagnostics;
using SmartEngine.Network.VirtualFileSystem;

namespace SagaBNS.AccountServer
{
    internal static class AccountServer
    {
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutingDown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Logger.InitializeLogger(LogManager.GetLogger(typeof(AccountServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Account Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/AccountServer.xml");
            //Logger.CurrentLogger.LogLevel.Value = Configuration.Instance.LogLevel;

            if (!StartDatabase())
            {
                Logger.Log.Error("Cannot connect to Mysql server");
                Logger.Log.Error("Shutting down in 20sec.");
                System.Threading.Thread.Sleep(20000);
                return;
            }

            AccountClientManager.Instance.Port = Configuration.Instance.Port;
            if (!AccountClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                AccountClientManager.Instance.Stop();
                Database.AccountDB.Instance.Shutdown();
                System.Threading.Thread.Sleep(20000);
                return;
            }

            Logger.Log.Info("Listening on port:" + AccountClientManager.Instance.Port);
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
                                foreach (Session<AccountPacketOpcode> i in AccountClientManager.Instance.Clients.ToArray())
                                {
                                    sendTotal += i.Network.UpStreamBand;
                                    receiveTotal += i.Network.DownStreamBand;
                                    Logger.Log.Warn(string.Format("Client:{0} Receive:{1:0.##}KB/s Send:{2:0.##}KB/s",
                                        i.ToString(),
                                        (float)i.Network.DownStreamBand / 1024,
                                        (float)i.Network.UpStreamBand / 1024));
                                }
                            }
                            catch
                            {
                            }
                            Logger.Log.Warn(string.Format("Total: Receive:{0:0.##}KB/s Send:{1:0.##}KB/s",
                                        (float)receiveTotal / 1024,
                                        (float)sendTotal / 1024));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        public static bool StartDatabase()
        {
            try
            {
                Database.AccountDB.Instance.Init(Configuration.Instance.DBHost,
                                                 Configuration.Instance.DBPort,
                                                 Configuration.Instance.DBName,
                                                 Configuration.Instance.DBUser,
                                                 Configuration.Instance.DBPassword);
                return Database.AccountDB.Instance.Connect();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ShutingDown(object sender, ConsoleCancelEventArgs args)
        {
            Logger.Log.Info("Closing.....");
            AccountClientManager.Instance.Stop();
            Database.AccountDB.Instance.Shutdown();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
            Logger.Log.Error("Error Message:" + ex.Message);
            Logger.Log.Error("Call Stack:" + ex.StackTrace);
            Logger.Log.Error("Trying to save all player's data");

            AccountClientManager.Instance.Stop();
            Database.AccountDB.Instance.Shutdown();
        }
    }
}
