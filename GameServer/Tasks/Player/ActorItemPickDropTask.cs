using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class ActorItemPickDropTask : Buff
    {
        private readonly ActorPC pc;
        private readonly ActorExt item;
        private readonly ActionTypes action;
        public enum ActionTypes
        {
            Pick,
            Drop,
            PickCorpse,
        }

        public ActorItemPickDropTask(ActorPC pc, ActorExt item, ActionTypes action)
            : base(pc, "ActorItemPickDropTask", 400)
        {
            this.pc = pc;
            this.item = item;
            this.action = action;
            OnAdditionStart += new StartEventHandler(ActorItemPickDropTask_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(ActorItemPickDropTask_OnAdditionEnd);

            if (pc.Tasks.TryRemove("ActorItemPickDropTask", out Task removed))
            {
                removed.Deactivate();
            }
        }

        private void ActorItemPickDropTask_OnAdditionEnd(SmartEngine.Network.Map.Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = MapManager.Instance.GetMap(pc.MapInstanceID);

            pc.Tasks.TryRemove("ActorItemPickDropTask", out Task removed);
            switch (action)
            {
                case ActionTypes.Pick :
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemPick,
                            Actor = pc,
                            Target = item,
                            UserData = (byte)1
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
                case ActionTypes.PickCorpse:
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemPickCorpse,
                            Actor = pc,
                            Target = item,
                            UserData = (byte)1
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
                case ActionTypes.Drop:
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemDrop,
                            Actor = pc,
                            Target = item,
                            X = (short)item.X,
                            Y = (short)item.Y,
                            Z = (short)item.Z,
                            UserData = (byte)1
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
            }
        }

        private void ActorItemPickDropTask_OnAdditionStart(SmartEngine.Network.Map.Actor actor, Buff skill)
        {
            Map.Map map = MapManager.Instance.GetMap(pc.MapInstanceID);
            switch (action)
            {
                case ActionTypes.Pick:
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemPick,
                            Actor = pc,
                            Target = item,
                            UserData = (byte)0
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
                case ActionTypes.PickCorpse:
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemPickCorpse,
                            Actor = pc,
                            Target = item,
                            UserData = (byte)0
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
                case ActionTypes.Drop:
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            UpdateType = UpdateTypes.ItemDrop,
                            Actor = pc,
                            Target = item,
                            X = (short)item.X,
                            Y = (short)item.Y,
                            Z = (short)item.Z,
                            UserData = (byte)0
                        };
                        map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, pc, true);
                    }
                    break;
            }
        }
    }
}
