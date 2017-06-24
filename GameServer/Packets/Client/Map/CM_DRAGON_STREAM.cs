
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_DRAGON_STREAM : Packet<GamePacketOpcode>
    {
        public CM_DRAGON_STREAM()
        {
            ID = GamePacketOpcode.CM_DRAGON_STREAM;
        }

        public ulong StreamID
        {
            get
            {
                return GetULong(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_DRAGON_STREAM();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnMapEnterStream(this);
        }
    }
}
