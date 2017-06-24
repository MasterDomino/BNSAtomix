
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_DROP : Packet<GamePacketOpcode>
    {
        public SM_QUEST_DROP()
        {
            ID = GamePacketOpcode.SM_QUEST_DROP;
        }

        public ushort Quest
        {
            set
            {
                PutUShort(value,2);
            }
        }
    }
}
