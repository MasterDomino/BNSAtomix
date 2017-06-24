using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Common.Additions
{
    public abstract class BonusAddition : Buff
    {
        public short AccumulateCount { get; set; }
        public uint BonusAdditionID { get; set; }
        public BonusAddition(ActorExt actor, string name, int duration)
            : base(actor, name, duration)
        {
        }
    }
}
