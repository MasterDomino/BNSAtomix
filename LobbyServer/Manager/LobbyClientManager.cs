using SagaBNS.Common.Packets;
using SagaBNS.LobbyServer.Network.Client;
using SagaBNS.LobbyServer.Packets.Client;
using SmartEngine.Network;

namespace SagaBNS.LobbyServer.Manager
{
    public class LobbyClientManager : ClientManager<LobbyPacketOpcode>
    {
        #region Instantiation

        public LobbyClientManager()
        {
            RegisterPacketHandler(LobbyPacketOpcode.CM_AUTH, new CM_AUTH());
            RegisterPacketHandler(LobbyPacketOpcode.CM_SERVER_LIST, new CM_SERVER_LIST());
            RegisterPacketHandler(LobbyPacketOpcode.CM_CHARACTER_LIST, new CM_CHARACTER_LIST());
            RegisterPacketHandler(LobbyPacketOpcode.CM_CHAR_CREATE, new CM_CHAR_CREATE());
            RegisterPacketHandler(LobbyPacketOpcode.CM_REQUEST_LOGIN, new CM_REQUEST_LOGIN());
            RegisterPacketHandler(LobbyPacketOpcode.CM_CHAR_DELETE, new CM_CHAR_DELETE());
        }

        #endregion

        #region Properties

        public static LobbyClientManager Instance { get; } = new LobbyClientManager();

        #endregion

        #region Methods

        protected override Session<LobbyPacketOpcode> NewSession()
        {
            return new LobbySession();
        }

        #endregion
    }
}