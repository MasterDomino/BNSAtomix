using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using SmartEngine.Network;
using SmartEngine.Network.VirtualFileSystem;
using SagaBNS.GameServer.Manager;
using SagaBNS.GameServer.Map;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Network;
using SagaBNS.GameServer.Item;
using SagaBNS.GameServer.Skills;
using SagaBNS.GameServer.Effect;
using SagaBNS.GameServer.BaGua;
using SagaBNS.GameServer.NPC;
using SagaBNS.GameServer.Network.AccountServer;
using SagaBNS.GameServer.Network.CharacterServer;
using SagaBNS.GameServer.Quests;
using SagaBNS.GameServer.Portal;
using SmartEngine.Core;
using System.Reflection;
using log4net;

namespace SagaBNS.GameServer
{
    internal static class GameServer
    {
        public static bool shuttingDown;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Shutdown);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.InitializeLogger(LogManager.GetLogger(typeof(GameServer)));

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string text = $"SagaBNS Game Server v{fileVersionInfo.ProductVersion} - Master's Edit";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

            Logger.Log.Info("Initializing VirtualFileSystem...");
            VirtualFileSystemManager.Instance.Init(FileSystems.Real, ".");

            Logger.Log.Info("Loading Configuration File...");
            Configuration.Instance.Initialization("./Config/GameServer.xml");

            Network<GamePacketOpcode>.SuppressUnknownPackets = !Debugger.IsAttached;
            Map.Map.MAX_SIGHT_RANGE = 800;

            /*for (int i = 0; i < 200000; i++)
            {
                test t = new test(Global.Random.Next(100, 1000));
                t.Activate();
            }*/
            //Logger.CurrentLogger.LogLevel.Value = Configuration.Instance.LogLevel;
            SmartEngine.Network.Tasks.TaskManager.Instance.SetWorkerCount(4, 4);
            ItemFactory.Instance.Init("./DB/item_templates.xml", Encoding.UTF8);
            SkillFactory.Instance.Init("./DB/skill_templates.xml", Encoding.UTF8);
            SkillManager.Instance.Init();
            ExperienceManager.Instance.Init("./DB/level.xml", Encoding.UTF8);
            EffectManager.Instance.Init("./DB/effect.xml", Encoding.UTF8);
            BaGuaManager.Instance.Init("./DB/baguaset.xml", Encoding.UTF8);
            NPCDataFactory.Instance.Init("./DB/npc_templates.xml", Encoding.UTF8);
            NPCStoreFactory.Instance.Init(System.IO.Directory.GetFiles("./DB", "store*.xml"), Encoding.UTF8);
            NPCSpawnManager.Instance.Init("./DB/Spawns", Encoding.UTF8, true);
            CampfireSpawnManager.Instance.Init("./DB/campfires.xml", Encoding.UTF8);
            QuestManager.Instance.Init("./DB/Quests", Encoding.UTF8, true);
            FactionRelationFactory.Instance.Init("./DB/faction_relations.xml", Encoding.UTF8);

            Scripting.ScriptManager.Instance.LoadScript("./Scripts");

            PortalDataManager.Instance.Init("./DB/portal_templates.xml", Encoding.UTF8);
            MapManager.Instance.Init("./DB/map_templates.xml", Encoding.UTF8);
            MapManager.Instance.InitStandardMaps();

            ClientManager<AccountPacketOpcode>.InitialSendCompletionPort = 10;
            ClientManager<AccountPacketOpcode>.NewSendCompletionPortEveryBatch = 2;
            ClientManager<CharacterPacketOpcode>.InitialSendCompletionPort = 10;
            ClientManager<CharacterPacketOpcode>.NewSendCompletionPortEveryBatch = 2;

            Logger.Log.Info($"Connecting account server at {Configuration.Instance.AccountHost}:{Configuration.Instance.AccountPort}");
            AccountSession.Instance.Host = Configuration.Instance.AccountHost;
            AccountSession.Instance.Port = Configuration.Instance.AccountPort;
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

            Logger.Log.Info($"Connecting character server at {Configuration.Instance.CharacterHost}:{Configuration.Instance.CharacterPort}");
            CharacterSession.Instance.Host = Configuration.Instance.CharacterHost;
            CharacterSession.Instance.Port = Configuration.Instance.CharacterPort;
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

            GameClientManager.Instance.Port = Configuration.Instance.Port;
            Encryption.Implementation = new Common.Encryption.BNSAESEncryption();
            Network<GamePacketOpcode>.Implementation = new Common.BNSGameNetwork<GamePacketOpcode>();
            if (!GameClientManager.Instance.Start())
            {
                Logger.Log.Error("Cannot Listen on port:" + Configuration.Instance.Port);
                Logger.Log.Error("Shutting down in 20sec.");
                GameClientManager.Instance.Stop();
                System.Threading.Thread.Sleep(20000);
                Environment.Exit(0);
                return;
            }

            Logger.Log.Info("Listening on port:" + GameClientManager.Instance.Port);
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
                        case "savehm":
                            SaveHeightMaps(true);
                            break;

                        case "filterhm":
                            FilterHeightMaps();
                            break;

                        case "printthreads":
                            ClientManager.PrintAllThreads();
                            break;

                        case "printband":
                            int sendTotal = 0;
                            int receiveTotal = 0;
                            Logger.Log.Warn("Bandwidth usage information:");
                            foreach (Session<GamePacketOpcode> i in GameClientManager.Instance.ValidClients)
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
                                    Logger.Log.Warn($"Total Session:{GameClientManager.Instance.Clients.Count} OnlinePlayer:{GameClientManager.Instance.ValidClients.Count}");
                                }
                            }
                            break;

                        case "status":
                            {
                                Logger.Log.Warn($"TaskManager:\r\n       Total Tasks:{SmartEngine.Network.Tasks.TaskManager.Instance.RegisteredCount} Execution/s:{SmartEngine.Network.Tasks.TaskManager.Instance.ExecutionCountPerMinute / 60} Avg Exection Time:{SmartEngine.Network.Tasks.TaskManager.Instance.AverageExecutionTime}ms \r\n       Avg Schedule Time:{SmartEngine.Network.Tasks.TaskManager.Instance.AverageScheduleTime}ms");
                                Logger.Log.Warn($"BufferManager:\r\n       TotalAllocatedMemory:{(float)SmartEngine.Network.Memory.BufferManager.Instance.TotalAllocatedMemory / 1024 / 1024:0.00}MB FreeMemory:{(float)SmartEngine.Network.Memory.BufferManager.Instance.FreeMemory / 1024 / 1024:0.##}MB");
                                Logger.Log.Warn("Network Status:");
                                Logger.Log.Warn("GameServer:");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{GameClientManager.CurrentCompletionPort} Free IOCPs:{GameClientManager.FreeCompletionPort}");
                                Logger.Log.Warn("AccountSession:");
                                Logger.Log.Warn($"       Receive:{(float)AccountSession.Instance.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)AccountSession.Instance.Network.UpStreamBand / 1024:0.##}KB/s");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{ClientManager<AccountPacketOpcode>.CurrentCompletionPort} Free IOCPs:{ClientManager<AccountPacketOpcode>.FreeCompletionPort}");
                                Logger.Log.Warn("CharacterSession:");
                                Logger.Log.Warn($"       Receive:{(float)CharacterSession.Instance.Network.DownStreamBand / 1024:0.##}KB/s Send:{(float)CharacterSession.Instance.Network.UpStreamBand / 1024:0.##}KB/s");
                                Logger.Log.Warn($"IOCP: \r\n       CurrentIOCPs:{ClientManager<CharacterPacketOpcode>.CurrentCompletionPort} Free IOCPs:{ClientManager<CharacterPacketOpcode>.FreeCompletionPort}");
                                Logger.Log.Warn($"Total Session:{GameClientManager.Instance.Clients.Count} OnlinePlayer:{GameClientManager.Instance.ValidClients.Count}");
                                Logger.Log.Warn($"Total Managed Ram:{(float)GC.GetTotalMemory(false) / 1024 / 1024:0.00} MB, Total Process Ram:{(float)Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024:0.00}MB");
                            }
                            break;

                        case "toptasks":
                            {
                                Logger.Log.Warn("Top 10 Tasks:");
                                foreach (KeyValuePair<string, int> i in SmartEngine.Network.Tasks.TaskManager.Instance.TenTasksWithMostInstances)
                                {
                                    Logger.Log.Warn($"{i.Key} : {i.Value} instances");
                                }
                            }
                            break;

                        case "mem":
                            {
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

        private static void FilterHeightMaps()
        {
            foreach (HeightMapBuilder i in Map.Map.heightmapBuilder.Values)
            {
                try
                {
                    i.Filter();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        private static void SaveHeightMaps(bool saveBMP)
        {
            foreach (string i in Map.Map.heightmapBuilder.Keys)
            {
                System.IO.FileStream fs = null;
                try
                {
                    if (Map.Map.heightmapBuilder[i].MinX == 0 && Map.Map.heightmapBuilder[i].MaxX == 0)
                    {
                        continue;
                    }

                    fs = new System.IO.FileStream("DB/HeightMaps/" + i + ".builder", System.IO.FileMode.Create);
                    Map.Map.heightmapBuilder[i].ToStream(fs);
                    if (saveBMP)
                    {
                        Map.Map.heightmapBuilder[i].ToBMP("DB/HeightMaps/" + i + ".psd");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
                finally
                {
                    fs?.Close();
                }
            }

            Logger.Log.Info("Heightmaps saved");
        }

        private static void Shutdown(object sender, ConsoleCancelEventArgs args)
        {
            if (!shuttingDown)
            {
                shuttingDown = true;
                Logger.Log.Info("Closing.....");
                GameClientManager.Instance.Stop();
                foreach (Network.Client.GameSession i in GameClientManager.Instance.Clients.ToArray())
                {
                    try
                    {
                        i.Network.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(ex);
                    }
                }
                Logger.Log.Info("Saving Heightmap information...");
                SaveHeightMaps(false);
                AccountSession.Instance.Disconnecting = true;
                AccountSession.Instance.Network.Disconnect(true);
                CharacterSession.Instance.Disconnecting = true;
                CharacterSession.Instance.Network.Disconnect(true);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!shuttingDown)
            {
                shuttingDown = true;
                Exception ex = e.ExceptionObject as Exception;

                Logger.Log.Error("Fatal: An unhandled exception is thrown, terminating...");
                Logger.Log.Error("Error Message:" + ex.Message);
                Logger.Log.Error("Call Stack:" + ex.StackTrace);
                GameClientManager.Instance.Stop();
                Logger.Log.Error("Trying to save all player's data");
                foreach (Network.Client.GameSession i in GameClientManager.Instance.Clients.ToArray())
                {
                    try
                    {
                        i.Network.Disconnect();
                    }
                    catch (Exception exc)
                    {
                        Logger.Log.Error(exc);
                    }
                }
                Logger.Log.Info("Saving Heightmap information...");

                SaveHeightMaps(false);
                AccountSession.Instance.Disconnecting = true;
                AccountSession.Instance.Network.Disconnect(true);
                CharacterSession.Instance.Disconnecting = true;
                CharacterSession.Instance.Network.Disconnect(true);
            }
        }
    }
}
