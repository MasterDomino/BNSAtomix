
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_LOAD_INVENTORY : Packet<GamePacketOpcode>
    {
        public CM_ITEM_LOAD_INVENTORY()
        {
            ID = GamePacketOpcode.CM_ITEM_LOAD_INVENTORY;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_LOAD_INVENTORY();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).LoadInventory(this);
        }
    }
}
