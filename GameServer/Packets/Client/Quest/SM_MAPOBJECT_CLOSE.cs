
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_MAPOBJECT_CLOSE : Packet<GamePacketOpcode>
    {
        public SM_MAPOBJECT_CLOSE()
        {
            ID = GamePacketOpcode.SM_MAPOBJECT_CLOSE;
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
