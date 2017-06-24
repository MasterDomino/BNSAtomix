using System;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Tasks.Actor
{
    public class CorpseDeleteTask : Task
    {
        private readonly ActorCorpse actor;
        private bool already;
        private bool shoudDisappear;
        public CorpseDeleteTask(int disappearTime, ActorCorpse actor)
            : base(200, disappearTime, "CorpseDelete")
        {
            this.actor = actor;
            if (actor.Tasks.TryRemove("CorpseDelete", out Task removed))
            {
                removed.Deactivate();
            }
            /*if (actor.NPC.BaseData.QuestIDs.Count > 0)
{
   actor.QuestID = actor.NPC.BaseData.QuestIDs[0];
}*/
            actor.Tasks["CorpseDelete"] = this;
        }

        public override void CallBack()
        {
            try
            {
                Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
                if (map == null)
                {
                    actor.Tasks.TryRemove("CorpseDelete", out Task task);
                    Deactivate();
                }
                if (!already)
                {
                    already = true;
                    actor.Invisible = false;
                    map.OnActorVisibilityChange(actor);
                    map.DeleteActor(actor.NPC);
                }
                else
                {
                    if ((actor.AvailableItems.Count > 0) || actor.NPC.BaseData.QuestIDs.Count > 0)
                    {
                        if (!shoudDisappear)
                        {
                            shoudDisappear = true;
                            dueTime = 60000;
                            Activate();
                            actor.ShouldDisappear = true;
                            UpdateEvent evt = new UpdateEvent()
                            {
                                Actor = actor,
                                UpdateType = UpdateTypes.DeleteCorpse
                            };
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, false);
                        }
                        else
                        {
                            actor.Items = null;
                            actor.Gold = 0;
                            if (actor.CurrentPickingPlayer > 0)
                            {
                                if (map.GetActor(actor.CurrentPickingPlayer) is ActorPC pc)
                                {
                                    pc.Client().SendActorCorpseClose(actor.ActorID);
                                }
                            }
                            actor.Tasks.TryRemove("CorpseDelete", out Task task);
                            Deactivate();
                            actor.ShouldDisappear = true;
                            map.DeleteActor(actor);
                        }
                    }
                    else
                    {
                        actor.Tasks.TryRemove("CorpseDelete", out Task task);
                        Deactivate();
                        actor.ShouldDisappear = true;
                        map.DeleteActor(actor);
                    }
                }
            }
            catch (Exception ex)
            {
                SmartEngine.Core.Logger.Log.Error(ex);
                actor.Tasks.TryRemove("CorpseDelete", out Task task);
                Deactivate();
            }
        }
    }
}
