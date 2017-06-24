
using SmartEngine.Network;
using SagaBNS.Common.Party;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_LOOT_MODE_CHANGED: Packet<GamePacketOpcode>
    {
        public SM_PARTY_LOOT_MODE_CHANGED()
        {
            ID = GamePacketOpcode.SM_PARTY_LOOT_MODE_CHANGED;
        }

        public PartyLootMode LootMode
        {
            set
            {
                PutByte((byte)value, 2);
            }
        }

        public byte ItemRank
        {
            set
            {
                PutByte(value, 3);
            }
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 4);
            }
        }
    }
}
