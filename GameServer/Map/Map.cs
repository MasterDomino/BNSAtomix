using System;
using System.Collections.Generic;

using SmartEngine.Core;
using SmartEngine.Network.Map;
using SmartEngine.Network.Map.PathFinding;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Map
{
    public enum MapInstanceType
    {
        NoInstance,
        Single,
        Party,
    }

    public class Map : Map<MapEvents>
    {
        private readonly Dictionary<ulong, ActorMapObj> mapObjects = new Dictionary<ulong, ActorMapObj>();
        private readonly Dictionary<ulong, ActorMapObj> campfires = new Dictionary<ulong, ActorMapObj>();
        public static Dictionary<string, HeightMapBuilder> heightmapBuilder = new Dictionary<string, HeightMapBuilder>();
        private HeightMapBuilder builder;
        private readonly PathFinding pathFinding = new PathFinding();
        public HeightMapBuilder HeightMapBuilder
        {
            get { return builder; }
            set
            {
                builder = value;
                pathFinding.GeoData = builder;
            }
        }

        public List<RespawnPoint> RespawnPoints
        {
            get
            {
                if (MapManager.Instance.RespawnPoints.ContainsKey(id))
                {
                    return MapManager.Instance.RespawnPoints[id];
                }
                else
                {
                    return new List<RespawnPoint>();
                }
            }
        }

        public PathFinding PathFinding { get { return pathFinding; } }
        public Dictionary<ulong, ActorMapObj> MapObjects { get { return mapObjects; } }
        public Dictionary<ulong, ActorMapObj> Campfires { get { return mapObjects; } }
        public MapInstanceType InstanceType { get; set; }
        public uint CreatorCharID { get; set; }
        private int playerCount;
        private MapInstanceDestoryTask autoDestoryTask;
        public Map()
            : this(0)
        {
        }

        public Map(uint id)
            : base(id)
        {
            InstanceID = 0x1C;
        }

        public override bool OnRegister(Actor actor)
        {
            if (actor.ActorType == ActorType.PC)
            {
                playerCount++;
                if (autoDestoryTask != null)
                {
                    autoDestoryTask.Deactivate();
                    autoDestoryTask = null;
                }
            }
            return true;
        }

        public override void OnDeleteActor(Actor actor)
        {
            builder.ClearArea((short)actor.X, (short)actor.Y, (short)actor.Z);
            if (actor.ActorType == ActorType.PC)
            {
                playerCount--;
                if (playerCount == 0)
                {
                    if (InstanceType != MapInstanceType.NoInstance)
                    {
                        bool hasNPC = false;
                        foreach (KeyValuePair<ulong, Actor> i in actorsByID)
                        {
                            if (i.Value.ActorType == ActorType.NPC)
                            {
                                hasNPC = true;
                                break;
                            }
                        }
                        if (!hasNPC)
                        {
                            Destroy();
                        }
                        else if (autoDestoryTask == null)
                        {
                            autoDestoryTask = new MapInstanceDestoryTask(this);
                            autoDestoryTask.Activate();
                        }
                    }
                }
            }
        }

        public override void OnMoveActor(Actor mActor, MoveArg arg, bool knockBack)
        {
            builder.ActorMove(mActor.ActorID, (short)mActor.X, (short)mActor.Y, (short)mActor.Z, (short)arg.X, (short)arg.Y, (short)arg.Z);
            if (mActor.ActorType == ActorType.PC)
            {
                if (((ActorPC)mActor).HoldingItem != null)
                {
                    MoveActor(((ActorPC)mActor).HoldingItem, arg);
                }
                if (arg is MoveArgument a)
                {
                    if (a.BNSMoveType == MoveType.Run || a.BNSMoveType == MoveType.Dash)
                    {
                        HeightMapBuilder.Collect((short)a.X, (short)a.Y, (short)a.Z);
                    }
                }
            }
        }

        public override void OnTeleportActor(Actor sActor, float x, float y, float z)
        {

        }

        public override void OnEvent(MapEvents etype, MapEventArgs args, Actor sActor, Actor dActor)
        {
            switch (etype)
            {
                case MapEvents.APPEAR:
                    {
                        sActor.EventHandler.OnActorAppears(dActor);
                    }
                    break;
                case MapEvents.DISAPPEAR:
                    {
                        sActor.EventHandler.OnActorDisappears(dActor);
                    }
                    break;
                case MapEvents.CHAT:
                    if (sActor.ActorType == ActorType.PC)
                    {
                        ((ActorEventHandlers.PCEventHandler)sActor.EventHandler).OnChat((ChatArgument)args);
                    }
                    break;
                case MapEvents.PORTAL_ENTER:
                    ((BNSActorEventHandler)sActor.EventHandler).OnActorEnterPortal(dActor);
                    break;
                case MapEvents.QUEST_UPDATE:
                    {
                        Scripting.NPCScriptHandler npc = sActor.EventHandler as Scripting.NPCScriptHandler;
                        QuestArgument arg = args as QuestArgument;
                        if (npc != null && arg != null && arg.OriginNPC != npc.NpcID)
                        {
                            npc.OnQuest(arg.Player, arg.Quest.QuestID, arg.Step, arg.Quest);
                        }
                    }
                    break;
                default:
                    if (sActor.ActorType == ActorType.PC)
                    {
                        ((ActorEventHandlers.PCEventHandler)sActor.EventHandler).OnBroadcastEvt((UpdateEvent)args);
                    }
                    break;
            }
        }

        public void Destroy()
        {
            foreach (ActorExt i in actorsByID.Values)
            {
                try
                {
                    if (i is ActorNPC npc)
                    {
                        foreach (SmartEngine.Network.Tasks.Task j in npc.Tasks.Values)
                        {
                            j.Deactivate();
                        }

                        npc.Tasks.Clear();
                        ((ActorEventHandlers.NPCEventHandler)npc.EventHandler).AI.Deactivate();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
        }

        protected override Map<MapEvents> GetMapOfActor(Actor actor)
        {
            return MapManager.Instance.GetMap(actor.MapInstanceID);
        }
    }
}
