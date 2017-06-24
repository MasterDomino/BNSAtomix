
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.Client;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ITEM_BUYBACK : Packet<GamePacketOpcode>
    {
        public CM_ITEM_BUYBACK()
        {
            ID = GamePacketOpcode.CM_ITEM_BUYBACK;
        }

        public ulong ActorID
        {
            get
            {
                return GetULong(2);
            }
        }

        public byte Slot
        {
            get
            {
                return (byte)(GetByte(10) - 1);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ITEM_BUYBACK();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            ((GameSession)client).OnItemBuyBack(this);
        }
    }
}
