
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_SAVE_UI_SETTING : Packet<GamePacketOpcode>
    {
        public CM_SAVE_UI_SETTING()
        {
            ID = GamePacketOpcode.CM_SAVE_UI_SETTING;
        }

        public string Settings
        {
            get
            {
                return GetString(2);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_SAVE_UI_SETTING();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).SaveSettings(this);
        }
    }
}
