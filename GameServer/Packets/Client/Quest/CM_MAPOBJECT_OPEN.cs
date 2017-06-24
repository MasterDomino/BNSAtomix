
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAPOBJECT_OPEN : Packet<GamePacketOpcode>
    {
        public CM_MAPOBJECT_OPEN()
        {
            ID = GamePacketOpcode.CM_MAPOBJECT_OPEN;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public ushort Unknown
        {
            get
            {
                return GetUShort(10);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAPOBJECT_OPEN();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapObjectOpen(this);
        }
    }
}
