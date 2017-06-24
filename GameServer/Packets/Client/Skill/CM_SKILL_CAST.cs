
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_SKILL_CAST : Packet<GamePacketOpcode>
    {
        public CM_SKILL_CAST()
        {
            ID = GamePacketOpcode.CM_SKILL_CAST;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_SKILL_CAST();
        }

        public uint SkillID
        {
            get
            {
                return GetUInt(3);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(8);
            }
        }

        public ushort Dir
        {
            get
            {
                return GetUShort(16);
            }
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnSkillCast(this);
        }
    }
}
