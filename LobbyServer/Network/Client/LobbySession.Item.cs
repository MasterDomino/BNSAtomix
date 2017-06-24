using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Actors;
using SagaBNS.LobbyServer.Network.CharacterServer;

namespace SagaBNS.LobbyServer.Network.Client
{
    public partial class LobbySession : Session<LobbyPacketOpcode>
    {
        private void LoadInventory()
        {
            if (currentCharIndex < chars.Count)
            {
                CharacterSession.Instance.GetInventory(chars.Values.ToList()[currentCharIndex].CharID, this);
            }
            else
            {
                SendCharacterList();
            }
        }

        public void OnGotInventoryItem(Common.Item.Item item, bool end)
        {
            if (item != null)
            {
                List<ActorPC> list = chars.Values.ToList();
                if (list.Count > currentCharIndex)
                {
                    if (item.SlotID == 255)
                    {
                        lock (list[currentCharIndex].Inventory.SoldItems)
                        {
                            list[currentCharIndex].Inventory.SoldItems.Add(item);
                        }
                    }
                    else
                    {
                        list[currentCharIndex].Inventory.Container[item.Container][item.SlotID] = item;
                        /*if (item.InventoryEquipSlot != Common.Inventory.InventoryEquipSlot.None)
                            list[currentCharIndex].Inventory.Equipments[item.InventoryEquipSlot] = item;*/
                    }
                }
            }
            if (end)
            {
                currentCharIndex++;
                LoadInventory();
            }
        }
    }
}
