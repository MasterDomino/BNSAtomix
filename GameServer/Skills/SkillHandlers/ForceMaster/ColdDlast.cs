using System.Collections.Generic;
using System.Threading;

using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Skills.SkillHandlers.Common;

namespace SagaBNS.GameServer.Skills.SkillHandlers.ForceMaster
{
    public class ColdDlast : DefaultAttack
    {
        private readonly uint additionID;
        public ColdDlast(uint additionID)
        {
            this.additionID = additionID;
        }
        #region ISkillHandler 成员

        public override void HandleSkillActivate(SkillArg arg)
        {
            SkillManager.Instance.DoAttack(arg);
            List<SkillAffectedActor> affected = arg.AffectedActors;
            bool noTarget = false;
            foreach (SkillAffectedActor i in affected)
            {
                SkillAttackResult res = i.Result;
                if (res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss && res != SkillAttackResult.Parry && res != SkillAttackResult.TotalParry)
                {
                    if (i.Target.Tasks.ContainsKey("ActorFrosen"))
                    {
                        Buff buff = i.Target.Tasks["ActorFrosen"] as Buff;
                        buff.Deactivate();
                    }

                    i.NoDamageBroadcast = true;
                    Common.Additions.ActorFrosen add = new Common.Additions.ActorFrosen(arg, i.Target, additionID, i.Damage, 5000);

                    i.Target.Tasks["ActorFrosen"] = add;
                    add.Activate();
                    noTarget = true;
                }
            }
            Additions.FrosenSelf frosenSelf = new Additions.FrosenSelf(arg);
            frosenSelf.Activate();

            arg.Caster.ChangeStance(Stances.Ice, arg.SkillSession, 12000002, 6);
            if (noTarget && arg.Caster.ActorType == ActorType.PC)
            {
                Interlocked.Add(ref arg.Caster.MP, 2);
                if (arg.Caster.MP > arg.Caster.MaxMP)
                {
                    Interlocked.Exchange(ref arg.Caster.MP, arg.Caster.MaxMP);
                }

                Network.Client.GameSession client = ((ActorPC)arg.Caster).Client();
                client?.SendPlayerMP();
            }
        }

        #endregion
    }
}
