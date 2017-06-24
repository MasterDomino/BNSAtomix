using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.Skills.SkillHandlers
{
    public interface ISkillHandler
    {
        void HandleOnSkillCasting(SkillArg arg);

        void HandleOnSkillCastFinish(SkillArg arg);

        void HandleSkillActivate(SkillArg arg);

        void OnAfterSkillCast(SkillArg arg);
    }
}
