
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_EXPAND_INVENTORY : Packet<GamePacketOpcode>
    {
        public SM_EXPAND_INVENTORY()
        {
            ID = GamePacketOpcode.SM_EXPAND_INVENTORY;
        }
    }
}
