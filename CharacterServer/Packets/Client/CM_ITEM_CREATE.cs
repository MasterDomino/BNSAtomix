
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_ITEM_CREATE : Common.Packets.CharacterServer.CM_ITEM_CREATE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_ITEM_CREATE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnItemCreate(this);
        }
    }
}
