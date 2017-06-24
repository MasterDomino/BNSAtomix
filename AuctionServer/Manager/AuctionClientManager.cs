using SagaBNS.AuctionServer.Network.Client;
using SagaBNS.AuctionServer.Packets.Client;
using SagaBNS.Common.Packets;
using SmartEngine.Network;

namespace SagaBNS.AuctionServer.Manager
{
    internal class AuctionClientManager : ClientManager<AuctionPacketOpcode>
    {
        #region Instantiation

        public AuctionClientManager()
        {
            RegisterPacketHandler(AuctionPacketOpcode.CM_AUTH, new CM_AUTH());
        }

        #endregion

        #region Properties

        public static AuctionClientManager Instance { get; } = new AuctionClientManager();

        #endregion

        #region Methods

        protected override Session<AuctionPacketOpcode> NewSession()
        {
            return new AuctionSession();
        }

        #endregion
    }
}