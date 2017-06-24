using System.Threading;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
using SagaBNS.Common.Actors;
using SmartEngine.Network.Tasks;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Common
{
    public class FoodRecovery : ISkillHandler
    {
        private readonly int recovery;

        public void HandleOnSkillCasting(SkillArg arg)
        {
        }

        public void HandleOnSkillCastFinish(SkillArg arg)
        {
        }

        public FoodRecovery(int recovery)
        {
            this.recovery = recovery;
        }

        #region ISkillHandler 成员

        public unsafe void HandleSkillActivate(SkillArg arg)
        {
            if (arg.Caster.Tasks.ContainsKey("FoodRecovery"))
            {
                Buff buff = arg.Caster.Tasks["FoodRecovery"] as Buff;
                buff.Deactivate();
            }
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            Buff add = new Buff(arg.Caster, "FoodRecovery", arg.Skill.BaseData.Duration, 2000);
            add.OnAdditionStart += (actor, addition) =>
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = actor,
                    Target = actor,
                    Skill = arg.Skill,
                    SkillSession = arg.SkillSession,
                    AdditionID = arg.Skill.BaseData.Effect,
                    AdditionSession = 20481,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                    UpdateType = UpdateTypes.Actor
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                evt = new UpdateEvent()
                {
                    Actor = actor,
                    Target = actor,
                    Skill = arg.Skill,
                    AdditionSession = 20481,
                    AdditionID = arg.Skill.BaseData.Effect,
                    RestTime = arg.Skill.BaseData.Duration,
                    SkillSession = arg.SkillSession,
                    UpdateType = UpdateTypes.ActorExtension
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
            };
            add.OnUpdate += (actor, addition) =>
            {
                ActorExt act = (ActorExt)actor;
                if (act.HP < act.MaxHP)
                {
                    Interlocked.Add(ref act.HP, recovery);
                    if (act.HP > act.MaxHP)
                    {
                        Interlocked.Exchange(ref act.HP, act.MaxHP);
                    }

                    UpdateEvent evt = new UpdateEvent()
                    {
                        Actor = actor,
                        Target = actor,
                        UpdateType = UpdateTypes.Actor
                    };
                    evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.HP, act.HP);
                    evt.SkillSession = arg.SkillSession;
                    evt.AdditionID = arg.Skill.BaseData.Effect;
                    evt.AdditionSession = 20481;
                    evt.ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Update;
                    byte[] buf = new byte[9];
                    fixed (byte* res = buf)
                    {
                        res[0] = 8;
                        *(int*)&res[1] = recovery;
                    }
                    evt.UserData = buf;
                    map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
                }
            };
            add.OnAdditionEnd += (actor, addition, cancel) =>
            {
                ((ActorExt)actor).Tasks.TryRemove("FoodRecovery", out Task removed);
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = actor,
                    Target = actor,
                    Skill = arg.Skill,
                    AdditionID = arg.Skill.BaseData.Effect,
                    AdditionSession = 20481,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                    SkillSession = arg.SkillSession,
                    UpdateType = UpdateTypes.Actor,
                    UserData = new byte[] { 9, 1, 0 }
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                evt = new UpdateEvent()
                {
                    Actor = actor,
                    Target = actor,
                    Skill = arg.Skill,
                    AdditionSession = 20481,
                    AdditionID = arg.Skill.BaseData.Effect,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                    SkillSession = arg.SkillSession,
                    UpdateType = UpdateTypes.ActorExtension
                };
                map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

                SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
            };
            arg.Caster.Tasks["FoodRecovery"] = add;
            add.Activate();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {

        }
        #endregion
    }
}
