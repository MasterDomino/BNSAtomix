
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Party;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_PARTY_CHANGE_LOOT_MODE : Packet<GamePacketOpcode>
    {
        public CM_PARTY_CHANGE_LOOT_MODE()
        {
            ID = GamePacketOpcode.CM_PARTY_CHANGE_LOOT_MODE;
        }

        public PartyLootMode LootMode
        {
            get
            {
                return (PartyLootMode)GetByte(2);
            }
        }

        public byte ItemRank
        {
            get
            {
                return GetByte(3);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(4);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_PARTY_CHANGE_LOOT_MODE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnPartyChangeLootMode(this);
        }
    }
}
