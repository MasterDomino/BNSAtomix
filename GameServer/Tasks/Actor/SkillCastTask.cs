using SmartEngine.Network.Tasks;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Actors;

namespace SagaBNS.GameServer.Tasks.Actor
{
    public class SkillCastTask : Task
    {
        private readonly ActorExt actor;
        private readonly SkillArg skill;
        public SkillCastTask(int castTime, ActorExt actor, SkillArg skill)
            : base(castTime, castTime, "SkillCast")
        {
            this.actor = actor;
            this.skill = skill;
        }

        public override void CallBack()
        {
            Deactivate();
            actor.Tasks.TryRemove("SkillCast", out Task task);
            if ((skill.Skill.BaseData.ActionTime > 0 || skill.Skill.BaseData.ShouldApproach) && skill.Skill.BaseData.CastTime > 0 && !skill.CastFinished)
            {
                skill.CastFinished = true;
                Skills.SkillManager.Instance.SkillCast(skill);
            }
            else
            {
                Skills.SkillManager.Instance.SkillActivate(skill);
            }
        }
    }
}
