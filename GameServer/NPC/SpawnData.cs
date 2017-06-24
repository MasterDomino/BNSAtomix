using System;
using System.Collections.Generic;
using SmartEngine.Network;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.NPC
{
    public class SpawnData
    {
        //List<ActorNPC> spawns = new List<ActorNPC>();
        public uint MapID { get; set; }
        public uint SpecialMapID { get; set; }
        public ushort NpcID { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public ushort Dir { get; set; }
        public int Count { get; set; }
        public int Range { get; set; }
        public int MoveRange { get; set; }
        public int Delay { get; set; }
        public int ManaType { get; set; }
        public ushort Motion { get; set; }
        public int AppearEffect { get; set; }
        public bool IsCampfire { get; set; }
        public bool IsMapObject { get; set; }
        public bool IsQuest { get; set; }
        public bool Hidden { get; set; }
        public bool Special { get; set; }
        public bool DragonStream { get; set; }
        private readonly Dictionary<uint, int> itemIDs = new Dictionary<uint, int>();
        public Dictionary<uint, int> ItemIDs { get { return itemIDs; } }
        public int MinGold { get; set; }
        public int MaxGold { get; set; }
        public SpawnData()
        {
            Count = 1;
            Range = 0;
            Delay = 60000;
        }

        public void DoSpawn(Map.Map map)
        {
            if (IsQuest)
            {
                ActorQuest obj = new ActorQuest()
                {
                    MapID = map.ID,
                    MapInstanceID = map.InstanceID,
                    X = X,
                    Y = Y,
                    Z = Z,
                    EventHandler = new ActorEventHandlers.QuestEventHandler()
                };
                map.RegisterActor(obj);
            }
            else if (IsCampfire)
            {
                ActorMapObj obj = new ActorMapObj(NpcID)
                {
                    ActorID = (ulong)(map.InstanceID | 0xC00000) << 32 | NpcID,
                    MapID = map.ID,
                    X = X,
                    Y = Y,
                    Z = Z,
                    SpecialMapID = SpecialMapID,
                    MapInstanceID = map.InstanceID,
                    EventHandler = new ActorEventHandlers.DummyEventHandler()
                };
                map.Campfires.Add(obj.ActorID, obj);
            }
            else if (!IsMapObject)
            {
                for (int i = 0; i < Count; i++)
                {
                    short x;
                    short y;
                    if (Range > 0)
                    {
                        x = (short)Global.Random.Next(X - Range, X + Range);
                        y = (short)Global.Random.Next(Y - Range, Y + Range);
                    }
                    else
                    {
                        x = X;
                        y = Y;
                    }
                    ActorNPC npc = new ActorNPC(NPCDataFactory.Instance.Items[NpcID])
                    {
                        X = x,
                        Y = y,
                        Z = Z,
                        X_Ori = x,
                        Y_Ori = y,
                        Z_Ori = Z,
                        MoveRange = MoveRange,
                        Dir = Dir,
                        StandartMotion = Motion,
                        AppearEffect = AppearEffect,
                        MapID = MapID
                    };
                    if (ManaType > 0)
                    {
                        npc.ManaType = (Common.Actors.ManaType)ManaType;
                    }

                    if (Scripting.ScriptManager.Instance.NpcScripts.ContainsKey(NpcID))
                    {
                        Scripting.NPCScriptHandler handler = (Scripting.NPCScriptHandler)Activator.CreateInstance(Scripting.ScriptManager.Instance.NpcScripts[NpcID].GetType());
                        handler.NPC = npc;
                        npc.EventHandler = handler;
                    }
                    else
                    {
                        npc.EventHandler = new ActorEventHandlers.NPCEventHandler(npc);
                    }

                    if (Delay > 0)
                    {
                        ((ActorEventHandlers.NPCEventHandler)npc.EventHandler).ShouldRespawn += () =>
                        {
                            Tasks.Actor.RespawnTask task = new Tasks.Actor.RespawnTask(Delay, npc);
                            task.Activate();
                        };
                    }
                    map.RegisterActor(npc);
                    npc.Invisible = Hidden;
                    map.OnActorVisibilityChange(npc);
                    map.SendVisibleActorsToActor(npc);
                    //spawns.Add(npc);
                }
            }
            else
            {
                ActorMapObj obj = new ActorMapObj(NpcID)
                {
                    ActorID = (ulong)(map.InstanceID | 0x200000) << 32 | NpcID,
                    MapID = map.ID
                };
                if (Delay > 0)
                {
                    obj.RespawnTime = Delay;
                }

                obj.Special = Special;
                obj.DragonStream = DragonStream;
                obj.X = X;
                obj.Y = Y;
                obj.Z = Z;
                obj.SpecialMapID = SpecialMapID;
                obj.MapInstanceID = map.InstanceID;
                foreach (uint i in itemIDs.Keys)
                {
                    obj.ItemIDs[i] = itemIDs[i];
                }

                obj.MinGold = MinGold;
                obj.MaxGold = MaxGold;
                obj.EventHandler = new ActorEventHandlers.DummyEventHandler();
                map.MapObjects.Add(obj.ActorID, obj);
            }
        }
    }
}
