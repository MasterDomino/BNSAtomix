
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_SKILL_ADD : Packet<GamePacketOpcode>
    {
        public SM_SKILL_ADD()
        {
            ID = GamePacketOpcode.SM_SKILL_ADD;
        }

        public uint SkillID
        {
            set
            {
                PutUInt(value, 2);
                PutByte(0);
            }
        }
    }
}
