using System;
using System.Collections.Generic;
using System.Linq;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Map;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Skills;
using SagaBNS.GameServer.Manager;
using SagaBNS.GameServer.Quests;
using SagaBNS.GameServer.Portal;
using SagaBNS.Common.Packets.GameServer;

using SagaBNS.GameServer.NPC;

namespace SagaBNS.GameServer.Command
{
    public class Commands : Singleton<Commands>
    {
        private delegate void ProcessCommandFunc(GameSession client, string args);

        private class CommandInfo
        {
            public ProcessCommandFunc func;
            public uint level;

            public CommandInfo(ProcessCommandFunc func, uint lvl)
            {
                this.func = func;
                level = lvl;
            }
        }

        private readonly Dictionary<string, CommandInfo> commandTable;
        private static readonly string _MasterName = "Saga";

        public Commands()
        {
            commandTable = new Dictionary<string, CommandInfo>();

            #region "Prefixes"
            string OpenCommandPrefix = "/";
            string GMCommandPrefix = "!";
            string RemoteCommandPrefix = "~";
            #endregion

            commandTable.Add(GMCommandPrefix + "raw", new CommandInfo(new ProcessCommandFunc(ProcessRaw), 99));
            commandTable.Add(GMCommandPrefix + "combat", new CommandInfo(new ProcessCommandFunc(ProcessCombat), 20));
            commandTable.Add(GMCommandPrefix + "item", new CommandInfo(new ProcessCommandFunc(ProcessItem), 20));
            commandTable.Add(GMCommandPrefix + "test", new CommandInfo(new ProcessCommandFunc(ProcessTest), 60));
            commandTable.Add(GMCommandPrefix + "test7", new CommandInfo(new ProcessCommandFunc(ProcessTest7), 60));
            commandTable.Add(GMCommandPrefix + "bc", new CommandInfo(new ProcessCommandFunc(ProcessBroadcast), 20));
            commandTable.Add(GMCommandPrefix + "bcevt", new CommandInfo(new ProcessCommandFunc(ProcessBroadcastEvent), 60));
            commandTable.Add(GMCommandPrefix + "addskill", new CommandInfo(new ProcessCommandFunc(ProcessAddSkill), 20));
            commandTable.Add(GMCommandPrefix + "level", new CommandInfo(new ProcessCommandFunc(ProcessLevel), 20));
            commandTable.Add(GMCommandPrefix + "mob", new CommandInfo(new ProcessCommandFunc(ProcessMob), 20));
            commandTable.Add(GMCommandPrefix + "reloadconfig", new CommandInfo(new ProcessCommandFunc(ProcessReloadConfig), 40));
            commandTable.Add(GMCommandPrefix + "reloadscript", new CommandInfo(new ProcessCommandFunc(ProcessReloadScript), 40));
            commandTable.Add(GMCommandPrefix + "faction", new CommandInfo(new ProcessCommandFunc(ProcessFactions), 20));
            commandTable.Add(GMCommandPrefix + "identifynpc", new CommandInfo(new ProcessCommandFunc(ProcessIdentifyNPC), 20));
            commandTable.Add(GMCommandPrefix + "deletespawn", new CommandInfo(new ProcessCommandFunc(ProcessDeleteSpawn), 60));
            commandTable.Add(GMCommandPrefix + "savespawn", new CommandInfo(new ProcessCommandFunc(ProcessSaveSpawn), 60));
            commandTable.Add(GMCommandPrefix + "setspawndelay", new CommandInfo(new ProcessCommandFunc(ProcessSetSpawnDelay), 60));
            commandTable.Add(GMCommandPrefix + "setspawnmoverange", new CommandInfo(new ProcessCommandFunc(ProcessSetSpawnMoveRange), 60));
            commandTable.Add(GMCommandPrefix + "addplayerrespawn", new CommandInfo(new ProcessCommandFunc(ProcessAddPlayerRespawn), 60));
            commandTable.Add(GMCommandPrefix + "savemaptemplates", new CommandInfo(new ProcessCommandFunc(ProcessSaveMapTemplates), 60));
            commandTable.Add(GMCommandPrefix + "tt", new CommandInfo(new ProcessCommandFunc(ProcessMapInfo), 60));
            commandTable.Add(GMCommandPrefix + "who", new CommandInfo(new ProcessCommandFunc(ProcessWho2), 20));
            commandTable.Add(GMCommandPrefix + "purgehm", new CommandInfo(new ProcessCommandFunc(ProcessPurgeHeightMap), 99));
            commandTable.Add(GMCommandPrefix + "quest", new CommandInfo(new ProcessCommandFunc(ProcessQuest), 60));
            commandTable.Add(OpenCommandPrefix + "where", new CommandInfo(new ProcessCommandFunc(ProcessWhere), 0));
            commandTable.Add(OpenCommandPrefix + "who", new CommandInfo(new ProcessCommandFunc(ProcessWho), 0));
            commandTable.Add(OpenCommandPrefix + "debugteleport", new CommandInfo(new ProcessCommandFunc(TeleportPointDistances), 60));
            commandTable.Add(GMCommandPrefix + "skilltree", new CommandInfo(new ProcessCommandFunc(ProcessTestTree), 1));
            commandTable.Add(GMCommandPrefix + "movetome", new CommandInfo(new ProcessCommandFunc(ProcessMoveToMe), 20));
            commandTable.Add(GMCommandPrefix + "moveto", new CommandInfo(new ProcessCommandFunc(ProcessMoveTo), 20));
            commandTable.Add(GMCommandPrefix + "cutscene", new CommandInfo(new ProcessCommandFunc(ProcessCutScene), 20));
            commandTable.Add(GMCommandPrefix + "gold", new CommandInfo(new ProcessCommandFunc(ProcessGold), 20));
            commandTable.Add(GMCommandPrefix + "kick", new CommandInfo(new ProcessCommandFunc(ProcessKick), 20));
            commandTable.Add(OpenCommandPrefix + "f4ct10n", new CommandInfo(new ProcessCommandFunc(ProcessTestFactions), 0));
            //this.commandTable.Add(OpenCommandPrefix + "resurrection", new CommandInfo(new ProcessCommandFunc(this.ProcessHelp), 0));            
        }

        private void ProcessCombat(GameSession client, string args)
        {
            client.ChangeCombatStatus(args == "1");
        }

        private void ProcessAddPlayerRespawn(GameSession client, string args)
        {
            if (!uint.TryParse(args, out uint mapID))
            {
                mapID = client.Map.ID;
            }

            if (!MapManager.Instance.RespawnPoints.ContainsKey(mapID))
            {
                MapManager.Instance.RespawnPoints.Add(mapID, new List<RespawnPoint>());
            }

            RespawnPoint p;

            p.MapID = client.Map.ID;
            p.X = (short)client.Character.X;
            p.Y = (short)client.Character.Y;
            p.Z = (short)client.Character.Z;
            p.Dir = client.Character.Dir;
            p.teleportId = 0;
            MapManager.Instance.RespawnPoints[mapID].Add(p);

            SM_SERVER_MESSAGE p2 = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = "Player revive respawn point added."
            };
            client.Network.SendPacket(p2);
        }

        private void ProcessSaveMapTemplates(GameSession client, string args)
        {
            Map.MapManager.Instance.Save("./DB/map_templates.xml");

            SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = "Map templates saved."
            };
            client.Network.SendPacket(p);
        }

        private void ProcessIdentifyNPC(GameSession client, string args)
        {
            var npcs = from npc in client.Map.GetActorsAroundActor(client.Character, 50, false)
                       where npc is ActorNPC
                       orderby client.Character.DirectionRelativeToTarget(npc)
                       select ((ActorNPC)npc).NpcID;
            ushort npcID = npcs.FirstOrDefault();

            SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = npcID > 0 ? string.Format("NPCID:{0} [{1}]", npcID, NPCDataFactory.Instance.Items[npcID].Faction.ToString()) : "No NPC found!"
            };
            client.Network.SendPacket(p);
        }

        private void ProcessSetSpawnMoveRange(GameSession client, string args)
        {
            int count = 1000;
            string[] token = args.Split(' ');
            if (!int.TryParse(token[0], out int moveRange))
            {
                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = "You have to specify a move range!"
                };
                client.Network.SendPacket(p);
            }
            else
            {
                if (token.Length == 1 || !int.TryParse(token[1], out int range))
                {
                    count = 1;
                    range = 50;
                }
                var spawns = from spawn in NPC.NPCSpawnManager.Instance[client.Map.ID]
                             where !spawn.IsMapObject && client.Character.DistanceToPoint(spawn.X, spawn.Y, spawn.Z) < range
                             orderby client.Character.DirectionRelativeToTarget(spawn.X, spawn.Y)
                             select spawn;
                int counter = 0;
                foreach (NPC.SpawnData i in spawns.Take(count))
                {
                    i.MoveRange = moveRange;
                    counter++;
                }
                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = counter + " Spawn data updated."
                };
                client.Network.SendPacket(p);
            }
        }

        private void ProcessSetSpawnDelay(GameSession client, string args)
        {
            int count = 1000;
            string[] token = args.Split(' ');
            if (!int.TryParse(token[0], out int delay))
            {
                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = "You have to specify a delay!"
                };
                client.Network.SendPacket(p);
            }
            else
            {
                if (token.Length == 1 || !int.TryParse(token[1], out int range))
                {
                    count = 1;
                    range = 50;
                }
                var spawns = from spawn in NPC.NPCSpawnManager.Instance[client.Map.ID]
                             where !spawn.IsMapObject && client.Character.DistanceToPoint(spawn.X, spawn.Y, spawn.Z) < range
                             orderby client.Character.DirectionRelativeToTarget(spawn.X, spawn.Y)
                             select spawn;
                int counter = 0;
                foreach (NPC.SpawnData i in spawns.Take(count))
                {
                    i.Delay = delay;
                    counter++;
                }
                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = counter + " Spawn data updated."
                };
                client.Network.SendPacket(p);
            }
        }

        private void ProcessDeleteSpawn(GameSession client, string args)
        {
            if (!int.TryParse(args, out int count))
            {
                count = 1;
            }

            var spawns = from spawn in NPC.NPCSpawnManager.Instance[client.Map.ID]
                         where !spawn.IsMapObject && client.Character.DistanceToPoint(spawn.X, spawn.Y, spawn.Z) < 50
                         orderby client.Character.DirectionRelativeToTarget(spawn.X, spawn.Y)
                         select spawn;
            int counter = 0;
            foreach (NPC.SpawnData i in spawns.Take(count))
            {
                NPC.NPCSpawnManager.Instance[client.Map.ID].Remove(i);
                counter++;
            }
            SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = counter + "Spawn data deleted."
            };
            client.Network.SendPacket(p);
        }

        private void ProcessSaveSpawn(GameSession client, string args)
        {
            NPC.NPCSpawnManager.Instance.Save("./DB/Spawns");

            SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
            {
                MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                Message = "Spawn data saved."
            };
            client.Network.SendPacket(p);
        }

        private void ProcessPurgeHeightMap(GameSession client, string args)
        {
            int range = int.Parse(args);
            client.Map.HeightMapBuilder.Purge((short)client.Character.X, (short)client.Character.Y, range);
        }

        private void ProcessWho2(GameSession client, string args)
        {
            List<GameSession> clients = GameClientManager.Instance.ValidClients;
            foreach (GameSession i in clients)
            {
                if (i.Character == null || i.Account == null || i.Map == null)
                {
                    continue;
                }

                string who = string.Format("{0}({1}) Map:{2}(Instance:{3}) {4},{5},{6}", i.Character.Name,
                    i.Account.UserName, i.Map.ID, i.Map.InstanceID, i.Character.X, i.Character.Y, i.Character.Z);

                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = who
                };
                client.Network.SendPacket(p);
            }
            {
                string who = string.Format("Total Online Player:{0}", clients.Count);

                client.SendServerMessage(who, SM_SERVER_MESSAGE.Positions.ChatWindow);
            }
        }

        private void ProcessWho(GameSession client, string args)
        {
            string who = string.Format("Total Online Player:{0}", GameClientManager.Instance.ValidClients.Count);

            client.SendServerMessage(who, SM_SERVER_MESSAGE.Positions.ChatWindow);
        }

        private void ProcessWhere(GameSession client, string args)
        {
            string where = string.Format("Map:{0}(Instance:{1}) {2},{3},{4} Dir:{5}", client.Map.ID, client.Map.InstanceID, client.Character.X, client.Character.Y, client.Character.Z, client.Character.Dir);

            client.SendServerMessage(where, SM_SERVER_MESSAGE.Positions.ChatWindow);
        }

        private void ProcessItem(GameSession client, string args)
        {
            ushort number;
            uint id = 0;
            if (args == "")
            {
                //client.SendSystemMessage(LocalManager.Instance.Strings.ATCOMMAND_ITEM_PARA);
            }
            else
            {
                try
                {
                    switch (args.Split(' ').Length)
                    {
                        case 1:
                            number = 1;
                            id = uint.Parse(args);
                            break;
                        case 2:
                            id = uint.Parse(args.Split(' ')[0]);
                            number = ushort.Parse(args.Split(' ')[1]);
                            break;
                        default:
                            number = 1;
                            uint.Parse(args);
                            break;
                    }

                    Common.Item.Item item = client.AddItem(id, number);
                    //client.SendItemList();
                    //client.SendItemUpdate(Common.Inventory.OperationResults.NEW_INDEX, item);
                    //((ActorEventHandlers.PCEventHandler)pc.EventHandler).Client.AddItem(itemID);

                }
                catch (Exception) { }
            }
        }

        private void ProcessRaw(GameSession client, string args)
        {
            byte[] buf = HexStr2Bytes(args.Replace(" ", ""));
            Packet<GamePacketOpcode> p = new Packet<GamePacketOpcode>();

            p.PutBytes(buf);
            client.Network.SendPacket(p);
        }

        private void ProcessBroadcast(GameSession client, string args)
        {
            foreach (GameSession i in GameClientManager.Instance.ValidClients)
            {
                i.SendServerMessage(args, SM_SERVER_MESSAGE.Positions.Top);
                i.SendServerMessage(args, SM_SERVER_MESSAGE.Positions.ChatWindow);
            }
        }

        private void ProcessBroadcastEvent(GameSession client, string args)
        {
            string[] token = args.Split(' ');
            Map.UpdateEvent evt = new Map.UpdateEvent()
            {
                UpdateType = Map.UpdateTypes.Actor
            };
            ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
            if (token[0] == "0")
            {
                actorID = client.Character.ActorID;
            }
            else
            {
                actorID |= uint.Parse(token[0]);
            }

            Actor actor = client.Map.GetActor(actorID);
            if (actor != null)
            {
                evt.Actor = actor;
                evt.Target = actor;
                for (int i = 0; i < (token.Length - 1) / 2; i++)
                {
                    evt.AddActorPara((Common.Packets.GameServer.PacketParameter)Convert.ToInt32(token[1 + i * 2], 16), long.Parse(token[2 + i * 2]));
                }
                client.Map.SendEventToAllActorsWhoCanSeeActor(Map.MapEvents.EVENT_BROADCAST, evt, actor, true);
            }
        }

        private void ProcessTest7(GameSession client, string args)
        {
            string[] token = args.Split(' ');
            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST();
            UpdateEvent evt = new UpdateEvent();
            for (int i = 0; i < token.Length / 2; i++)
            {
                evt.AddActorPara((Common.Packets.GameServer.PacketParameter)Convert.ToUInt16(token[i * 2], 16), long.Parse(token[i * 2 + 1]));
            }
            p.Parameters = evt;
            client.Network.SendPacket(p);
        }

        private void ProcessTestTree(GameSession client, string args)
        {
            byte[] buf = HexStr2Bytes("42003F00000000000000000000000012010000000000000000000000000000F0000E04020000000000F0FF3F178F1BD3FB7BFFFF87E13020DF39E87CE2F5075D10FC010400000000000A00BD014E0C634E00000000");
            Packet<GamePacketOpcode> p = new Packet<GamePacketOpcode>();
            p.PutBytes(buf);
            client.Network.SendPacket(p);
           }

        private void ProcessTest(GameSession client, string args)
        {
            Packet<GamePacketOpcode> p2 = new Packet<GamePacketOpcode>();
            p2.PutUShort(0x42, 0);
            p2.PutUShort(0x29);
            p2.PutBytes(new byte[27]);
            p2.PutUShort(0xF0);
            p2.PutUInt(0xFFFFFFFF);
            p2.PutUInt(0xffffffff);
            p2.PutUShort(0xFFFF);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUShort(0xffff);
            p2.PutUInt(4);
            p2.PutShort(0);
            p2.PutUInt(4);
            p2.PutShort(0);
            p2.PutUShort((ushort)(p2.Length - 16), 2);
            client.Network.SendPacket(p2);
            return;
            string[] token = args.Split(' ');
            Map.UpdateEvent evt = new Map.UpdateEvent()
            {
                Actor = client.Character,
                UpdateType = Map.UpdateTypes.Debug
            };
            Packet<int> tmp = new Packet<int>();
            switch (token[0])
            {
                case "424":
                    {
                        tmp.PutUShort(0x424, 0);
                        tmp.PutInt(65000);
                        tmp.PutShort(0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutShort(0);
                        tmp.PutShort(1);
                        tmp.PutShort(4);
                        tmp.PutShort(21);
                        tmp.PutShort(short.Parse(token[2]));
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                    }
                    break;
                case "423":
                    {
                        tmp.PutUShort(0x423, 0);
                        tmp.PutInt(65000);
                        tmp.PutShort(0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutShort(0);
                        tmp.PutShort(1);
                        tmp.PutShort(3);
                        tmp.PutShort(short.Parse(token[2]));
                        tmp.PutByte(byte.Parse(token[3]));
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                    }
                    break;
                case "421":
                    {
                        tmp.PutUShort(0x421, 0);
                        tmp.PutInt(10001012);
                        tmp.PutShort(8193);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutByte(2);
                        tmp.PutByte(255);
                        tmp.PutInt(0);
                        tmp.PutByte(3);
                        tmp.PutByte(9);
                        tmp.PutByte(2);
                    }
                    break;
                case "50e":
                    {
                        tmp.PutUShort(0x50e, 0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutByte(4);
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                        tmp.PutByte(32);
                    }
                    break;
                case "b0f":
                    {
                        tmp.PutUShort(0xb0f, 0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        if (token[2] == "1")
                        {
                            tmp.PutShort(368);
                            tmp.PutByte(1);
                            tmp.PutByte(20);
                            tmp.PutByte(1);
                        }
                        else
                        {
                            tmp.PutShort(368);
                            tmp.PutByte(0);
                            tmp.PutByte(2);
                            tmp.PutByte(1);
                        }
                    }
                    break;
                case "420":
                    {
                        tmp.PutUShort(0x420, 0);
                        tmp.PutInt(55111201);
                        tmp.PutShort(4097);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutShort(4609);
                        tmp.PutInt(0);
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                    }
                    break;
                case "426":
                    {
                        tmp.PutUShort(0x426, 0);
                        tmp.PutInt(65000);
                        tmp.PutShort(0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutShort(0);
                        tmp.PutShort(1);
                        tmp.PutShort(6);
                        tmp.PutShort(0x18);
                        tmp.PutInt(0x62);
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                    }
                    break;
                case "518":
                    {
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutShort(0x518, 0);
                        SmartEngine.Network.Map.Actor actor = client.Map.GetActor(actorID);
                        tmp.PutULong(actorID);
                        tmp.PutByte(14);
                        tmp.PutByte(1);
                        tmp.PutByte(1);
                        tmp.PutByte(16);
                        tmp.PutShort((short)actor.X);
                        tmp.PutShort((short)actor.Y);
                        tmp.PutShort((short)actor.Z);
                        tmp.PutShort(0);
                        tmp.PutShort(1);
                    }
                    break;
                case "21f":
                    {
                        tmp.PutUShort(0x21F, 0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutUInt(5511120);
                        tmp.PutByte(0);
                        tmp.PutByte(0);
                        tmp.PutShort(300);
                        tmp.PutByte(byte.Parse(token[3]));
                        tmp.PutShort(short.Parse(token[4]));
                        tmp.PutByte(10);
                        tmp.PutByte(1);
                        actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[2]);
                        tmp.PutULong(actorID);
                    }
                    break;
                case "31a":
                    {
                        tmp.PutUShort(0x31A);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[2]);
                        tmp.PutULong(actorID);
                        tmp.PutUInt(5511120);
                        tmp.PutByte(0);
                        tmp.PutByte(0x0);
                        tmp.PutShort(4);
                    }
                    break;
                case "42a":
                    {
                        tmp.PutUShort(0x42a, 0);
                        tmp.PutInt(65000);
                        tmp.PutShort(0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        tmp.PutShort(0);
                        tmp.PutShort(1);
                        tmp.PutShort(10);
                        tmp.PutShort(0x24);//type
                        actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[2]);
                        tmp.PutULong(actorID);
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                    }
                    break;
                case "42b":
                    {
                        tmp.PutUShort(0x42B, 0);
                        tmp.PutInt(55100020);
                        tmp.PutShort(0);
                        ulong actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[1]);
                        tmp.PutULong(actorID);
                        actorID = 0x4000000000000 | ((ulong)client.Map.InstanceID << 32);
                        actorID |= uint.Parse(token[2]);
                        tmp.PutULong(actorID);
                        tmp.PutByte(0);
                        tmp.PutByte(95);
                        tmp.PutShort(1);
                        tmp.PutShort(6);
                        tmp.PutShort(24);
                        tmp.PutInt(100);//hp
                        tmp.PutByte(7);
                        tmp.PutByte(2);
                        tmp.PutByte(1);
                        tmp.PutInt(0);
                    }
                    break;
                case "1":
                    ProcessTest(client, "21f 14 13 1 7");
                    return;
                case "2":
                    // ProcessTest(client, "21f 8 14 1");
                    ProcessTest(client, "21f 14 13 4 7");
                    ProcessTest(client, "31a 14 13");
                    ProcessTest(client, "42b 14 13");
                    return;
                case "3":
                    ProcessTest(client, "21f 14 13 8 7");
                    return;
                default:
                    return;
            }
            evt.UserData = tmp.ToArray();

            client.Map.SendEventToAllActorsWhoCanSeeActor(Map.MapEvents.EVENT_BROADCAST, evt, client.Character, true);
        }

        private void ProcessAddSkill(GameSession client, string args)
        {
            uint SkillID = uint.MinValue;
            if (uint.TryParse(args, out SkillID))
            {
                SkillManager.Instance.PlayerAddSkill(client.Character, SkillID, true);
            }
        }

        private void ProcessLevel(GameSession client, string args)
        {
            uint Level = uint.MinValue;
            if (uint.TryParse(args, out Level))
            {
                if (Level > 50)
                {
                    Level = 50;
                }

                uint exp = ExperienceManager.Instance.GetExpForLevel(Level);
                client.Character.Level = 0;
                client.Character.Exp = exp;
                ExperienceManager.Instance.CheckExp(client.Character);
            }
        }

        private void ProcessMob(GameSession client, string args)
        {
            string[] command = args.Split(' ');
            uint id = uint.MinValue;
            if (uint.TryParse(command[0], out id))
            {
                if (NPCDataFactory.Instance.Items.ContainsKey(id))
                {
                    Map.Map map = Map.MapManager.Instance.GetMap(client.Character.MapInstanceID);
                    if (command.Length == 1)
                    {
                        Scripting.Utils.SpawnNPC(map, (ushort)id, (short)client.Character.X,
                            (short)client.Character.Y, (short)client.Character.Z, client.Character.Dir, 0);
                    }
                    else
                    {
                        Scripting.Utils.SpawnNPC(map, (ushort)id, (short)client.Character.X,
                            (short)client.Character.Y, (short)client.Character.Z, client.Character.Dir, ushort.Parse(command[1]));
                    }
                }
            }
            //SpawnNPC(map, 752, 62450, 9085, 588, 45, 374);
        }

        private void ProcessReloadConfig(GameSession client, string args)
        {
            switch (args)
            {
                case "quest":
                    client.SendServerMessage("reloadconfig Quest...", SM_SERVER_MESSAGE.Positions.ChatWindow);
                    QuestManager.Instance.Reload();
                    break;
                case "map":
                    PortalDataManager.Instance.Reload();
                    MapManager.Instance.Reload();
                    NPCSpawnManager.Instance.Reload();
                    CampfireSpawnManager.Instance.Reload();
                    client.SendServerMessage("reloadconfig MapData...", SM_SERVER_MESSAGE.Positions.ChatWindow);
                    break;
                case "npc":
                    NPCDataFactory.Instance.Reload();
                    client.SendServerMessage("reloadconfig Npc...", SM_SERVER_MESSAGE.Positions.ChatWindow);
                    break;
                case "skill":
                    SkillFactory.Instance.Reload();
                    client.SendServerMessage("reloadconfig Skill...", SM_SERVER_MESSAGE.Positions.ChatWindow);
                    break;
            }
        }

        private void ProcessReloadScript(GameSession client, string args)
        {
            Scripting.ScriptManager.Instance.NpcScripts.Clear();
            Scripting.ScriptManager.Instance.MapObjectScripts.Clear();
            Scripting.ScriptManager.Instance.LoadScript("./Scripts");
        }

        private void ProcessFactions(GameSession client, string args)
        {
            try
            {
                client.Character.Faction = (Factions)Enum.Parse(typeof(Factions), args, true);
                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = "Faction changed to:\"" + client.Character.Faction + "\""
                };
                client.Network.SendPacket(p);
                if ((int)client.Character.Faction < 5)
                {
                    SM_PLAYER_UPDATE_LIST r = new SM_PLAYER_UPDATE_LIST()
                    {

                        //evt.AddActorPara(PacketParameter.PlayerFaction, (byte)client.Character.Faction);
                        Parameters = new UpdateEvent()
                    };
                    client.Network.SendPacket(r);
                }
            }
            catch
            {
                string[] command = args.Split(' ');
                switch (command[0])
                {
                    case "mob":
                        if (NPCDataFactory.Instance.Items.ContainsKey(uint.Parse(command[1])))
                        {
                            NPCDataFactory.Instance[uint.Parse(command[1])].Faction = (Factions)Enum.Parse(typeof(Factions), command[2], true);
                        }
                        break;
                }
                if (args == "999")
                {
                    FactionRelationFactory.Instance[Factions.Monster][Factions.Player1] = Relations.Friendly;
                    FactionRelationFactory.Instance[Factions.MonsterAggressive][Factions.Player1] = Relations.Friendly;
                    FactionRelationFactory.Instance[Factions.Chunggakdan][Factions.Player1] = Relations.Friendly;
                }
                if (args == "000")
                {
                    FactionRelationFactory.Instance[Factions.Monster][Factions.Player1] = Relations.Neutral;
                    FactionRelationFactory.Instance[Factions.MonsterAggressive][Factions.Player1] = Relations.Enemy;
                    FactionRelationFactory.Instance[Factions.Chunggakdan][Factions.Player1] = Relations.Enemy;
                }
            }
        }

        private void ProcessTestFactions(GameSession client, string args)
        {
            try
            {
                if( args != null  && args != string.Empty)
                {
                    client.Character.Faction = Factions.PlayerSelfDefenceForce;
                }
                else
                {
                    client.Character.Faction = Factions.Player1;
                }

                SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                {
                    MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                    Message = "Faction changed to:\"" + client.Character.Faction + "\""
                };
                client.Network.SendPacket(p);
                if ((int)client.Character.Faction < 5)
                {
                    SM_PLAYER_UPDATE_LIST r = new SM_PLAYER_UPDATE_LIST()
                    {

                        //evt.AddActorPara(PacketParameter.PlayerFaction, (byte)client.Character.Faction);
                        Parameters = new UpdateEvent()
                    };
                    client.Network.SendPacket(r);
                }
            }
            catch
            {
            }
        }

        private void ProcessQuest(GameSession client, string args)
        {
            string[] command = args.Split(' ');
            switch (command[0])
            {
                case "list":
                    {
                        SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow
                        };
                        List<ushort> list = client.Character.Quests.Keys.ToList<ushort>();
                        p.Message = string.Format("QuestList:{0} ", string.Join(", ", list));
                        client.Network.SendPacket(p);
                    }
                    break;
                case "re":
                    if (client.Character.Quests.ContainsKey(ushort.Parse(command[1])))
                    {
                        if (command[2] == "0")
                        {
                            client.Character.Quests.TryRemove(ushort.Parse(command[1]), out Quest q);
                            client.FinishQuest(q);
                            q = null;

                            if (client.Character.QuestsCompleted.Contains(ushort.Parse(command[1])))
                            {
                                client.Character.QuestsCompleted.Remove(ushort.Parse(command[1]));
                            }

                            client.SendNextQuest(ushort.Parse(command[1]));
                        }
                        else
                        {
                            if (command.Length >= 7)
                            {
                                client.Character.Quests[ushort.Parse(command[1])].Step = byte.Parse(command[2]);
                                client.Character.Quests[ushort.Parse(command[1])].NextStep = byte.Parse(command[3]);
                                client.Character.Quests[ushort.Parse(command[1])].Flag1 = short.Parse(command[4]);
                                client.Character.Quests[ushort.Parse(command[1])].Flag2 = short.Parse(command[5]);
                                client.Character.Quests[ushort.Parse(command[1])].Flag3 = short.Parse(command[6]);
                                client.SendQuestUpdate(client.Character.Quests[ushort.Parse(command[1])]);
                            }
                        }
                    }
                    break;
                case "add":
                    if (!client.Character.Quests.ContainsKey(ushort.Parse(command[1])))
                    {
                        if (client.Character.QuestsCompleted.Contains(ushort.Parse(command[1])))
                        {
                            client.Character.QuestsCompleted.Remove(ushort.Parse(command[1]));
                        }

                        client.SendNextQuest(ushort.Parse(command[1]));
                    }
                    break;
                case "del":
                    if (client.Character.Quests.ContainsKey(ushort.Parse(command[1])))
                    {
                        client.Character.Quests.TryRemove(ushort.Parse(command[1]), out Quest q);
                        client.FinishQuest(q);
                        q = null;
                    }
                    if (client.Character.QuestsCompleted.Contains(ushort.Parse(command[1])))
                    {
                        client.Character.QuestsCompleted.Remove(ushort.Parse(command[1]));
                    }

                    break;
                case "fin":
                    if (client.Character.Quests.ContainsKey(ushort.Parse(command[1])))
                    {
                        client.Character.Quests.TryRemove(ushort.Parse(command[1]), out Quest q);
                        client.FinishQuest(q);
                        q = null;
                    }
                    if (!client.Character.QuestsCompleted.Contains(ushort.Parse(command[1])))
                    {
                        client.Character.QuestsCompleted.Add(ushort.Parse(command[1]));
                    }

                    break;
            }
            client.SendQuestList();
        }

        private void ProcessGold(GameSession client, string args)
        {
            string[] command = args.Split(' ');
            if (command.Length == 1)
            {
                SendGold(client, command[0]);
            }
            else
            {
                GameSession temp = GameClientManager.Instance.FindClient(command[1]);
                if (temp != null)
                {
                    SendGold(temp, command[0]);
                    SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                    {
                        MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                        Message = temp.Character.Name + " received the money."
                    };
                    client.Network.SendPacket(p);
                }
            }
        }

        private void ProcessKick(GameSession client, string args)
        {
            string[] command = args.Split(' ');
            if (command.Length == 1)
            {
                GameSession temp = GameClientManager.Instance.FindClient(command[0]);
                if (temp != null)
                {
                    temp.Network.Disconnect();
                }
            }
        }

        private void SendGold(GameSession client, string GoldS)
        {
            if (!int.TryParse(GoldS, out int gold))
            {
                gold = 0;
            }

            client.Character.Gold += gold;
            SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST();
            UpdateEvent evt = new UpdateEvent();
            evt.AddActorPara(PacketParameter.Gold, client.Character.Gold);
            p.Parameters = evt;
            client.Network.SendPacket(p);
        }

        private void ProcessMapInfo(GameSession client, string args)
        {
            string[] command = args.Split(new char[] { ' ' }, 2);
            if (command.Length == 1)
            {
                command = new string[] { command[0], "" };
            }

            switch (command[0])
            {
                case "speed":
                    {
                        if (!uint.TryParse(command[1], out uint speed))
                        {
                            speed = 60;
                        }

                        UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(client.Character, client.Character, 255, 28674, 65010, UpdateEvent.ExtraUpdateModes.Activate);
                        //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Speed, speed);
                        client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);

                        SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                            Message = "Speed: " + speed
                        };
                        client.Network.SendPacket(p);
                    }
                    break;
                case "fh":
                    ProcessAddPlayerRespawn(client, command[1]);
                    break;
                case "save":
                    ProcessSaveMapTemplates(client, args);
                    break;
                case "id":
                    ProcessIdentifyNPC(client, args);
                    break;
                case "mapre":
                    {
                        MapInstanceType mapType = client.Map.InstanceType;
                        mapType = (MapInstanceType)int.Parse(command[1]);
                        MapManager.Instance.Items[client.Map.ID].InstanceType = mapType;
                    }
                    {
                        SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                            Message = string.Format("MapID:{0},MapType:{1},Height:{2},RespawnPoints:{3}", client.Map.ID, client.Map.InstanceType, client.Map.HeightMapBuilder.Name, client.Map.RespawnPoints.Count)
                        };
                        client.Network.SendPacket(p);
                    }
                    break;
                case "mapinfo":
                    {
                        SM_SERVER_MESSAGE p = new SM_SERVER_MESSAGE()
                        {
                            MessagePosition = SM_SERVER_MESSAGE.Positions.ChatWindow,
                            Message = string.Format("MapID:{0},MapType:{1},Height:{2},RespawnPoints:{3}", client.Map.ID, client.Map.InstanceType, client.Map.HeightMapBuilder.Name, client.Map.RespawnPoints.Count)
                        };
                        client.Network.SendPacket(p);
                    }
                    break;
                case "warp":
                    {
                        if (command.LongLength == 1)
                        {
                            ProcessHelp(client, args);
                        }
                        else
                        {
                            client.Character.Status.ShouldLoadMap = true;
                            string[] zb = command[1].Split(' ');

                            Map.Map tMap = MapManager.Instance.GetMap(uint.Parse(zb[0]), client.Character.CharID, client.Character.PartyID);
                            Map.Map map = tMap;
                            RespawnPoint res;
                            if (zb.LongLength == 1)
                            {
                                res.X = tMap.RespawnPoints[0].X;
                                res.Y = tMap.RespawnPoints[0].Y;
                                res.Z = tMap.RespawnPoints[0].Z;
                                map = MapManager.Instance.GetMap(tMap.RespawnPoints[0].MapID, client.Character.CharID, client.Character.PartyID);
                            }
                            else
                            {
                                res.X = short.Parse(zb[1]);
                                res.Y = short.Parse(zb[2]);
                                res.Z = short.Parse(zb[3]);
                            }
                            client.Map.SendActorToMap(client.Character, map, res.X, res.Y, res.Z);
                        }
                        //*/
                    }
                    break;
                case "load":
                    ProcessReloadConfig(client, command[1]);
                    break;
                case "npcdel":
                    ProcessDeleteSpawn(client, command[1]);
                    break;
                case "npcsave":
                    ProcessSaveSpawn(client, command[1]);
                    break;
                case "qc":
                    {
                        foreach (KeyValuePair<ulong, Actor> i in Map.MapManager.Instance.Items[client.Map.ID].Actors)
                        {
                            if (i.Value.ActorType == ActorType.PC)
                            {
                                continue;
                            }

                            Map.MapManager.Instance.Items[client.Map.ID].DeleteActor(i.Value);
                        }
                    }
                    break;
                case "cz":
                    NPC.NPCSpawnManager.Instance.SpawnAll(client.Map.ID, client.Map);
                    break;
                case "hp":
                    client.Character.MaxHP = 20000;
                    client.Character.HP = 20000;
                    client.SendPlayerHP();
                    break;
            }
        }

        private void ProcessMoveToMe(GameSession client, string args)
        {
            GameSession temp = GameClientManager.Instance.FindClient(args);
            if (temp != null)
            {
                MovePlayer(temp, client);
            }
        }

        private void ProcessCutScene(GameSession client, string args)
        {
            client.SendQuestCutScene(uint.Parse(args));
        }

        private void ProcessMoveTo(GameSession client, string args)
        {
            GameSession temp = GameClientManager.Instance.FindClient(args);
            if (temp != null)
            {
                MovePlayer(client, temp);
            }
        }

        private void MovePlayer(GameSession from, GameSession to)
        {
            from.Character.Status.ShouldLoadMap = true;

            Map.Map map = MapManager.Instance.GetMap(to.Character.MapID, from.Character.CharID, from.Character.PartyID);
            RespawnPoint res;
            res.X = (short)to.Character.X;
            res.Y = (short)to.Character.Y;
            res.Z = (short)to.Character.Z;
            from.Map.SendActorToMap(from.Character, map, res.X, res.Y, res.Z);
        }

        private void ProcessHelp(GameSession client, string args)
        {
            RespawnPoint res = Point(client);
            if (res.MapID == 0)
            {
                client.SendServerMessage(string.Format("[MapID:{0}]Not Respawn Point!", client.Map.ID), SM_SERVER_MESSAGE.Positions.ChatWindow);
            }
            else
            {
                client.Character.Status.ShouldLoadMap = true;
                Map.Map tMap = MapManager.Instance.GetMap(res.MapID, client.Character.CharID, client.Character.PartyID);
                if (client.Map != null)
                {
                    client.Character.Dir = res.Dir;
                    client.Map.SendActorToMap(client.Character, tMap, res.X, res.Y, res.Z);
                }
                else
                {
                    client.Map.SendActorToMap(client.Character, client.Map, client.Character.X, client.Character.Y, client.Character.Z);
                }
            }
        }

        #region ""
        public byte ToByte(string Value)
        {
            if (Value == null)
            {
                return 0;
            }

            long num2;
            num2 = Convert.ToInt64(Value, 0x10);
            return (byte)num2;
        }

        public byte[] HexStr2Bytes(string s)
        {
            byte[] b = new byte[s.Length / 2];
            int i;
            for (i = 0; i < s.Length / 2; i++)
            {
                //b[i] = Conversions.ToByte( "&H" + s.Substring( i * 2, 2 ) );
                b[i] = ToByte(s.Substring(i * 2, 2));
            }
            return b;
        }

        public RespawnPoint Point(GameSession client)
        {
            var points = from point in client.Map.RespawnPoints
                         orderby client.Character.DistanceToPoint(point.X, point.Y, point.Z)
                         select point;
            return points.FirstOrDefault();
        }

        public RespawnPoint TeleportPoint(GameSession client)
        {
            var points = from point in client.Map.RespawnPoints
                         where point.teleportId > 0
                         orderby client.Character.DistanceToPoint(point.X, point.Y, point.Z)
                         select point;
            return points.FirstOrDefault();
        }

        private void TeleportPointDistances(GameSession client, string args)
        {
            List<RespawnPoint>[] temp = MapManager.Instance.RespawnPoints.Values.ToArray();
            List<RespawnPoint> temp2 = new List<RespawnPoint>();
            List<RespawnPoint> temp3 = new List<RespawnPoint>();
            List<RespawnPoint> temp4 = new List<RespawnPoint>();
            foreach (List<RespawnPoint> i in temp)
            {
                temp2.AddRange(i.ToArray());
            }

            temp = null;
            var points = from point in temp2
                         where point.teleportId > 0
                         orderby client.Character.DistanceToPoint2D(point.X, point.Y)
                         select point;
            temp2 = null;
            foreach (RespawnPoint i in points)
            {
                if (i.MapID / 1000 == 2)
                {
                    temp3.Add(i);
                }
                else
                {
                    temp4.Add(i);
                }
            }
            temp3.AddRange(temp4.ToArray());
            temp4 = null;
            foreach (RespawnPoint i in temp3)
            {
                Logger.Log.Info(string.Format("MapId:{0} TeleportId:{1} Distance:{2}", i.MapID, i.teleportId, client.Character.DistanceToPoint2D(i.X, i.Y)));
            }
            Logger.Log.Info("\r\n");
        }
        #endregion

        public bool ProcessCommand(GameSession client, string command)
        {
            try
            {
                string[] args = command.Split(new char[] { ' ' }, 2);
                args[0] = args[0].ToLower();

                if (commandTable.ContainsKey(args[0]))
                {
                    CommandInfo cInfo = commandTable[args[0]];

                    if (client.Account.GMLevel >= cInfo.level)
                    {
                        /*/
                        Logger.LogGMCommand(client.Character.Name + "(" + client.Character.CharID + ")", "",
                            string.Format("Account:{0}({1}) GMLv:{2} Command:{3}",
                            client.Character.Account.Name,
                            client.Character.Account.AccountID,
                            client.Character.Account.GMLevel, command));
                        //*/

                        if (args.Length == 2)
                        {
                            cInfo.func(client, args[1]);
                        }
                        else
                        {
                            cInfo.func(client, "");
                        }
                    }
                    else
                    {
                        client.SendServerMessage("You don't have access to this command!", SM_SERVER_MESSAGE.Positions.ChatWindow);
                    }

                    return true;
                }
            }
            catch (Exception e) { Logger.Log.Error(e); }

            return false;
        }
    }
}
