
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_SKILL_CAST_RESULT : Packet<GamePacketOpcode>
    {
        public SM_SKILL_CAST_RESULT()
        {
            ID = GamePacketOpcode.SM_SKILL_CAST_RESULT;
        }

        public byte SkillSession
        {
            set
            {
                PutByte(value, 2);
            }
        }

        public ushort Unknown
        {
            set
            {
                PutUShort(value, 3);
            }
        }

        public uint SkillID
        {
            set
            {
                PutUInt(value, 5);
                PutByte(0);
            }
        }
    }
}
