using System.Threading;
using SmartEngine.Network.Tasks;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Skills.SkillHandlers.Assassin
{
    public class Dash : ISkillHandler
    {
        private readonly bool isBack;

        public Dash(bool isBack)
        {
            this.isBack = isBack;
        }

        #region ISkillHandler 成员

        public void HandleOnSkillCasting(SkillArg arg)
        {
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Dash, true);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                AdditionID = 15000030,
                AdditionSession = 1,
                Target = arg.Caster,
                Skill = arg.Skill,
                SkillSession = arg.SkillSession,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF4, 1);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF1, 1);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                Target = arg.Caster,
                AdditionSession = 1,
                AdditionID = 15000030,
                RestTime = arg.ApproachTime,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            if (isBack)
            {
                arg.Caster.Status.Stealth = true;
            }

            Map.MoveArgument argu = new Map.MoveArgument()
            {
                BNSMoveType = MoveType.StepForward
            };
            int distance = arg.Caster.DistanceToActor(arg.Target);
            int forward = distance < 40 ? 0 : distance - 20;
            argu.SkillSession = arg.SkillSession;
            float deltaX = (float)(arg.Target.X - arg.Caster.X) / distance;
            float deltaY = (float)(arg.Target.Y - arg.Caster.Y) / distance;

            argu.X = arg.Caster.X + (int)(deltaX * forward);
            argu.Y = arg.Caster.Y + (int)(deltaY * forward);
            argu.Z = (short)arg.Target.Z;
            argu.Dir = arg.Caster.Dir;
            map.MoveActor(arg.Caster, argu, true);
        }

        public void HandleOnSkillCastFinish(SkillArg arg)
        {
            Map.Map map = MapManager.Instance.GetMap(arg.Caster.MapInstanceID);
            arg.Caster.Status.StanceFlag1.SetValue(StanceU1.Dash, false);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                AdditionID = 15000030,
                AdditionSession = 1,
                Target = arg.Caster,
                Skill = arg.Skill,
                SkillSession = arg.SkillSession,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.Unk7A, arg.Caster.Status.StanceFlag1.Value);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF4, 0);
            //evt.AddActorPara(SagaBNS.Common.Packets.GameServer.PacketParameter.UnkF1, 0);
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);

            evt = new UpdateEvent()
            {
                Actor = arg.Caster,
                Target = arg.Caster,
                AdditionSession = 1,
                AdditionID = 15000030,
                RestTime = arg.ApproachTime,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.ActorExtension
            };
            map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, arg.Caster, true);
        }

        public void HandleSkillActivate(SkillArg arg)
        {
            if (isBack)
            {
                if (arg.Caster.Tasks.ContainsKey("Stealth"))
                {
                    Buff buff = arg.Caster.Tasks["Stealth"] as Buff;
                    buff.Deactivate();
                }

                Additions.Stealth add = new Additions.Stealth(arg);

                arg.Caster.Tasks["Stealth"] = add;
                add.Activate();
                Interlocked.Add(ref arg.Caster.MP, 3);
            }
            else
            {
                Interlocked.Increment(ref arg.Caster.MP);
            }
            if (arg.Caster.MP > arg.Caster.MaxMP)
            {
                Interlocked.Exchange(ref arg.Caster.MP, arg.Caster.MaxMP);
            }

            Network.Client.GameSession client = ((ActorPC)arg.Caster).Client();
            client?.SendPlayerMP();
        }

        public void OnAfterSkillCast(SkillArg arg)
        {
        }
        #endregion
    }
}
