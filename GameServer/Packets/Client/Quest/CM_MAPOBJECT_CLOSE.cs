
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAPOBJECT_CLOSE : Packet<GamePacketOpcode>
    {
        public CM_MAPOBJECT_CLOSE()
        {
            ID = GamePacketOpcode.CM_MAPOBJECT_CLOSE;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAPOBJECT_CLOSE();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapObjectClose(this);
        }
    }
}
