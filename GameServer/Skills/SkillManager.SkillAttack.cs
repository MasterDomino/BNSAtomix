using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
using SmartEngine.Core.Math;
using SmartEngine.Network;
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SagaBNS.GameServer.Skills
{
    public partial class SkillManager : Singleton<SkillManager>
    {
        #region Methods

        public void DoAttack(SkillArg arg)
        {
            List<SkillAffectedActor> affected = arg.AffectedActors;
            List<ActorExt> targets = new List<ActorExt>();

            #region Target Selection

            if (!arg.Caster.Status.Dead && !arg.Caster.Status.Down && !arg.Caster.Status.Invincible)
            {
                if (arg.CastMode == SkillCastMode.Coordinate)
                {
                    Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
                    foreach (Actor i in map.GetActorsAroundArea(arg.X, arg.Y, arg.Z, arg.Skill.BaseData.NoTargetRange > 0 ? arg.Skill.BaseData.NoTargetRange : arg.Skill.BaseData.CastRangeMax))
                    {
                        if (NPC.FactionRelationFactory.Instance[arg.Caster.Faction][((ActorExt)i).Faction] != Relations.Friendly)
                        {
                            targets.Add((ActorExt)i);
                        }
                    }
                }
                else
                {
                    switch (arg.Skill.BaseData.SkillType)
                    {
                        case SkillType.Single:
                            if (!arg.Target.Status.Dead || arg.Target.Status.Recovering)
                            {
                                int dir = arg.Dir.DirectionRelativeToTarget(arg.Caster.X, arg.Caster.Y, arg.Target.X, arg.Target.Y);
                                if (dir <= arg.Skill.BaseData.NoTargetAngle)
                                {
                                    targets.Add(arg.Target);
                                }
                            }
                            break;

                        case SkillType.NoTarget:
                            {
                                Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
                                foreach (Actor i in map.GetActorsAroundActor(arg.Caster, arg.Skill.BaseData.NoTargetRange > 0 ? arg.Skill.BaseData.NoTargetRange : arg.Skill.BaseData.CastRangeMax, false))
                                {
                                    if (i.ActorType == ActorType.CORPSE || i.ActorType == ActorType.ITEM)
                                    {
                                        continue;
                                    }

                                    if (arg.Skill.BaseData.NoTargetType == NoTargetTypes.Angular)
                                    {
                                        if (arg.Dir.DirectionRelativeToTarget(arg.Caster.X, arg.Caster.Y, i.X, i.Y) <= arg.Skill.BaseData.NoTargetAngle && NPC.FactionRelationFactory.Instance[arg.Caster.Faction][((ActorExt)i).Faction] != Relations.Friendly)
                                        {
                                            targets.Add((ActorExt)i);
                                        }
                                    }
                                    else
                                    {
                                        float degree = arg.Caster.DirectionRelativeToTarget(i);
                                        float distance = arg.Caster.DistanceToActor(i);
                                        Degree deg = degree;
                                        float width = (float)(distance * Math.Sin(deg.InRadians()));
                                        float length = (float)(distance * Math.Cos(deg.InRadians()));
                                        if (width <= arg.Skill.BaseData.NoTargetWidth && NPC.FactionRelationFactory.Instance[arg.Caster.Faction][((ActorExt)i).Faction] != Relations.Friendly && degree < 90)
                                        {
                                            targets.Add((ActorExt)i);
                                        }
                                    }
                                }
                            }
                            break;

                        case SkillType.Self:

                            //Logger.Log.Debug("Do attack on self casting, really wanted?");
                            targets.Add(arg.Caster);
                            break;
                    }
                }
            }

            #endregion

            #region Attack Action

            foreach (ActorExt i in targets)
            {
                SkillAffectedActor res = new SkillAffectedActor();
                SkillAffectedActor bonus = null;
                res.Result = CalcAttackResult(arg.Caster, i, arg.CastMode == SkillCastMode.Coordinate || arg.Skill.BaseData.SkillType == SkillType.NoTarget, arg.Skill.BaseData.CastRangeMax);
                res.Target = i;
                if (res.Result != SkillAttackResult.Avoid && res.Result != SkillAttackResult.Miss)
                {
                    switch (res.Result)
                    {
                        case SkillAttackResult.Counter:
                            {
                                if (i.Tasks.TryGetValue("CounterEnemy", out Task task))
                                {
                                    SkillHandlers.KungfuMaster.Additions.CounterEnemy counter = task as SkillHandlers.KungfuMaster.Additions.CounterEnemy;
                                    counter.Deactivate();
                                    SkillArg arg2 = new SkillArg();
                                    if (i is ActorPC pc && pc.Skills.TryGetValue(counter.CounterSkillID, out Skill skill))
                                    {
                                        if (pc.Job == Job.KungfuMaster)
                                        {
                                            Interlocked.Add(ref pc.MP, 3);
                                            if (pc.MP > pc.MaxMP)
                                            {
                                                Interlocked.Exchange(ref pc.MP, pc.MaxMP);
                                            }

                                            pc.Client().SendPlayerMP();
                                        }
                                        arg2.Skill = skill;
                                        arg2.Caster = i;
                                        arg2.Target = arg.Caster;
                                        arg2.SkillSession = (byte)Global.Random.Next(0, 255);
                                        arg2.Dir = i.Dir;
                                        SkillCast(arg2);
                                    }
                                }
                                if (i.Tasks.TryGetValue("WoodBlock", out task))
                                {
                                    SkillHandlers.Assassin.Additions.WoodBlock counter = task as SkillHandlers.Assassin.Additions.WoodBlock;
                                    counter.Deactivate();
                                    SkillArg arg2 = new SkillArg();
                                    if (i is ActorPC pc && pc.Skills.TryGetValue(counter.CounterSkillID, out Skill skill))
                                    {
                                        arg2.Skill = skill;
                                        arg2.Caster = i;
                                        arg2.Target = arg.Caster;
                                        arg2.SkillSession = (byte)Global.Random.Next(0, 255);
                                        arg2.Dir = i.Dir;
                                        SkillCast(arg2);
                                    }
                                }
                            }
                            break;

                        case SkillAttackResult.TotalParry:

                            #region TotalParry

                            {
                                if (i is ActorPC pc)
                                {
                                    if (pc.Job == Job.BladeMaster && pc.MP < pc.MaxMP)
                                    {
                                        Interlocked.Increment(ref pc.MP);
                                        pc.Client().SendPlayerMP();
                                    }
                                }
                            }

                            #endregion

                            break;

                        default:

                            #region default attacking

                            {
                                int dmg = CalcDamage(arg, i, out int bonusCount, out int bonusDmg, out uint bonusAdditionID);
                                switch (res.Result)
                                {
                                    case SkillAttackResult.Critical:
                                        dmg = (int)(dmg * 1.2f);
                                        bonusDmg = (int)(bonusDmg * 1.2f);
                                        break;

                                    case SkillAttackResult.Parry:
                                        dmg = (int)(dmg * 0.3f);
                                        bonusDmg = (int)(bonusDmg * 0.3f);
                                        break;
                                }
                                res.Damage = dmg;
                                ApplyDamage(arg.Caster, i, dmg);
                                if (bonusDmg > 0)
                                {
                                    bonus = new SkillAffectedActor()
                                    {
                                        Damage = bonusDmg,
                                        Target = i,
                                        BonusAdditionID = bonusAdditionID
                                    };
                                    ApplyDamage(arg.Caster, i, bonusDmg);
                                }
                                if (i.EventHandler is BNSActorEventHandler handler)
                                {
                                    handler.OnSkillDamage(arg, res.Result, dmg, bonusCount);
                                }
                                if (i is ActorPC pc)
                                {
                                    Network.Client.GameSession client = pc.Client();
                                    client?.ChangeCombatStatus(true);
                                    if (pc.Tasks.TryGetValue("CombatStatusTask", out Task task))
                                    {
                                        task.DueTime = 30000;
                                        task.Activate();
                                    }
                                    else
                                    {
                                        Tasks.Player.CombatStatusTask ct = new Tasks.Player.CombatStatusTask(30000, pc);
                                        pc.Tasks["CombatStatusTask"] = ct;
                                        ct.Activate();
                                    }
                                }
                            }

                            #endregion

                            break;
                    }
                }
                affected.Add(res);
                if (bonus != null)
                {
                    affected.Add(bonus);
                }
            }

            #endregion
        }

        private void ApplyDamage(ActorExt sActor, ActorExt dActor, int damage)
        {
            Interlocked.Add(ref dActor.HP, -damage);
            if (dActor.HP < 0)
            {
                Interlocked.Exchange(ref dActor.HP, 0);
            }
        }

        private SkillAttackResult CalcAttackResult(ActorExt sActor, ActorExt dActor, bool ignoreRange, int maxRange)
        {
            if (dActor.Status.Invincible)
            {
                return SkillAttackResult.Miss;
            }

            if (sActor.DistanceToActor(dActor) > maxRange && !ignoreRange)
            {
                return SkillAttackResult.Miss;
            }

            int angDif = Math.Abs(sActor.Dir - dActor.Dir);
            if (dActor.Status.Blocking && angDif >= 90)
            {
                return SkillAttackResult.TotalParry;
            }

            if (dActor.Status.Counter && angDif >= 90)
            {
                return SkillAttackResult.Counter;
            }

            if (dActor.Status.Dummy)
            {
                return SkillAttackResult.Counter;
            }

            if (Global.Random.Next(0, 99) <= 95 || dActor.Status.Down)
            {
                int random = Global.Random.Next(0, 99);
                if (random <= 15)
                {
                    return SkillAttackResult.Critical;
                }
                else if (random <= 18 && !dActor.Status.Down)
                {
                    return SkillAttackResult.Avoid;
                }
                else if (random <= 20 && !dActor.Status.Down)
                {
                    return SkillAttackResult.Parry;
                }
                else
                {
                    return SkillAttackResult.Normal;
                }
            }
            else
            {
                return SkillAttackResult.Miss;
            }
        }

        private int CalcDamage(SkillArg arg, ActorExt target, out int bonusCount, out int bonusDamage, out uint bonusAdditionID)
        {
            int dmgMin = (int)(arg.Caster.Status.AtkMin * arg.Skill.BaseData.MinAtk);
            int dmgMax = (int)(arg.Caster.Status.AtkMax * arg.Skill.BaseData.MaxAtk);
            int dmg = Global.Random.Next(dmgMin, dmgMax);
            float rate = arg.Skill.BaseData.ActivationTimes.Count > 0 ? 1f / arg.Skill.BaseData.ActivationTimes.Count : 1f;
            dmg = (int)(dmg * rate);
            bonusCount = 0;
            bonusDamage = 0;
            bonusAdditionID = 0;
            if (!string.IsNullOrEmpty(arg.Skill.BaseData.BonusAddition))
            {
                if (target.Tasks.TryGetValue(arg.Skill.BaseData.BonusAddition, out Task task))
                {
                    if (task is SkillHandlers.Common.Additions.BonusAddition addition)
                    {
                        bonusCount = addition.AccumulateCount;
                        bonusDamage = (int)(dmg * (arg.Skill.BaseData.BonusRate * bonusCount));
                        bonusAdditionID = addition.BonusAdditionID;
                    }
                    task.Deactivate();
                }
            }
            return dmg;
        }

        #endregion
    }
}