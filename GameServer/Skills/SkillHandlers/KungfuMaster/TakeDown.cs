using System.Collections.Generic;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.KungfuMaster
{
    public class TakeDown : ISkillHandler
    {
        private readonly uint additionSelf, additionTarget;
        public TakeDown(uint additionSelf = 55111142, uint additionTarget = 55111140)
        {
            this.additionTarget = additionTarget;
            this.additionSelf = additionSelf;
        }

        #region ISkillHandler 成员

        public void HandleOnSkillCasting(SkillArg arg)
        {
        }

        public void HandleOnSkillCastFinish(SkillArg arg)
        {
        }

        public void HandleSkillActivate(SkillArg arg)
        {
            SkillManager.Instance.DoAttack(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                SkillAttackResult res = i.Result;
                if (res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss)
                {
                    if (arg.Caster.Tasks.ContainsKey("ActorTakeDown"))
                    {
                        Buff buff = arg.Caster.Tasks["ActorTakeDown"] as Buff;
                        buff.Deactivate();
                    }
                    if (i.Target.Tasks.ContainsKey("ActorDown"))
                    {
                        Buff buff = i.Target.Tasks["ActorDown"] as Buff;
                        buff.TotalLifeTime += 3000;
                    }
                    Buff add = new Buff(arg.Caster, "ActorTakeDown", 3000);
                    add.OnAdditionStart += (actor, addition) =>
                        {
                            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
                            if (!arg.Caster.Status.TakeDown)
                            {
                                arg.Caster.Status.TakeDown = true;
                                i.Target.Status.TakenDown = true;
                                i.Target.Status.StanceFlag1.SetValue(StanceU1.TakenDown, true);
                                i.Target.Status.StanceFlag2.SetValue(StanceU2.TakenDown1, true);
                                i.Target.Status.StanceFlag2.SetValue(StanceU2.TakenDown2, true);
                                i.Target.Status.InteractWith = arg.Caster.ActorID;
                                arg.Caster.Status.InteractWith = i.Target.ActorID;
                                i.Target.Dir = (ushort)(arg.Caster.Dir + 180);
                                if (i.Target.Dir >= 360)
                                {
                                    i.Target.Dir -= 360;
                                }

                                UpdateEvent evt = new UpdateEvent()
                                {
                                    Actor = arg.Caster,
                                    Target = arg.Caster,
                                    SkillSession = arg.SkillSession,
                                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                                    UpdateType = UpdateTypes.Actor
                                };
                                evt.AddActorPara(PacketParameter.X, i.Target.X);
                                evt.AddActorPara(PacketParameter.Y, i.Target.Y);
                                //evt.AddActorPara(PacketParameter.InteractWith, (long)arg.Caster.Status.InteractWith);
                                //evt.AddActorPara(PacketParameter.InteractionType, 1);
                                //evt.AddActorPara(PacketParameter.InteractionRelation, 1);
                                //evt.AddActorPara(PacketParameter.Unk74, 1);
                                //evt.AddActorPara(PacketParameter.UnkD7, 1);
                                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                                evt = new UpdateEvent()
                                {
                                    Actor = arg.Caster,
                                    AdditionID = additionTarget,
                                    Target = i.Target,
                                    Skill = arg.Skill,
                                    SkillSession = arg.SkillSession,
                                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                                    UpdateType = UpdateTypes.Actor
                                };
                                //evt.AddActorPara(PacketParameter.Unk7A, i.Target.Status.StanceFlag1.Value);
                                //evt.AddActorPara(PacketParameter.Unk7B, i.Target.Status.StanceFlag2.Value);
                                //evt.AddActorPara(PacketParameter.Dir, i.Target.Dir);
                                //evt.AddActorPara(PacketParameter.InteractWith, (long)i.Target.Status.InteractWith);
                                //evt.AddActorPara(PacketParameter.InteractionType, 1);
                                //evt.AddActorPara(PacketParameter.InteractionRelation, 2);
                                //evt.AddActorPara(PacketParameter.Unk74, 1);
                                //evt.AddActorPara(PacketParameter.UnkD7, 1);
                                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                                evt = UpdateEvent.NewActorAdditionExtEvent(i.Target, arg.SkillSession, 11, additionTarget, 3000, UpdateEvent.ExtraUpdateModes.Activate);
                                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                                evt = new UpdateEvent()
                                {
                                    Actor = arg.Caster,
                                    Target = arg.Caster,
                                    AdditionID = additionSelf,
                                    SkillSession = arg.SkillSession,
                                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                                    UpdateType = UpdateTypes.Actor
                                };
                                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                                evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 11, additionSelf, 3000, UpdateEvent.ExtraUpdateModes.Activate);
                                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
                            }
                        };
                    add.OnAdditionEnd += (actor, addition, cancel) =>
                        {
                            Map.Map map = MapManager.Instance.GetMap(actor.MapInstanceID);
                            if (i.Target.Tasks.ContainsKey("ActorDown"))
                            {
                                Buff buff = i.Target.Tasks["ActorDown"] as Buff;
                                buff.Deactivate();
                            }
                            arg.Caster.Status.TakeDown = false;
                            i.Target.Status.TakenDown = false;
                            i.Target.Status.StanceFlag1.SetValue(StanceU1.TakenDown, false);
                            i.Target.Status.StanceFlag2.SetValue(StanceU2.TakenDown1, false);
                            i.Target.Status.StanceFlag2.SetValue(StanceU2.TakenDown2, false);
                            i.Target.Status.InteractWith = 0;
                            arg.Caster.Status.InteractWith = 0;
                            arg.Caster.Tasks.TryRemove("ActorTakeDown", out Task removed);
                            UpdateEvent evt = new UpdateEvent()
                            {
                                Actor = arg.Caster,
                                Target = arg.Caster,
                                SkillSession = arg.SkillSession,
                                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                                UpdateType = UpdateTypes.Actor
                            };
                            //evt.AddActorPara(PacketParameter.X, arg.Caster.X);
                            //evt.AddActorPara(PacketParameter.Y, arg.Caster.Y);
                            //evt.AddActorPara(PacketParameter.InteractWith, (long)arg.Caster.Status.InteractWith);
                            //evt.AddActorPara(PacketParameter.InteractionType, 0);
                            //evt.AddActorPara(PacketParameter.InteractionRelation, 0);
                            //evt.AddActorPara(PacketParameter.Unk74, 0);
                            //evt.AddActorPara(PacketParameter.UnkD7, 0);
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                            evt = UpdateEvent.NewActorAdditionExtEvent(arg.Caster, arg.SkillSession, 11, additionSelf, 3000, UpdateEvent.ExtraUpdateModes.Cancel);
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

                            evt = new UpdateEvent()
                            {
                                Actor = arg.Caster,
                                AdditionID = additionTarget,
                                Target = i.Target,
                                Skill = arg.Skill,
                                SkillSession = arg.SkillSession,
                                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                                UpdateType = UpdateTypes.Actor,
                                //evt.AddActorPara(PacketParameter.Unk7A, i.Target.Status.StanceFlag1.Value);
                                //evt.AddActorPara(PacketParameter.Unk7B, i.Target.Status.StanceFlag2.Value);
                                //evt.AddActorPara(PacketParameter.InteractWith, (long)i.Target.Status.InteractWith);
                                //evt.AddActorPara(PacketParameter.InteractionType, 0);
                                //evt.AddActorPara(PacketParameter.InteractionRelation, 0);
                                //evt.AddActorPara(PacketParameter.Unk74, 0);
                                //evt.AddActorPara(PacketParameter.UnkD7, 0);
                                UserData = new byte[] { 9, 1, 0 }
                            };
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);

                            evt = UpdateEvent.NewActorAdditionExtEvent(i.Target, arg.SkillSession, 11, additionTarget, 3000, UpdateEvent.ExtraUpdateModes.Cancel);
                            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, i.Target, true);
                        };
                    arg.Caster.Tasks["ActorTakeDown"] = add;
                    add.Activate();
                }
            }
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
