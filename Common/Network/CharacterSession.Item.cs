using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.Common.Network
{
    public abstract partial class CharacterSession<T> : DefaultClient<CharacterPacketOpcode>
    {
        private readonly ConcurrentDictionary<long, Common.Item.Item> createItems = new ConcurrentDictionary<long, Common.Item.Item>();
        public void OnItemCreateResult(SM_ITEM_CREATE_RESULT p)
        {
            long session = p.SessionID;
            if (createItems.ContainsKey(session))
            {
                createItems[session].ID = p.ItemID;
                createItems.TryRemove(session, out Item.Item removed);
            }
        }

        public void CreateItem(Common.Item.Item item)
        {
            long session = Global.PacketSession;
            createItems[session] = item;

            CM_ITEM_CREATE p = new CM_ITEM_CREATE()
            {
                SessionID = session,
                Item = item
            };
            Network.SendPacket(p);
        }

        public void SaveInventory(Inventory.Inventory inv)
        {
            foreach (Common.Item.Item i in inv.Container[Item.Containers.Inventory])
            {
                if (i != null)
                {
                    SaveItem(i);
                }
            }
            foreach (Common.Item.Item i in inv.Container[Item.Containers.Equipment])
            {
                if (i != null)
                {
                    SaveItem(i);
                }
            }
            foreach (Common.Item.Item i in inv.Container[Item.Containers.Warehouse])
            {
                if (i != null)
                {
                    SaveItem(i);
                }
            }
            SaveItem(inv.SoldItems);
        }

        public void SaveItem(Common.Item.Item item)
        {
            CM_ITEM_SAVE p = new CM_ITEM_SAVE()
            {
                Item = item
            };
            Network.SendPacket(p);
        }

        public void SaveItem(List<Common.Item.Item> items)
        {
            CM_ITEM_LIST_SAVE p = new CM_ITEM_LIST_SAVE()
            {
                Items = items
            };
            Network.SendPacket(p);
        }

        public void DeleteItem(List<Common.Item.Item> items)
        {
            var ids = from item in items
                      select item.ID;
            CM_ITEM_DELETE p = new CM_ITEM_DELETE()
            {
                ItemIDs = ids.ToList()
            };
            Network.SendPacket(p);
        }

        public void GetInventory(uint charID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_ITEM_INVENTORY_GET p = new CM_ITEM_INVENTORY_GET()
            {
                SessionID = session,
                CharID = charID
            };
            Network.SendPacket(p);
        }

        public void OnItemInventoryItem(SM_ITEM_INVENTORY_ITEM p)
        {
            long session = p.SessionID;
            if (packetSessions.TryGetValue(session, out T client))
            {
                OnGotInventoryItem(client, p.Item, p.End);
                if (p.End)
                {
                    packetSessions.TryRemove(session, out client);
                }
            }
        }

        protected abstract void OnGotInventoryItem(T client, Item.Item item, bool end);
    }
}
