using SagaBNS.Common.Actors;

namespace SagaBNS.Common.Skills
{
    public class SkillAffectedActor
    {
        public SkillAttackResult Result;
        public ActorExt Target;
        public bool NoDamageBroadcast;
        public int Damage;
        public uint BonusAdditionID;
    }
}
