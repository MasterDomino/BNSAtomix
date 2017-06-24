using System;
using System.Collections.Generic;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.NPC.AI;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.ActorEventHandlers
{
    public class NPCEventHandler : BNSActorEventHandler
    {
        private ActorNPC npc;
        public delegate void RespawnArg();
        public event RespawnArg ShouldRespawn;
        public AI AI { get; set; }
        public ActorNPC NPC
        {
            get { return npc; }
            set
            {
                if (npc != value)
                {
                    npc = value;
                    AI?.Deactivate();

                    AI = new AI(npc);
                }
            }
        }

        private DateTime attackStamp = DateTime.Now;
        private ActorPC firstAttacker;
        public NPCEventHandler()
        {
        }

        public NPCEventHandler(ActorNPC npc)
        {
            this.npc = npc;
            AI = new AI(npc);
        }

        #region ActorEventHandler 成员

        public override void OnCreate(bool success)
        {

        }

        public override void OnDelete()
        {
            AI.Deactivate();
            npc.VisibleActors.Clear();
        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {

        }

        public override void OnActorStopsMoving(Actor mActor, MoveArg arg)
        {

        }

        public override void OnActorAppears(Actor aActor)
        {
            npc.VisibleActors[aActor.ActorID] = aActor.ActorID;
            if (aActor.ActorType == ActorType.PC)
            {
                if (!AI.Activated)
                {
                    AI.Activate();
                }
            }
        }

        public override void OnActorDisappears(Actor dActor)
        {
            npc.VisibleActors.TryRemove(dActor.ActorID, out ulong removed);
            AI.HateTable.TryRemove(dActor.ActorID, out int val);
        }

        public override void OnTeleport(float x, float y, float z)
        {

        }

        public override void OnGotVisibleActors(List<Actor> actors)
        {
            foreach (Actor aActor in actors)
            {
                npc.VisibleActors[aActor.ActorID] = aActor.ActorID;
                if (aActor.ActorType == ActorType.PC)
                {
                    if (!AI.Activated)
                    {
                        AI.Activate();
                    }
                }
            }
        }

        public override void OnDie(ActorExt killedBy)
        {
            if (npc.Tasks.TryGetValue("ActorCatch", out Task removed))
            {
                removed.Deactivate();
            }

            if (firstAttacker != null && npc.BaseData.QuestIDs.Count > 0)
            {
                ActorPC pc = firstAttacker;
                List<ushort> already = new List<ushort>();
                foreach (ushort i in npc.BaseData.QuestIDs)
                {
                    if (!already.Contains(i))
                    {
                        already.Add(i);
                    }
                    else
                    {
                        continue;
                    }

                    if (pc.Quests.TryGetValue(i, out Common.Quests.Quest q))
                    {
                        try
                        {
                            Quests.QuestManager.Instance.ProcessQuest(pc, i, q.NextStep, q, npc, false, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Error(ex);
                        }
                    }
                }
            }
            if (ShouldRespawn != null)
            {
                ShouldRespawn();
            }

            AI.Deactivate();
            Map.Map map = MapManager.Instance.GetMap(npc.MapInstanceID);
            ActorCorpse corpse = new ActorCorpse(npc);
            if (firstAttacker != null)
            {
                corpse.Owner = Party.PartyManager.Instance.GetPartyLootOwner(firstAttacker);
            }

            corpse.MapID = npc.MapID;
            corpse.X = npc.X;
            corpse.Y = npc.Y;
            corpse.Z = npc.Z;
            corpse.EventHandler = new DummyEventHandler();

            if (firstAttacker != null)
            {
                uint exp = (uint)(npc.Level / 2 + 10);
                Manager.ExperienceManager.Instance.ApplyExp(firstAttacker, exp);
                List<Common.Item.Item> items = new List<Common.Item.Item>();
                foreach (uint i in npc.BaseData.Items.Keys)
                {
                    int rate = npc.BaseData.Items[i];
                    if (Global.Random.Next(0, 99) < rate)
                    {
                        Common.Item.Item item = Item.ItemFactory.Instance.CreateNewItem(i);
                        item.Count = npc.BaseData.ItemCounts[i];
                        items.Add(item);
                    }
                }
                //corpse.Gold = 200;
                corpse.Items = items.ToArray();
            }
            map.RegisterActor(corpse);
            npc.Status.CorpseActorID = corpse.ActorID;

            Tasks.Actor.CorpseDeleteTask task = new Tasks.Actor.CorpseDeleteTask(npc.BaseData.CorpseItemID > 0 ? 20000 : 10000, corpse);
            task.Activate();

            foreach (NPCDeathSpawn i in npc.BaseData.DeathSpawns)
            {
                for (int j = 0; j < i.Count; j++)
                {
                    short x = (short)Global.Random.Next(npc.X - 15, npc.X + 15);
                    short y = (short)Global.Random.Next(npc.Y - 15, npc.Y + 15);
                    short z = (short)npc.Z;
                    Scripting.Utils.SpawnNPC(map, i.NPCID, i.AppearEffect, x, y, z, 0, i.Motion);
                }
            }
            firstAttacker = null;
        }

        public override void OnActorEnterPortal(Actor aActor)
        {

        }

        public override void OnSkillDamage(SkillArg arg, SkillAttackResult result, int dmg, int bonusCount)
        {
            if (arg.Caster.ActorType == ActorType.PC && ((DateTime.Now - attackStamp).TotalMinutes > 5 || firstAttacker == null || (firstAttacker?.DistanceToActor(npc) > 200)))
            {
                firstAttacker = arg.Caster as ActorPC;
                attackStamp = DateTime.Now;
            }
            AI.OnDamage(arg, dmg);
        }
        #endregion
    }
}
