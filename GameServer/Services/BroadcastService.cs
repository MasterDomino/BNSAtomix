using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Map;
using SagaBNS.GameServer.Network.Client;
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SagaBNS.GameServer.Services
{
    /// <summary>
    /// 线程安全的广播服务，将尽可能多的广播事件整合到一个封包
    /// </summary>
    public class BroadcastService : Task
    {
        #region Members

        private readonly ConcurrentDictionary<ulong, UpdateEvent> moveEvents = new ConcurrentDictionary<ulong, UpdateEvent>();
        private ConcurrentQueue<UpdateEvent> appearEvents = new ConcurrentQueue<UpdateEvent>();
        private GameSession client;
        private ConcurrentQueue<UpdateEvent> disappearEvents = new ConcurrentQueue<UpdateEvent>();
        private ConcurrentQueue<UpdateEvent> events = new ConcurrentQueue<UpdateEvent>();

        #endregion

        #region Instantiation

        public BroadcastService(GameSession client) : base(0, 100, "BroadcastService")//500ms interval
        {
            this.client = client;
        }

        #endregion

        #region Methods

        public override void CallBack()
        {
            List<UpdateEvent> shouldUpdate = new List<UpdateEvent>();
            List<Actor> shouldAppear = new List<Actor>();
            List<Actor> shouldDisappear = new List<Actor>();
            if (appearEvents == null || disappearEvents == null || events == null || client == null)
            {
                return;
            }

            int maxCount = events.Count;
            int count = 0;
            do
            {
                while (shouldAppear.Count < 20 && appearEvents.TryDequeue(out UpdateEvent current))
                {
                    if (((ActorExt)current.Actor).Status.IsInCombat)
                    {
                        UpdateEvent evt = new UpdateEvent()
                        {
                            Actor = current.Actor,
                            Target = current.Actor,
                            UpdateType = UpdateTypes.Actor
                        };

                        //evt.AddActorPara(PacketParameter.CombatStatus, 1);
                        EnqueueUpdateEvent(evt);
                    }
                    shouldAppear.Add(current.Actor);
                }
                while (shouldDisappear.Count < 20 && disappearEvents.TryDequeue(out UpdateEvent current))
                {
                    shouldDisappear.Add(current.Actor);
                }
                while (shouldUpdate.Count < 20 && events.TryDequeue(out UpdateEvent current))
                {
                    shouldUpdate.Add(current);
                    if (current.UpdateType == UpdateTypes.Movement)
                    {
                        moveEvents.TryRemove(current.Actor.ActorID, out UpdateEvent removed);
                    }
                    count++;
                }

                if (client != null)
                {
                    if (shouldUpdate.Count > 0)
                    {
                        client.SendActorUpdates(shouldUpdate);
                    }

                    if (shouldAppear.Count > 0 || shouldDisappear.Count > 0)
                    {
                        client.SendActorAppear(shouldAppear, shouldDisappear);
                    }
                }
                shouldUpdate.Clear();
                shouldAppear.Clear();
                shouldDisappear.Clear();
            } while (events?.Count > 0 && count < maxCount);
        }

        public void EnqueueUpdateEvent(UpdateEvent evt)
        {
            switch (evt.UpdateType)
            {
                case UpdateTypes.Appear:
                    appearEvents.Enqueue(evt);
                    break;

                case UpdateTypes.Disappear:
                    disappearEvents.Enqueue(evt);
                    break;

                default:
                    if (evt.UpdateType == UpdateTypes.Movement)
                    {
                        if (moveEvents.TryGetValue(evt.Actor.ActorID, out UpdateEvent old))
                        {
                            old.MoveArgument.BNSMoveType = evt.MoveArgument.BNSMoveType;
                            old.MoveArgument.DashID = evt.MoveArgument.DashID;
                            old.MoveArgument.DashUnknown = evt.MoveArgument.DashUnknown;
                            old.MoveArgument.Dir = evt.MoveArgument.Dir;
                            old.MoveArgument.MoveType = evt.MoveArgument.MoveType;
                            old.MoveArgument.PushBackSource = evt.MoveArgument.PushBackSource;
                            old.MoveArgument.SkillSession = evt.MoveArgument.SkillSession;
                            old.MoveArgument.Speed = evt.MoveArgument.Speed;
                            old.MoveArgument.X = evt.MoveArgument.X;
                            old.MoveArgument.Y = evt.MoveArgument.Y;
                            old.MoveArgument.Z = evt.MoveArgument.Z;
                            while (evt.MoveArgument.PosDiffs.TryDequeue(out sbyte[] diffs))
                            {
                                old.MoveArgument.PosDiffs.Enqueue(diffs);
                            }

                            events.Enqueue(evt);
                        }
                        else
                        {
                            UpdateEvent newE = new UpdateEvent()
                            {
                                Actor = evt.Actor,
                                UpdateType = UpdateTypes.Movement,
                                MoveArgument = new MoveArgument()
                                {
                                    BNSMoveType = evt.MoveArgument.BNSMoveType,
                                    DashID = evt.MoveArgument.DashID,
                                    DashUnknown = evt.MoveArgument.DashUnknown,
                                    Dir = evt.MoveArgument.Dir,
                                    MoveType = evt.MoveArgument.MoveType,
                                    PushBackSource = evt.MoveArgument.PushBackSource,
                                    SkillSession = evt.MoveArgument.SkillSession,
                                    Speed = evt.MoveArgument.Speed,
                                    X = evt.MoveArgument.X,
                                    Y = evt.MoveArgument.Y,
                                    Z = evt.MoveArgument.Z
                                }
                            };
                            while (evt.MoveArgument.PosDiffs.TryDequeue(out sbyte[] diffs))
                            {
                                newE.MoveArgument.PosDiffs.Enqueue(diffs);
                            }

                            events.Enqueue(evt);
                        }
                    }
                    else
                    {
                        events.Enqueue(evt);
                    }
                    break;
            }
        }

        protected override void OnDeactivate()
        {
            events = null;
            appearEvents = null;
            disappearEvents = null;
            client = null;
        }

        #endregion
    }
}