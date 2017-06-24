
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.CharacterServer;

namespace SagaBNS.LoginServer.Packets.CharacterServer
{
    public class SM_ITEM_INVENTORY_ITEM : Common.Packets.CharacterServer.SM_ITEM_INVENTORY_ITEM
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new SM_ITEM_INVENTORY_ITEM();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnItemInventoryItem(this);
        }
    }
}
