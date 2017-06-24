
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.LoginServer.Network.Client;
using SagaBNS.LoginServer.Packets.Client;

namespace SagaBNS.LoginServer.Manager
{
    public class LoginClientManager : ClientManager<LoginPacketOpcode>
    {
        private static readonly LoginClientManager instance = new LoginClientManager();

        public static LoginClientManager Instance { get { return instance; } }

        public LoginClientManager()
        {
            RegisterPacketHandler(LoginPacketOpcode.CM_STS_CONNECT, new Packet<LoginPacketOpcode>());
            RegisterPacketHandler(LoginPacketOpcode.CM_AUTH_LOGIN_START, new CM_AUTH_LOGIN_START());
            RegisterPacketHandler(LoginPacketOpcode.CM_AUTH_KEY_DATA, new CM_AUTH_KEY_DATA());
            RegisterPacketHandler(LoginPacketOpcode.CM_AUTH_LOGIN_FINISH, new CM_AUTH_LOGIN_FINISH());
            RegisterPacketHandler(LoginPacketOpcode.CM_AUTH_GAME_TOKEN, new CM_AUTH_GAME_TOKEN());
            RegisterPacketHandler(LoginPacketOpcode.CM_AUTH_TOKEN, new CM_AUTH_TOKEN());
            RegisterPacketHandler(LoginPacketOpcode.CM_ACCOUNT_LIST, new CM_ACCOUNT_LIST());
            RegisterPacketHandler(LoginPacketOpcode.CM_WORLD_LIST, new CM_WORLD_LIST());
            RegisterPacketHandler(LoginPacketOpcode.CM_CHAR_LIST, new CM_CHAR_LIST());
            RegisterPacketHandler(LoginPacketOpcode.CM_CHAR_SLOT_REQUEST, new CM_CHAR_SLOT_REQUEST());
            RegisterPacketHandler(LoginPacketOpcode.CM_CHAR_CREATE, new CM_CHAR_CREATE());
            RegisterPacketHandler(LoginPacketOpcode.CM_CHAR_DELETE, new CM_CHAR_DELETE());
            RegisterPacketHandler(LoginPacketOpcode.CM_SLOT_LIST, new CM_SLOT_LIST());
            RegisterPacketHandler(LoginPacketOpcode.CM_PING, new Packet<LoginPacketOpcode>());
        }

        protected override Session<LoginPacketOpcode> NewSession()
        {
            return new LoginSession();
        }
    }
}
