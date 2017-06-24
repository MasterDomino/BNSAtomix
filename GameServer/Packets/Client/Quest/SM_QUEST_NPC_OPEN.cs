
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_QUEST_NPC_OPEN : Packet<GamePacketOpcode>
    {
        public SM_QUEST_NPC_OPEN()
        {
            ID = GamePacketOpcode.SM_QUEST_NPC_OPEN;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }
    }
}
