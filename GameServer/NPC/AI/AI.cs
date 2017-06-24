using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using SmartEngine.Core;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.NPC.AI.AICommands;
using SmartEngine.Network;
using SmartEngine.Core.Math;
using SmartEngine.Network.Tasks;

namespace SagaBNS.GameServer.NPC.AI
{
    public class AI : Task
    {
        private readonly ActorNPC npc;
        private IAICommand currentCommand;
        private ActorExt currentTarget;
        private readonly ConcurrentDictionary<ulong, int> hateTable = new ConcurrentDictionary<ulong, int>();
        public ConcurrentDictionary<ulong, int> HateTable { get { return hateTable; } }
        private Map.Map map;
        public AIStatus Status { get; set; }
        public bool Pause { get; set; }
        public AI(ActorNPC npc)
            : base(2000, 1000, "NPCAI")
        {
            this.npc = npc;
            IsSlowTask = true;
            Status = AIStatus.Normal;
            SmartEngine.Network.Map.PathFinding.PathFinding.MaxIteration = 50;
        }

        private int sleepCounter;
        public override void CallBack()
        {
            if(map ==null)
            {
                map = Map.MapManager.Instance.GetMap(npc.MapInstanceID);
            }

            if (Pause)
            {
                if (currentCommand?.Type == CommandTypes.Attack)
                {
                    ((Attack)currentCommand).Target = null;
                }
                return;
            }
            try
            {
                if (npc.Status.Dead)
                {
                    Deactivate();
                    return;
                }
                if (npc.Status.Blocking || npc.Status.Down || npc.Status.Frosen || npc.Status.Catch || npc.Status.Invincible || npc.Status.Stun || npc.Status.TakenDown)
                {
                    return;
                }

                if (hateTable.Count == 0 && !npc.Status.Dead && npc.HP < npc.MaxHP)
                {
                    System.Threading.Interlocked.Exchange(ref npc.HP, npc.MaxHP);
                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = npc,
                        Target = npc,
                        UpdateType = UpdateTypes.Actor
                    };
                    evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, npc.HP);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, npc, false);
                }

                switch (Status)
                {
                    case AIStatus.Normal:
                        DoNormal();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public bool NoPlayer
        {
            get
            {
                return sleepCounter > 2;
            }
        }

        public int GetCurrentCastRange(ActorExt target)
        {
            int res = 40;
            List<Skill> possible = GetPossibleSkills(target, false);
            if (possible.Count > 0)
            {
                res = (int)(possible[0].BaseData.CastRangeMax / 1.5f);
            }
            return res;
        }

        public List<Skill> GetPossibleSkills(ActorExt target, bool includingSelf = true)
        {
            var query = from skill in npc.BaseData.Skill.Values
                        where Skills.SkillManager.Instance.CheckSkillCast(npc, target, npc.Dir, skill) && (includingSelf || skill.BaseData.SkillType == SkillType.Single)
                        orderby skill.BaseData.SkillType == SkillType.Single ? skill.BaseData.CastRangeMax : 55 descending
                        select skill;
            return query.ToList();
        }

        private bool HasVisiblePlayer()
        {
            var ids = from actor in npc.VisibleActors
                      where (actor.Key & 0x1000000000000) != 0
                      select actor.Key;
            return ids.FirstOrDefault() > 0;
        }

        protected override void OnDeactivate()
        {
            currentTarget = null;
            if (currentCommand?.Type == CommandTypes.Attack)
            {
                ((Attack)currentCommand).Target = null;
            }

            currentCommand = null;
            hateTable.Clear();
        }

        private void DoNormal()
        {
            currentTarget = GetCurrentTarget();
            if (currentTarget != null && (npc.DistanceToActor(currentTarget) > 500 || (currentTarget.Status.Dead && !currentTarget.Status.Recovering)))
            {
                hateTable.TryRemove(currentTarget.ActorID, out int removed);
                currentTarget = null;
            }
            if (currentCommand == null)
            {
                if (currentTarget == null)
                {
                    period = 1000;
                    if (!HasVisiblePlayer())
                    {
                        sleepCounter++;
                        if (sleepCounter > 100)
                        {
                            Deactivate();
                            sleepCounter = 0;
                        }
                    }
                    else
                    {
                        sleepCounter = 0;
                    }

                    CheckAggro();
                    if (npc.MoveRange > 0 || (npc.DistanceToPoint(npc.X_Ori, npc.Y_Ori, npc.Z_Ori) > npc.MoveRange && npc.MoveRange > 0))
                    {
                        if (Global.Random.Next(0, 99) < 30)
                        {
                            Vec3 vec = Vec3.Zero;
                            int shouldDistance = 75;
                            int dis = npc.DistanceToPoint(npc.X_Ori, npc.Y_Ori, npc.Z_Ori);
                            if (dis > npc.MoveRange)
                            {
                                int deltaX = npc.X_Ori - npc.X;
                                int deltaY = npc.Y_Ori - npc.Y;
                                float distance = (float)(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                                vec.X = deltaX / distance;
                                vec.Y = deltaY / distance;
                                if (dis < shouldDistance)
                                {
                                    shouldDistance = dis;
                                }
                            }
                            else
                            {
                                ushort dir = (ushort)Global.Random.Next(0, 359);
                                vec = dir.DirectionToVector();
                            }
                            if (shouldDistance > 15)
                            {
                                vec *= shouldDistance;
                                vec.X += npc.X;
                                vec.Y += npc.Y;
                                var points = from p in map.HeightMapBuilder.GetZ((short)vec.X, (short)vec.Y)
                                             orderby Math.Abs(npc.Z - p.Key)
                                             select p;
                                currentCommand = new Move(this, npc, (short)vec.X, (short)vec.Y, points.FirstOrDefault().Key);
                            }
                        }
                    }
                }
                else
                {
                    period = 400;
                    if (npc.DistanceToActor(currentTarget) <= GetCurrentCastRange(currentTarget))
                    {
                        currentCommand = new Attack(this, npc, currentTarget);
                    }
                    else
                    {
                        currentCommand = new Chase(this, npc, currentTarget);
                    }
                }
            }
            else
            {
                if (currentTarget == null)
                {
                    CheckAggro();
                }

                currentCommand.Target = currentTarget;
                if (currentCommand.Status == CommandStatus.Running)
                {
                    currentCommand.Update();
                }
                else
                {
                    currentCommand = null;
                }
            }
        }

        private ActorExt GetCurrentTarget()
        {
            ulong[] actors = hateTable.Keys.ToArray();
            Map.Map map = MapManager.Instance.GetMap(npc.MapInstanceID);
            if (map == null)
            {
                Deactivate();
                return null;
            }
            int hate = int.MinValue;
            ActorExt found = null;
            foreach (ulong i in actors)
            {
                int val = hateTable[i];
                if (val > hate)
                {
                    found = map.GetActor(i) as ActorExt;
                    if (found == null || found.Status.Stealth)
                    {
                        found = null;
                        continue;
                    }
                    if (found == null || i == npc.ActorID || FactionRelationFactory.Instance[npc.Faction][found.Faction]== Relations.Friendly)
                    {
                        hateTable.TryRemove(i, out val);
                        continue;
                    }
                    hate = val;
                }
            }
            return found;
        }

        private void CheckAggro()
        {
            try
            {
                Map.Map map = MapManager.Instance.GetMap(npc.MapInstanceID);
                if (map == null)
                {
                    Deactivate();
                    return;
                }
                int distance = int.MaxValue;
                ActorExt found = null;
                foreach (KeyValuePair<ulong,ulong> i in npc.VisibleActors)
                {
                    ulong id = i.Key;
                    if (map.GetActor(id) is ActorExt target)
                    {
                        if (target.Status.Stealth || (target.Status.Dead && !target.Status.Recovering))
                        {
                            continue;
                        }

                        if (FactionRelationFactory.Instance[npc.Faction][target.Faction] == Relations.Enemy)
                        {
                            if (target.Status.Dead && !target.Status.Recovering)
                            {
                                continue;
                            }

                            int len = npc.DistanceToActor(target);
                            if (len < distance)
                            {
                                found = target;
                                distance = len;
                            }
                        }
                    }
                }
                if (found != null && distance < npc.BaseData.AggroRange)
                {
                    if (!hateTable.ContainsKey(found.ActorID))
                    {
                        if (found is ActorPC pc)
                        {
                            pc.Client().SendActorAggro(npc.ActorID);
                        }
                        if (npc.Invisible)
                        {
                            npc.Invisible = false;
                            map.OnActorVisibilityChange(npc);
                        }
                        ActorNPC npc2 = found as ActorNPC;
                        hateTable[found.ActorID] = 20;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public void OnDamage(SkillArg arg, int dmg)
        {
            if (arg.Caster == npc)
            {
                return;
            }

            npc.VisibleActors[arg.Caster.ActorID] = arg.Caster.ActorID;
            ActorNPC npc2 = arg.Caster as ActorNPC;
            if (!hateTable.ContainsKey(arg.Caster.ActorID))
            {
                hateTable[arg.Caster.ActorID] = dmg;
            }
            else
            {
                hateTable[arg.Caster.ActorID] += dmg;
            }

            if (!Activated)
            {
                Activate();
            }
        }
    }
}
