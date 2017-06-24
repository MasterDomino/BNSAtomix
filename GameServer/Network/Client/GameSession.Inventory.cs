using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Packets.Client;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public void OnExpandInventory()
        {
            int upgradecost = UpgradeCost(chara.InventorySize);
            if (chara.Gold >= upgradecost && upgradecost != -1)
            {
                SM_EXPAND_INVENTORY r = new SM_EXPAND_INVENTORY();
                Network.SendPacket(r);
                chara.InventorySize += 8;
                chara.Gold -= upgradecost;
                SM_PLAYER_UPDATE_LIST p = new SM_PLAYER_UPDATE_LIST();
                UpdateEvent evt = new UpdateEvent();
                evt.AddActorPara(PacketParameter.Gold,chara.Gold);
                evt.AddActorPara(PacketParameter.InventorySlots, chara.InventorySize);
                p.Parameters = evt;
                Network.SendPacket(p);
            }
        }

        private int UpgradeCost(byte inventory)
        {
            switch (inventory)
            {
                case 32:
                    return 500;
                case 40:
                    return 1000;
                case 48:
                    return 6500;
                case 56:
                    return 12500;
                case 64:
                    return 62500;
                case 72:
                    return 500000;
                default:
                    return -1;
            }
        }
    }
}
