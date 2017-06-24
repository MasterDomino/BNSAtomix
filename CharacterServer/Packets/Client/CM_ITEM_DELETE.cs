
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.CharacterServer.Network.Client;

namespace SagaBNS.CharacterServer.Packets.Client
{
    public class CM_ITEM_DELETE : Common.Packets.CharacterServer.CM_ITEM_DELETE
    {
        public override Packet<CharacterPacketOpcode> New()
        {
            return new CM_ITEM_DELETE();
        }

        public override void OnProcess(Session<CharacterPacketOpcode> client)
        {
            ((CharacterSession)client).OnItemDelete(this);
        }
    }
}
