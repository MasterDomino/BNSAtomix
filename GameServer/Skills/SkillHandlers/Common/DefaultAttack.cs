using System;
using System.Collections.Generic;
using System.Threading;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class DefaultAttack : ISkillHandler
    {
        private readonly bool addMana;
        private readonly int canPushBack;
        private readonly int stepForward = 10;
        public DefaultAttack()
        {
            addMana = false;
        }

        public DefaultAttack(bool addMana)
        {
            this.addMana = addMana;
        }

        public DefaultAttack(bool addMana, bool canPushBack)
            : this(addMana)
        {
            if (canPushBack)
            {
                this.canPushBack = 10;
            }
        }

        public DefaultAttack(bool addMana, int canPushBack,int stepForward = 10)
            : this(addMana)
        {
            this.canPushBack = canPushBack;
            this.stepForward = stepForward;
        }

        #region ISkillHandler 成员
        public virtual void HandleOnSkillCasting(SkillArg arg)
        {
        }

        public virtual void HandleOnSkillCastFinish(SkillArg arg)
        {
        }

        public virtual void HandleSkillActivate(SkillArg arg)
        {
            SkillManager.Instance.DoAttack(arg);
            List<SkillAffectedActor> affected = arg.AffectedActors;
            bool pushedBack = false;
            Map.Map map = Map.MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            foreach (SkillAffectedActor i in affected)
            {
                SkillAttackResult res = i.Result;
                if (addMana && res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss)
                {
                    if (arg.Caster.ActorType == SmartEngine.Network.Map.ActorType.PC && arg.Caster.MP < arg.Caster.MaxMP)
                    {
                        Network.Client.GameSession client = ((ActorPC)arg.Caster).Client();
                        if (client != null)
                        {
                            Interlocked.Increment(ref arg.Caster.MP);
                            ((ActorPC)arg.Caster).Client().SendPlayerMP();
                        }
                    }
                }
                pushedBack = true;
                bool noPushBack = false;
                if (i.Target is ActorNPC)
                {
                    noPushBack = ((ActorNPC)i.Target).BaseData.NoPushBack;
                }
                if (canPushBack > 0 && !noPushBack && res != SkillAttackResult.Miss && res != SkillAttackResult.Avoid)
                {
                    Map.MoveArgument argu = new Map.MoveArgument()
                    {
                        BNSMoveType = Map.MoveType.PushBack
                    };
                    bool pushed = map.HeightMapBuilder.GetMaximunPushBackPos(i.Target.X, i.Target.Y, i.Target.Z, arg.Caster.Dir, canPushBack, out argu.X, out argu.Y, out argu.Z);
                    int delta = Math.Abs(argu.Z - i.Target.Z);

                    if (pushed && delta < 50)
                    {
                        argu.PushBackSource = arg.Caster;
                        argu.Dir = i.Target.Dir;
                        argu.SkillSession = arg.SkillSession;
                        //SmartEngine.Core.Logger.Log.Info(argu.Z.ToString());
                        map.MoveActor(i.Target, argu, true);
                        if (stepForward > 0)
                        {
                            argu = new Map.MoveArgument()
                            {
                                BNSMoveType = Map.MoveType.StepForward
                            };
                            pushed = map.HeightMapBuilder.GetMaximunPushBackPos(arg.Caster.X, arg.Caster.Y, arg.Caster.Z, arg.Caster.Dir, stepForward, out argu.X, out argu.Y, out argu.Z);
                            argu.Dir = arg.Caster.Dir;
                            delta = Math.Abs(argu.Z - arg.Caster.Z);
                            //SmartEngine.Core.Logger.Log.Info(argu.Z.ToString());
                            if (pushed && delta < 50)
                            {
                                map.MoveActor(arg.Caster, argu, true);
                            }
                        }
                    }
                }
                break;
            }
            if (canPushBack > 0 && !pushedBack && stepForward > 0)
            {
                Map.MoveArgument argu = new Map.MoveArgument()
                {
                    BNSMoveType = Map.MoveType.StepForward
                };
                bool pushed = map.HeightMapBuilder.GetMaximunPushBackPos(arg.Caster.X, arg.Caster.Y, arg.Caster.Z, arg.Caster.Dir, stepForward, out argu.X, out argu.Y, out argu.Z);
                argu.SkillSession = arg.SkillSession;
                int delta = Math.Abs(argu.Z - arg.Caster.Z);
                //SmartEngine.Core.Logger.Log.Info(argu.Z.ToString());
                if (pushed && delta < 50)
                {
                    argu.Dir = arg.Caster.Dir;
                    map.MoveActor(arg.Caster, argu, true);
                }
            }
        }

        public virtual void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
