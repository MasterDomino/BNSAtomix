
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ADD_CRAFT : Packet<GamePacketOpcode>
    {
        public CM_ADD_CRAFT()
        {
            ID = GamePacketOpcode.CM_ADD_CRAFT;
        }

        public byte Craft
        {
            get
            {
                return GetByte(2);
            }
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(3);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ADD_CRAFT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnAddCraft(this);
        }
    }
}
