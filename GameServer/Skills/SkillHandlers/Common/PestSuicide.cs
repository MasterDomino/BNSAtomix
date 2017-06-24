using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class PestSuicide : DefaultAttack
    {
        public PestSuicide()
        {
        }

        public override void HandleSkillActivate(SkillArg arg)
        {
            base.HandleSkillActivate(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                if (i.Result != SkillAttackResult.Miss && i.Result != SkillAttackResult.Avoid)
                {
                    Common.Additions.Poisen poisen = new Additions.Poisen(arg, i.Target, 52000206, i.Damage, 12000);
                    poisen.Activate();
                }
            }
            /*arg.Caster.HP = 0;
            SkillAffectedActor self = new SkillAffectedActor();
            self.Target = arg.Caster;
            self.Damage = arg.Caster.MaxHP;
            self.Result = SkillAttackResult.Normal;
            arg.AffectedActors.Add(self);*/
            Common.Additions.Suicide suicide = new Additions.Suicide(arg, arg.Caster, 52000206);
            suicide.Activate();
        }
    }
}
