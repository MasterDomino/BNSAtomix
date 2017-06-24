
using SmartEngine.Network;
using SagaBNS.Common.Packets;

namespace SagaBNS.GameServer.Packets.Client
{
    public class CM_ADD_WEAPON_GEM_SLOT : Packet<GamePacketOpcode>
    {
        public CM_ADD_WEAPON_GEM_SLOT()
        {
            ID = GamePacketOpcode.CM_ADD_WEAPON_GEM_SLOT;
        }

        public ushort WeaponSlotID
        {
            get
            {
                return GetUShort(2);
            }
        }

        public ushort TalismanSlotID
        {
            get
            {
                return GetUShort(4);
            }
        }

        public override Packet<GamePacketOpcode> New()
        {
            return new CM_ADD_WEAPON_GEM_SLOT();
        }

        public override void OnProcess(Session<GamePacketOpcode> client)
        {
            //((GameSession)client).OnAddGemSlot(this);
        }
    }
}
