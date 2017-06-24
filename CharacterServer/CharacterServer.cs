using System;
using System.Linq;
using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Manager;
using log4net;
using System.Reflection;
using System.Diagnostics;
using SmartEngine.Network.VirtualFileSystem;

namespace SagaBNS.CharacterServer
{
    internal static class CharacterServer
    {
        private static DateTime upTime;
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutingDown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.InitializeLogger(LogManager.GetLogger(typeof(CharacterServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Character Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/CharacterServer.xml");
            upTime = DateTime.Now;
            if (!StartDatabase())
            {
                Logger.Log.Error("Cannot connect to Mysql server");
                Logger.Log.Error("Shutting down in 20sec.");
                System.Threading.Thread.Sleep(20000);
                return;
            }

            CharacterClientManager.Instance.Port = Configuration.Instance.Port;
            if (!CharacterClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                CharacterClientManager.Instance.Stop();
                Database.CharacterDB.Instance.Shutdown();
                Database.ItemDB.Instance.Shutdown();
                System.Threading.Thread.Sleep(20000);
                return;
            }

            Logger.Log.Info("Listening on port:" + CharacterClientManager.Instance.Port);
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
                            foreach (Session<CharacterPacketOpcode> i in CharacterClientManager.Instance.Clients.ToArray())
                            {
                                try
                                {
                                    sendTotal += i.Network.UpStreamBand;
                                    receiveTotal += i.Network.DownStreamBand;
                                    Logger.Log.Warn($"Client:{i} Receive:{(float)i.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)i.Network.UpStreamBand / 1024:0.##}KB/s");
                                }
                                catch
                                {
                                    Logger.Log.Warn($"Total: Receive:{(float)receiveTotal / 1024:0.##}KB/s Send:{(float)sendTotal / 1024:0.##}KB/s");
                                }
                            }
                            break;

                        case "status":
                            PrintStatus();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        private static void PrintStatus()
        {
            Logger.Log.Warn("Running Status For Caches:");
            PrintCacheStatus(Cache.CharacterCache.Instance, "CharacterCache");
            PrintCacheStatus(Cache.ItemCache.Instance, "ItemCache");
            Logger.Log.Warn("Running Status For MySql DBs:");
            PrintMySQLStatus(Database.CharacterDB.Instance, "CharacterDB");
            PrintMySQLStatus(Database.ItemDB.Instance, "ItemDB");
        }

        private static void PrintCacheStatus<K, T>(SmartEngine.Network.Database.Cache.Cache<K, T> cache, string name)
        {
            Logger.Log.Warn($"{name}:\r\n       Usage:{cache.Count}/{cache.Capacity} ({(float)cache.Count * 100 / cache.Capacity:0.00}%)\r\n       Requests/Min:{(float)cache.CacheRequestTimes / (DateTime.Now - upTime).TotalMinutes:0.00} Hit Rate:{(float)cache.CacheRequestHitTimes * 100 / cache.CacheRequestTimes:0.00}%\r\n       Write Requests/Min:{(float)cache.WriteRequestTimes / (DateTime.Now - upTime).TotalMinutes:0.00} Avg Write Wait Time:{cache.WriteAvgWaitingTime.TotalSeconds}s\r\n       Old data clean-up triggered:{(float)cache.EldestDataClearTimes / (DateTime.Now - upTime).TotalHours:0.00}/Hour Avg clean-up trigger time:{cache.EldestDataClearAvgTime}");
        }

        private static void PrintMySQLStatus(SmartEngine.Network.Database.MySQLConnectivity sql, string name)
        {
            Logger.Log.Warn($"{name}:\r\n       Requests/Min:{sql.RequestsPerMinute}  Execution/Min:{sql.ExecutionPerMinute}  Queue Length:{sql.QueueLength}");
        }

        public static bool StartDatabase()
        {
            try
            {
                Database.CharacterDB.Instance.Init(Configuration.Instance.DBHost, Configuration.Instance.DBPort, Configuration.Instance.DBName, Configuration.Instance.DBUser, Configuration.Instance.DBPassword);
                Database.ItemDB.Instance.Init(Configuration.Instance.DBHost, Configuration.Instance.DBPort, Configuration.Instance.DBName, Configuration.Instance.DBUser, Configuration.Instance.DBPassword);
                return Database.CharacterDB.Instance.Connect() && Database.ItemDB.Instance.Connect();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ShutingDown(object sender, ConsoleCancelEventArgs args)
        {
            Logger.Log.Info("Closing.....");
            CharacterClientManager.Instance.Stop();
            Cache.CharacterCache.Instance.Shutdown();
            Database.CharacterDB.Instance.Shutdown();
            Cache.ItemCache.Instance.Shutdown();
            Database.ItemDB.Instance.Shutdown();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
            Logger.Log.Error("Error Message:" + ex.Message);
            Logger.Log.Error("Call Stack:" + ex.StackTrace);
            Logger.Log.Error("Trying to save all player's data");

            CharacterClientManager.Instance.Stop();
            Cache.CharacterCache.Instance.Shutdown();
            Database.CharacterDB.Instance.Shutdown();
            Cache.ItemCache.Instance.Shutdown();
            Database.ItemDB.Instance.Shutdown();
        }
    }
}
