namespace SagaBNS.ChatServer.Packets
{
    public enum BNSChatOpcodes
    {
        CM_CHANNEL_QUIT = 0x12,
        CM_CHAT = 0x18,
        SM_CHAT = 0x1A,
        CM_PING = 0xFF,
        CM_LOGIN_AUTH = 0x4005,
        SM_LOGIN_AUTH_RESULT = 0x4002,
        CM_PLAYER_CHANGE_CHANNEL = 0x4010,
        SM_PLAYER_CHANGE_CHANNEL_RESULT = 0x4011,
    }
}
