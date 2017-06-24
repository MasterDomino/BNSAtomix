
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_MAPOBJECT_GET_ITEM : Packet<GamePacketOpcode>
    {
        public CM_MAPOBJECT_GET_ITEM()
        {
            ID = GamePacketOpcode.CM_MAPOBJECT_GET_ITEM;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_MAPOBJECT_GET_ITEM();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapObjectGetItem(this);
        }
    }
}
