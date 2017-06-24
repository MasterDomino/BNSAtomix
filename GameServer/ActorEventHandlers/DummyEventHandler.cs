using System.Collections.Generic;

using SmartEngine.Network.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.ActorEventHandlers
{
    public class DummyEventHandler : BNSActorEventHandler
    {
        public DummyEventHandler()
        {

        }

        #region ActorEventHandler 成员

        public override void OnCreate(bool success)
        {

        }

        public override void OnDelete()
        {

        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {

        }

        public override void OnActorStopsMoving(Actor mActor, MoveArg arg)
        {

        }

        public override void OnActorAppears(Actor aActor)
        {

        }

        public override void OnActorDisappears(Actor dActor)
        {

        }

        public override void OnTeleport(float x, float y, float z)
        {

        }

        public override void OnGotVisibleActors(List<Actor> actors)
        {

        }

        public override void OnActorEnterPortal(Actor aActor)
        {

        }

        public override void OnDie(ActorExt killedBy)
        {

        }

        public override void OnSkillDamage(SkillArg arg, SkillAttackResult result, int dmg, int bonusCount)
        {
        }
        #endregion
    }
}
