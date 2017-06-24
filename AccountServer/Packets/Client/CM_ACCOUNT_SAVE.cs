
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.AccountServer.Network.Client;

namespace SagaBNS.AccountServer.Packets.Client
{
    public class CM_ACCOUNT_SAVE : Common.Packets.AccountServer.CM_ACCOUNT_SAVE
    {
        public override Packet<AccountPacketOpcode> New()
        {
            return new CM_ACCOUNT_SAVE();
        }

        public override void OnProcess(Session<AccountPacketOpcode> client)
        {
            ((AccountSession)client).OnAccountSave(this);
        }
    }
}
