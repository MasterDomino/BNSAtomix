using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.GameServer.Map;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Tasks.Player;
namespace SagaBNS.GameServer.ActorEventHandlers
{
    public class PCEventHandler : BNSActorEventHandler
    {
        private readonly GameSession client;
        public GameSession Client { get { return client; } }

        public PCEventHandler(GameSession client)
        {
            this.client = client;
        }
        #region ActorEventHandler 成员

        public override void OnCreate(bool success)
        {
            if (success)
            {
                if (!client.Authenticated)
                {
                    client.SendLoginInit();
                }
                else
                {
                    client.SendWarp();
                }
            }
        }

        public override void OnDelete()
        {

        }

        public unsafe override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor.ActorType == ActorType.ITEM || client.BroadcastService == null)
            {
                return;
            }

            if (mActor.ActorType != ActorType.NPC || ((MoveArgument)arg).BNSMoveType != Map.MoveType.Dash)
            {
                MoveArgument move = arg as MoveArgument;
                switch (move.BNSMoveType)
                {
                    case Map.MoveType.StepForward :
                        {
                            UpdateEvent evt = new UpdateEvent()
                            {
                                UpdateType = UpdateTypes.Actor,
                                AdditionSession = 0x7001,
                                Actor = mActor,
                                Target = mActor,
                                SkillSession = move.SkillSession,
                                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate
                            };
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.X, move.X);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Y, move.Y);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Z, move.Z);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dir, move.Dir);
                            byte[] buf = new byte[11];
                            fixed (byte* p = buf)
                            {
                                p[0] = 5;
                                short* ptr = (short*)&p[1];
                                ptr[0] = (short)move.X;
                                ptr[1] = (short)move.Y;
                                ptr[2] = (short)move.Z;
                                ptr[3] = (short)move.Dir;
                                ptr[4] = 1;
                            }
                            evt.UserData = buf;
                            client.BroadcastService.EnqueueUpdateEvent(evt);
                            evt = new UpdateEvent()
                            {
                                Actor = mActor,
                                AdditionSession = 0x7001,
                                UpdateType = UpdateTypes.ActorExtension
                            };
                            //client.BroadcastService.EnqueueUpdateEvent(evt);
                        }
                        break;
                    case Map.MoveType.PushBack :
                        {
                            UpdateEvent evt = new UpdateEvent()
                            {
                                UpdateType = UpdateTypes.Actor,
                                AdditionSession = 0x1001,
                                Actor = ((MoveArgument)arg).PushBackSource,
                                Target = mActor,
                                SkillSession = move.SkillSession,
                                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate
                            };
                            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Unk7A, 0x2000);
                            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.UnkD5, 1);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.X, move.X);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Y, move.Y);
                            evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dir, move.Dir);
                            byte[] buf = new byte[20];
                            fixed (byte* p = buf)
                            {
                                short* ptr = (short*)p;
                                ptr[0] = 6;
                                ptr[1] = (short)move.X;
                                ptr[2] = (short)move.Y;
                                ptr[3] = (short)move.Z;
                                ptr[4] = (short)move.Dir;
                                ptr[5] = 1;
                            }
                            evt.UserData = buf;
                            client.BroadcastService.EnqueueUpdateEvent(evt);
                        }
                        break;
                    default:
                        {
                            UpdateEvent evt = new UpdateEvent()
                            {
                                UpdateType = UpdateTypes.Movement,
                                Actor = mActor,
                                MoveArgument = (MoveArgument)arg
                            };
                            client.BroadcastService.EnqueueUpdateEvent(evt);
                        }
                        break;
                }
            }
            else
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    UpdateType = UpdateTypes.NPCDash,
                    Actor = mActor,
                    MoveArgument = new MoveArgument()
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);

                evt = new UpdateEvent()
                {
                    UpdateType = UpdateTypes.NPCDash,
                    Actor = mActor,
                    MoveArgument = (MoveArgument)arg
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);

                /*evt = new UpdateEvent();
                evt.UpdateType = UpdateTypes.Unknown423;
                evt.Actor = mActor;
                evt.MoveArgument = (MoveArgument)arg;

                client.BroadcastService.EnqueueUpdateEvent(evt);*/

                /*evt = new UpdateEvent();
                evt.UpdateType = UpdateTypes.Unknown50E;
                evt.Actor = mActor;
                evt.MoveArgument = (MoveArgument)arg;

                client.BroadcastService.EnqueueUpdateEvent(evt);*/
            }
        }

        public override void OnActorStopsMoving(Actor mActor, MoveArg arg)
        {

        }

        public override void OnActorAppears(Actor aActor)
        {
            if (client.BroadcastService == null)
            {
                return;
            }

            ActorCorpse corpse = aActor as ActorCorpse;
            ActorItem item = aActor as ActorItem;
            if (corpse != null)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = aActor,
                    UpdateType = UpdateTypes.ShowCorpse
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
            else if (item != null)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = item.Creator,
                    Target = item,
                    UpdateType = UpdateTypes.ItemAppear
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
            else
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    UpdateType = UpdateTypes.Appear,
                    Actor = aActor
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
        }

        public override void OnActorDisappears(Actor dActor)
        {
            if (client.BroadcastService == null)
            {
                return;
            }

            ActorCorpse corpse = dActor as ActorCorpse;
            ActorItem item = dActor as ActorItem;
            if (corpse != null)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = dActor,
                    UpdateType = UpdateTypes.DeleteCorpse
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
            else if (item != null)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = item.Creator,
                    Target = item,
                    UpdateType = UpdateTypes.ItemDisappear
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
            else
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    UpdateType = UpdateTypes.Disappear,
                    Actor = dActor
                };
                client.BroadcastService.EnqueueUpdateEvent(evt);
            }
        }

        public override void OnTeleport(float x, float y, float z)
        {

        }

        public override void OnGotVisibleActors(List<Actor> actors)
        {
            var query = from actor in actors
                        where actor.ActorType == ActorType.PC || actor.ActorType == ActorType.NPC
                        select actor;
            client.SendActorList(query.ToList());
            foreach (ActorExt i in (from actor in actors where actor.ActorType != ActorType.PC && actor.ActorType != ActorType.NPC select actor))
            {
                OnActorAppears(i);
            }
        }

        #endregion

        public void OnBroadcastEvt(UpdateEvent evt)
        {
            client.BroadcastService?.EnqueueUpdateEvent(evt);
        }

        public void OnChat(ChatArgument arg)
        {
            client.SendChat(arg);
        }

        public override void OnDie(ActorExt killedBy)
        {
            {
                if (client.Character.Tasks.TryGetValue("ActorCatch", out Task removed))
                {
                    removed.Deactivate();
                }
            }
            if (!client.Character.Status.Dying)
            {
                client.Character.Status.Dying = true;
                DieTask task = new DieTask(client);
                client.Character.Tasks["DieTask"] = task;
                task.Activate();
            }
            else
            {
                if (client.Character.Tasks.TryRemove("DieTask", out Task removed))
                {
                    removed.Deactivate();
                }

                if (client.Character.Tasks.TryRemove("RecoverTask", out removed))
                {
                    removed.Deactivate();
                }

                client.Character.Status.Dying = false;
                client.Character.Status.Recovering = false;
            }
        }

        public override void OnActorEnterPortal(Actor aActor)
        {

        }

        public override void OnSkillDamage(SkillArg arg, SkillAttackResult result, int dmg, int bonusCount)
        {

        }
    }
}
