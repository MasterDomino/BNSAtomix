using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.BladeMaster
{
    public class SwordBlocking : ISkillHandler
    {
        private readonly uint motion;
        public SwordBlocking()
        {
        }

        public SwordBlocking(uint motion)
        {
            this.motion = motion;
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
            if (arg.Caster.Tasks.ContainsKey("SwordBlocking"))
            {
                Buff buff = arg.Caster.Tasks["SwordBlocking"] as Buff;
                buff.Deactivate();
            }
            Buff add = new Buff(arg.Caster, "SwordBlocking", arg.Skill.BaseData.Duration);
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            add.OnAdditionStart += (actor, addition) =>
                {
                    ((ActorExt)actor).Status.Blocking = true;
                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = actor,
                        AdditionSession = 20481,
                        Target = actor,
                        Skill = arg.Skill,
                        AdditionID = motion,
                        SkillSession = arg.SkillSession,
                        ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                        UpdateType = UpdateTypes.Actor
                    };
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 1);

                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                    evt = new UpdateEvent()
                    {
                        Actor = actor,
                        AdditionSession = 20481,
                        SkillSession = arg.SkillSession
                    };
                    if (motion > 0)
                    {
                        evt.AdditionID = motion;
                    }
                    else
                    {
                        evt.AdditionID = arg.Skill.ID < 1000000 ? (arg.Skill.ID * 1000 + 11) : (arg.Skill.ID * 10 + 1);
                    }

                    evt.RestTime = arg.Skill.BaseData.Duration;
                    evt.UpdateType = UpdateTypes.ActorExtension;

                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
                };
            add.OnAdditionEnd += (actor, addition, cancel) =>
                {
                    ((ActorExt)actor).Status.Blocking = false;
                    ((ActorExt)actor).Tasks.TryRemove("SwordBlocking", out Task removed);
                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = actor,
                        Target = actor,
                        AdditionSession = 20481,
                        AdditionID = motion,
                        Skill = arg.Skill,
                        ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                        SkillSession = arg.SkillSession,
                        UpdateType = UpdateTypes.Actor,
                        UserData = new byte[] { 9, 1, 0 }
                    };
                    //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.BlockingStance, 0);
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                    evt = new UpdateEvent()
                    {
                        Actor = actor,
                        AdditionSession = 20481,
                        SkillSession = arg.SkillSession
                    };
                    if (motion > 0)
                    {
                        evt.AdditionID = motion;
                    }
                    else
                    {
                        evt.AdditionID = arg.Skill.ID < 1000000 ? (arg.Skill.ID * 1000 + 11) : (arg.Skill.ID * 10 + 1);
                    }

                    evt.ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel;
                    evt.UpdateType = UpdateTypes.ActorExtension;

                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
                    SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
                };
            arg.Caster.Tasks["SwordBlocking"] = add;
            add.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
