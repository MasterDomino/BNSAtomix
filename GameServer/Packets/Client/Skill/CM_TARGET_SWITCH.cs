
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_TARGET_SWITCH : Packet<GamePacketOpcode>
    {
        public CM_TARGET_SWITCH()
        {
            ID = GamePacketOpcode.CM_TARGET_SWITCH;
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_TARGET_SWITCH();
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnTargetSwitch(this);
        }
    }
}
