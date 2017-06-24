
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAP_PORTAL_CANCEL : Packet<GamePacketOpcode>
    {
        public SM_MAP_PORTAL_CANCEL()
        {
            ID = GamePacketOpcode.SM_MAP_PORTAL_CANCEL;

            PutUShort(0x200);
        }
    }
}
