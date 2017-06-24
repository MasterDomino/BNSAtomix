using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Skills.SkillHandlers;
using SagaBNS.GameServer.Tasks.Actor;
using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using System;

namespace SagaBNS.GameServer.Skills
{
    public partial class SkillManager : Singleton<SkillManager>
    {
        #region Methods

        public unsafe void BroadcastSkillCast(SkillArg arg, SkillMode mode)
        {
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            if (map == null)
            {
                return;
            }

            UpdateEvent evt;
            if (mode == SkillMode.End)
            {
                if (arg.AffectedActors.Count == 0 && (arg.Skill.BaseData.SkillType != SkillType.Single))
                {
                    evt = new UpdateEvent()
                    {
                        Actor = arg.Caster,
                        UpdateType = UpdateTypes.Effect,
                        Target = arg.Target,
                        SkillSession = arg.SkillSession,
                        Skill = arg.Skill,
                        SkillAttackResult = SkillAttackResult.Normal
                    };
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
                }
                foreach (SkillAffectedActor i in arg.AffectedActors)
                {
                    if (i.BonusAdditionID == 0)
                    {
                        evt = new UpdateEvent()
                        {
                            Actor = arg.Caster,
                            UpdateType = UpdateTypes.Effect,
                            Target = i.Target,
                            SkillSession = arg.SkillSession,
                            Skill = arg.Skill,
                            SkillAttackResult = i.Result
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
                    }
                    if (i.Damage > 0 || i.Target.HP == 0)
                    {
                        evt = new UpdateEvent()
                        {
                            Actor = arg.Caster,
                            Skill = arg.Skill,
                            AdditionID = i.BonusAdditionID,
                            SkillSession = arg.SkillSession,
                            Target = i.Target,
                            UpdateType = UpdateTypes.Actor
                        };
                        int damage = i.Damage;
                        if (!i.NoDamageBroadcast)
                        {
                            byte[] buf = new byte[6];
                            fixed (byte* res = buf)
                            {
                                res[0] = 2;
                                *(int*)&res[1] = damage;
                                res[5] = 0;
                            }
                            evt.UserData = buf;
                        }
                        evt.AddActorPara(Common.Packets.GameServer.PacketParameter.HP, i.Target.HP);
                        if (i.Target.HP <= 0 && !(i.Target.Status.Dead && !i.Target.Status.Dying))
                        {
                            i.Target.Status.Dead = true;

                            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dead, i.Target.ActorType == ActorType.PC ? (i.Target.Status.Dying ? 1 : 2) : 1);
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
                            ((BNSActorEventHandler)i.Target.EventHandler).OnDie(arg.Caster);
                            if (arg.Caster is ActorPC pc)
                            {
                                if (pc.Tasks.TryGetValue("CombatStatusTask", out Task task))
                                {
                                    task.DueTime = 10000;
                                    task.Activate();
                                }
                            }
                        }
                        else
                        {
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
                        }
                    }
                }
                if (arg.ActivationIndex >= arg.Skill.BaseData.ActivationTimes.Count)
                {
                    HandleAfterSkillActivate(arg);
                }
            }

            evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                SkillSession = arg.SkillSession,
                UpdateType = UpdateTypes.Skill,
                Target = arg.Target,
                Skill = arg.Skill,
                SkillCastMode = arg.CastMode,
                X = arg.X,
                Y = arg.Y,
                Z = arg.Z
            };
            evt.SkillSession = arg.SkillSession;
            evt.UserData = arg.ActivationIndex;
            if (mode == SkillMode.CastActionDelay)
            {
                evt.UserData = arg.ApproachTime;
            }

            if (evt.SkillCastMode != SkillCastMode.Coordinate)
            {
                switch (arg.Skill.BaseData.SkillType)
                {
                    case SkillType.Single:
                    case SkillType.Self:
                    case SkillType.Direction:
                    case SkillType.NoTarget:
                        evt.SkillCastMode = SkillCastMode.Single;
                        break;
                }
            }
            evt.SkillMode = mode;
            if (mode != SkillMode.End || (arg.ActivationIndex >= arg.Skill.BaseData.ActivationTimes.Count))
            {
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
            }

            if (mode == SkillMode.Activate && arg.Skill.BaseData.Duration > 0)
            {
                evt = new UpdateEvent()
                {
                    Actor = arg.Caster,
                    UpdateType = UpdateTypes.Effect,
                    Target = arg.Target,
                    SkillSession = arg.SkillSession,
                    Skill = arg.Skill,
                    SkillAttackResult = SkillAttackResult.Normal
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
            }
            if ((mode == SkillMode.End || mode == SkillMode.DurationEnd) && arg.ActivationIndex >= arg.Skill.BaseData.ActivationTimes.Count)
            {
                arg.Caster.Status.CastingSkill = false;
            }
        }

        public bool CheckSkillCast(ActorExt sActor, ActorExt dActor, ushort dir, Skill skill)
        {
            DateTime now = DateTime.Now;
            if (now < sActor.Status.SkillCooldownEnd || now < skill.CoolDownEndTime)
            {
                return false;
            }

            if (skill.BaseData.PreviousSkills.Count > 0 && !skill.BaseData.PreviousSkills.Contains(sActor.Status.LastSkillID))
            {
                return false;
            }
            switch (skill.BaseData.RequiredCasterStance)
            {
                case SkillCastStances.None:
                    if (sActor.Status.Dead || sActor.Status.Down || sActor.Status.TakeDown)
                    {
                        return false;
                    }

                    break;

                case SkillCastStances.TakeDown:
                    if (!sActor.Status.TakeDown)
                    {
                        return false;
                    }

                    break;

                default:
                    return false;
            }
            if (dActor != null)
            {
                if (NPC.FactionRelationFactory.Instance[sActor.Faction][dActor.Faction] == Relations.Friendly && sActor != dActor)
                {
                    return false;
                }

                switch (skill.BaseData.RequiredTargetStance)
                {
                    case SkillCastStances.Down:
                        if (!dActor.Status.Down || sActor.Status.TakeDown)
                        {
                            return false;
                        }

                        break;

                    case SkillCastStances.NoMove:
                        if (!dActor.Status.Down && !dActor.Status.Frosen && !dActor.Status.Stun)
                        {
                            return false;
                        }

                        break;
                }
            }
            switch (skill.BaseData.SkillType)
            {
                case SkillType.Single:
                    {
                        int dist = sActor.DistanceToActor(dActor);
                        if (dist >= skill.BaseData.CastRangeMin && dist <= skill.BaseData.CastRangeMax)
                        {
                            return CheckMana(sActor, skill);
                        }
                        else
                        {
                            return false;
                        }
                    }
                case SkillType.Self:
                    if (sActor == dActor)
                    {
                        return CheckMana(sActor, skill);
                    }
                    else
                    {
                        return true;
                    }

                case SkillType.Direction:
                case SkillType.NoTarget:
                    return CheckMana(sActor, skill);

                default:
                    return false;
            }
        }

        public void PlayerAddSkill(ActorPC pc, uint skillID, bool sendPacket)
        {
            if (SkillFactory.Instance.Items.ContainsKey(skillID))
            {
                Skill skill = new Skill(SkillFactory.Instance[skillID]);
                pc.Skills[skillID] = skill;
                foreach (uint i in skill.BaseData.RelatedSkills)
                {
                    if (i == skillID || pc.Skills.ContainsKey(i))
                    {
                        continue;
                    }

                    if (SkillFactory.Instance.Items.ContainsKey(i))
                    {
                        Skill dummy = new Skill(SkillFactory.Instance[i])
                        {
                            Dummy = true
                        };
                        pc.Skills[i] = dummy;
                    }
                }
                if (sendPacket)
                {
                    pc.Client().SendSkillAdd(skillID);
                }
            }
            else
            {
                Logger.Log.Debug(string.Format("SkillID:{0} not found!", skillID));
            }
        }

        public void SkillActivate(SkillArg arg)
        {
            ActorPC pc = arg.Caster as ActorPC;
            arg.Caster.Status.CastingSkill = false;
            if (pc != null && arg.Skill.BaseData.MovementLockOnAction > 0)
            {
                SkillHandlers.Common.Additions.MovementLock mLock = new SkillHandlers.Common.Additions.MovementLock(pc.Client(), arg.Skill.BaseData.MovementLockOnAction);
                mLock.Activate();
            }
            if (arg.Caster.Status.Dead)
            {
                return;
            }

            arg.Caster.Status.LastSkillID = arg.Skill.ID;
            if (!arg.Caster.Status.Dead && !arg.Caster.Status.Down)
            {
                HandleSkillCastFinish(arg);
                BroadcastSkillCast(arg, SkillMode.Activate);
                HandleSkillActivate(arg);
            }

            if (pc != null)
            {
                int duration = arg.AffectedActors.Count > 0 ? 30000 : 5000;
                if (pc.Tasks.TryGetValue("CombatStatusTask", out Task task))
                {
                    if (task.DueTime < duration)
                    {
                        task.DueTime = duration;
                    }
                    task.Activate();
                }
                else
                {
                    Tasks.Player.CombatStatusTask ct = new Tasks.Player.CombatStatusTask(duration, pc);
                    pc.Tasks["CombatStatusTask"] = ct;
                    ct.Activate();
                }
            }
            arg.ActivationIndex++;
            if (arg.Skill.BaseData.Duration <= 0)
            {
                BroadcastSkillCast(arg, SkillMode.End);
            }

            if (arg.ActivationIndex < arg.Skill.BaseData.ActivationTimes.Count)
            {
                arg.Caster.Status.CastingSkill = true;
                SkillCastTask task = new SkillCastTask(arg.Skill.BaseData.ActivationTimes[arg.ActivationIndex - 1], arg.Caster, arg);
                arg.Caster.Tasks["SkillCast"] = task;
                task.Activate();
            }
            else
            {
                if (arg.Skill.BaseData.CoolDown != 0)
                {
                    arg.Skill.CoolDownEndTime = DateTime.Now.AddMilliseconds(arg.Skill.BaseData.CoolDown);
                }

                //if (arg.Skill.BaseData.ActivationTimes.Count < arg.ActivationIndex)
                //{
                //    Logger.Log.Debug($"ActivationTimes for skill:{arg.Skill.ID} is smaller than index:{arg.ActivationIndex}");
                //}
                if (arg.Caster.ActorType == ActorType.NPC)
                {
                    arg.Caster.Status.SkillCooldownEnd = DateTime.Now.AddMilliseconds(arg.Skill.BaseData.ActivationTimes.Count > 0 && arg.Skill.BaseData.ActivationTimes.Count > (arg.ActivationIndex - 1) ? arg.Skill.BaseData.ActivationTimes[arg.ActivationIndex - 1] : 500);
                }
                else
                {
                    arg.Caster.Status.SkillCooldownEnd = DateTime.Now.AddMilliseconds(100);//TODO: Use real data
                }
            }
        }

        public bool SkillCast(SkillArg arg)
        {
            ActorExt sActor = arg.Caster;
            ActorExt dActor = arg.Target;
            Skill skill = arg.Skill;
            ushort dir = arg.Dir;
            DateTime now = DateTime.Now;
            if (skill.BaseData.Effect != 0)
            {
                skill.BaseData.Duration = (int)Effect.EffectManager.Instance[skill.BaseData.Effect].Duration;
            }

            if (!sActor.Status.CastingSkill || arg.CastFinished)
            {
                if (arg.CastFinished || CheckSkillCast(sActor, dActor, dir, skill))
                {
                    if (arg.Skill.BaseData.ManaCost > 0 && !arg.CastFinished && arg.Caster.ActorType == SmartEngine.Network.Map.ActorType.PC)
                    {
                        System.Threading.Interlocked.Add(ref arg.Caster.MP, -arg.Skill.BaseData.ManaCost);
                        ((ActorPC)arg.Caster).Client().SendPlayerMP();
                    }
                    if (arg.Caster.Tasks.TryGetValue("SwordBlocking", out Task duration))
                    {
                        duration.Deactivate();
                    }

                    if (arg.Caster.Tasks.TryGetValue("Teleport", out duration))
                    {
                        duration.Deactivate();
                    }

                    if (arg.Caster.Tasks.TryGetValue("FoodRecovery", out duration))
                    {
                        duration.Deactivate();
                    }
                    if (arg.Caster.Tasks.TryGetValue("Stealth", out duration))
                    {
                        duration.Deactivate();
                    }

                    if (skill.BaseData.CastTime == 0 || (skill.BaseData.ShouldApproach && arg.CastFinished) || (skill.BaseData.ActionTime != 0 && skill.BaseData.CastTime == 0) || arg.CastFinished)
                    {
                        sActor.Status.CastingSkill = true;
                        if (skill.BaseData.ShouldApproach || skill.BaseData.ActionTime != 0)
                        {
                            int castTime = 0;
                            if (skill.BaseData.ShouldApproach)
                            {
                                castTime = 500;
                                if (arg.Target != null && arg.Target != arg.Caster)
                                {
                                    castTime = arg.Caster.DistanceToActor(arg.Target) * arg.Skill.BaseData.ApproachTimeRate;
                                }
                                arg.ApproachTime = castTime;
                                BroadcastSkillCast(arg, SkillMode.CastActionDelay);
                            }
                            else if (skill.BaseData.ActionTime > 0)
                            {
                                castTime = skill.BaseData.ActionTime;
                                arg.ApproachTime = castTime;
                                BroadcastSkillCast(arg, SkillMode.CastActionDelay);
                            }
                            if ((arg.Caster is ActorPC pc) && arg.Skill.BaseData.MovementLockOnCasting > 0)
                            {
                                SkillHandlers.Common.Additions.MovementLock mLock = new SkillHandlers.Common.Additions.MovementLock(pc.Client(), arg.Skill.BaseData.MovementLockOnCasting);
                                mLock.Activate();
                            }

                            HandleSkillCasting(arg);

                            SkillCastTask task = new SkillCastTask(castTime, sActor, arg);
                            sActor.Tasks["SkillCast"] = task;
                            task.Activate();
                        }
                        else
                        {
                            SkillActivate(arg);
                        }
                    }
                    else
                    {
                        sActor.Status.CastingSkill = true;
                        int castTime = 0;

                        BroadcastSkillCast(arg, SkillMode.Cast);
                        castTime = skill.BaseData.CastTime;
                        if ((arg.Caster is ActorPC pc) && arg.Skill.BaseData.MovementLockOnCasting > 0)
                        {
                            SkillHandlers.Common.Additions.MovementLock mLock = new SkillHandlers.Common.Additions.MovementLock(pc.Client(), arg.Skill.BaseData.MovementLockOnCasting);
                            mLock.Activate();
                        }

                        HandleSkillCasting(arg);

                        SkillCastTask task = new SkillCastTask(castTime, sActor, arg);
                        sActor.Tasks["SkillCast"] = task;
                        task.Activate();
                    }
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private bool CheckMana(ActorExt sActor, Skill skill)
        {
            if (sActor is ActorPC pc)
            {
                if (pc.MP >= skill.BaseData.ManaCost)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void HandleAfterSkillActivate(SkillArg arg)
        {
            if (!skillHandlers.TryGetValue(arg.Skill.ID, out ISkillHandler handler))
            {
                handler = skillHandlers[uint.MaxValue];
            }

            handler.OnAfterSkillCast(arg);
        }

        private void HandleSkillActivate(SkillArg arg)
        {
            if (!skillHandlers.TryGetValue(arg.Skill.ID, out ISkillHandler handler))
            {
                handler = skillHandlers[uint.MaxValue];
            }

            handler.HandleSkillActivate(arg);
        }

        private void HandleSkillCastFinish(SkillArg arg)
        {
            if (!skillHandlers.TryGetValue(arg.Skill.ID, out ISkillHandler handler))
            {
                handler = skillHandlers[uint.MaxValue];
            }

            handler.HandleOnSkillCastFinish(arg);
        }

        private void HandleSkillCasting(SkillArg arg)
        {
            if (!skillHandlers.TryGetValue(arg.Skill.ID, out ISkillHandler handler))
            {
                handler = skillHandlers[uint.MaxValue];
            }

            handler.HandleOnSkillCasting(arg);
        }

        #endregion
    }
}