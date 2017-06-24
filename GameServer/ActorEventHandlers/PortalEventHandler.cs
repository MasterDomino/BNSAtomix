using System;
using System.Collections.Generic;

using SmartEngine.Network.Map;
using SmartEngine.Core;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;

namespace SagaBNS.GameServer.ActorEventHandlers
{
    public class PortalEventHandler : BNSActorEventHandler
    {
        private readonly ActorPortal portal;
        public PortalEventHandler(ActorPortal portal)
        {
            this.portal = portal;
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
            if (aActor.ActorType == ActorType.PC)
            {
                bool warped = false;
                ActorPC pc = (ActorPC)aActor;
                foreach (PortalTrigger i in portal.PortalTriggers)
                {
                    if (i.Quest > 0)
                    {
                        if (pc.Quests.ContainsKey(i.Quest))
                        {
                            if (i.Step == pc.Quests[i.Quest].NextStep - 1 || i.Step == -1 || (i.Step ==0 && i.Step == pc.Quests[i.Quest].Step))
                            {
                                int abs = Math.Abs(i.Dir - pc.Dir);
                                if (abs <= 90 || abs > 270)
                                {
                                    Map.Map map = Map.MapManager.Instance.GetMap(i.MapTarget, pc.CharID, pc.PartyID);
                                    if (map != null)
                                    {
                                        warped = true;
                                        pc.Client().Map.SendActorToMap(pc, map, pc.X, pc.Y, pc.Z);
                                    }
                                    else
                                    {
                                        Logger.Log.Warn(string.Format("MapID:{0} isn't defined!", i.MapTarget));
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Map.Map map = Map.MapManager.Instance.GetMap(i.MapTarget, pc.CharID, pc.PartyID);
                        if (map != null)
                        {
                            warped = true;
                            pc.Client().Map.SendActorToMap(pc, map, pc.X, pc.Y, pc.Z);
                        }
                    }
                }
                if (!warped)
                {
                    pc.Client().SendPortalNotWarp();
                }
            }
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
