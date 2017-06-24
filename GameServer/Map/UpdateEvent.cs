using System.Collections.Generic;

using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.Common.Skills;

using SmartEngine.Network.Map;

namespace SagaBNS.GameServer.Map
{
    public class UpdateEvent : MapEventArgs
    {
        public enum ExtraUpdateModes
        {
            None,
            Activate,
            Cancel,
            Update,
        }

        private readonly List<ActorUpdateParameter> paras = new List<ActorUpdateParameter>();
        public UpdateTypes UpdateType { get; set; }
        public Actor Actor { get; set; }
        public ushort AdditionSession { get; set; }
        public uint AdditionID { get; set; }
        public byte SkillSession { get; set; }
        public ExtraUpdateModes ExtraActivateMode { get; set; }
        public MoveArgument MoveArgument { get; set; }
        public Actor Target { get; set; }
        public Skill Skill { get; set; }
        public SkillMode SkillMode { get; set; }
        public SkillCastMode SkillCastMode { get; set; }
        public SkillAttackResult SkillAttackResult { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public int RestTime { get; set; }
        public short AdditionCount { get; set; }

        public object UserData { get; set; }
        public object UserData2 { get; set; }
        public List<ActorUpdateParameter> ActorUpdateParameters { get { return paras; } }

        public UpdateEvent()
        {
            AdditionCount = 1;
        }

        public void AddActorPara(PacketParameter para, long val)
        {
            ActorUpdateParameter p = new ActorUpdateParameter(para)
            {
                Value = val
            };
            paras.Add(p);
        }

        public static UpdateEvent NewActorAdditionEvent(ActorExt src, ActorExt target, byte session, ushort additionSession,uint additionID, ExtraUpdateModes mode)
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = src,
                Target = target,
                SkillSession = session,
                AdditionID = additionID,
                AdditionSession = additionSession,
                ExtraActivateMode = mode,
                UpdateType = UpdateTypes.Actor
            };
            return evt;
        }

        public static UpdateEvent NewActorAdditionExtEvent(ActorExt src, byte session, ushort additionSession, uint additionID, int restTime, ExtraUpdateModes mode)
        {
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = src,
                AdditionSession = additionSession,
                AdditionID = additionID,
                RestTime = restTime,
                SkillSession = session,
                ExtraActivateMode = mode,
                UpdateType = UpdateTypes.ActorExtension
            };
            return evt;
        }
    }
}
