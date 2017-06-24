
using SmartEngine.Network.Map;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;
namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin.Additions
{
    public class Mine : Buff
    {
        private readonly SkillArg arg;
        public Mine(SkillArg arg)
            : base(arg.Caster, "Mine", 10000)
        {
            this.arg = arg;
            OnAdditionStart += new StartEventHandler(Mine_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(Mine_OnAdditionEnd);
        }

        private void Mine_OnAdditionEnd(Actor actor, Buff skill, bool cancel)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            ((ActorExt)actor).Tasks.TryRemove("Mine", out Task removed);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = 128,
                AdditionID = 15208019,
                AdditionSession = 1,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel
            };
            evt.SkillSession = arg.SkillSession;
            evt.UpdateType = UpdateTypes.Actor;
            evt.UserData = new byte[] { 9, 3, 0 };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 1,
                AdditionID = 15208019,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                SkillSession = 128,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
            SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.Abort);
            SkillManager.Instance.DoAttack(arg);
            foreach (SkillAffectedActor i in arg.AffectedActors)
            {
                SkillAttackResult res = i.Result;
                if (res != SkillAttackResult.Avoid && res != SkillAttackResult.Miss && res != SkillAttackResult.Parry && res != SkillAttackResult.TotalParry)
                {
                    if (i.Target.Tasks.ContainsKey("ActorDown"))
                    {
                        Buff buff = i.Target.Tasks["ActorDown"] as Buff;
                        buff.Deactivate();
                    }
                    i.NoDamageBroadcast = true;
                    Common.Additions.ActorDown add = new Common.Additions.ActorDown(arg, i.Target, 15208018, i.Damage);

                    i.Target.Tasks["ActorDown"] = add;
                    add.Activate();
                }
            }
            SkillManager.Instance.BroadcastSkillCast(arg, SkillMode.End);
        }

        private void Mine_OnAdditionStart(Actor actor, Buff skill)
        {
            Map.Map map = Map.MapManager.Instance.GetMap(actor.MapInstanceID);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                SkillSession = 128,
                AdditionID = 15208019,
                AdditionSession = 1,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                UpdateType = UpdateTypes.Actor
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);

            evt = new UpdateEvent()
            {
                Actor = actor,
                Target = actor,
                AdditionSession = 1,
                AdditionID = 15208019,
                RestTime = arg.Skill.BaseData.Duration,
                SkillSession = 128,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, actor, true);
        }
    }
}
