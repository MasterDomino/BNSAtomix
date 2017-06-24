
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_DECOMPOSE_BAGUA : Packet<GamePacketOpcode>
    {
        public CM_DECOMPOSE_BAGUA()
        {
            ID = GamePacketOpcode.CM_DECOMPOSE_BAGUA;
        }

        public ushort ItemSlot
        {
            get
            {
                return GetUShort(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_DECOMPOSE_BAGUA();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).DecomposeBaGua(this);
        }
    }
}
