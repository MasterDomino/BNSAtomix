
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_SKILL_CAST_COORDINATE : Packet<GamePacketOpcode>
    {
        public CM_SKILL_CAST_COORDINATE()
        {
            ID = GamePacketOpcode.CM_SKILL_CAST_COORDINATE;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_SKILL_CAST_COORDINATE();
        }

        public uint SkillID
        {
            get
            {
                return GetUInt(2);
            }
        }

        public short X
        {
            get
            {
                return GetShort(7);
            }
        }

        public short Y
        {
            get
            {
                return GetShort(9);
            }
        }

        public short Z
        {
            get
            {
                return GetShort(11);
            }
        }

        public ushort Dir
        {
            get
            {
                return GetUShort(13);
            }
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnSkillCastCoordinate(this);
        }
    }
}
