
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_EXPAND_INVENTORY : Packet<GamePacketOpcode>
    {
        public CM_EXPAND_INVENTORY()
        {
            ID = GamePacketOpcode.CM_EXPAND_INVENTORY;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_EXPAND_INVENTORY();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnExpandInventory();
        }
    }
}
