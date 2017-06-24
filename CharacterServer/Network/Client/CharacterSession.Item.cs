using System.Collections.Generic;

using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.CharacterServer.Database;
using SagaBNS.CharacterServer.Cache;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public void OnItemCreate(Packets.Client.CM_ITEM_CREATE p)
        {
            SM_ITEM_CREATE_RESULT p1 = new SM_ITEM_CREATE_RESULT()
            {
                SessionID = p.SessionID,
                Result = SM_ITEM_CREATE_RESULT.Results.OK,
                ItemID = ItemCache.Instance.Create(p.Item)
            };
            Network.SendPacket(p1);
        }

        public void OnItemSave(Packets.Client.CM_ITEM_SAVE p)
        {
            Common.Item.Item item = p.Item;
            ItemCache.Instance.Save(item.ID, item);
        }

        public void OnItemListSave(Packets.Client.CM_ITEM_LIST_SAVE p)
        {
            foreach (Common.Item.Item i in p.Items)
            {
                ItemCache.Instance.Save(i.ID, i);
            }
        }

        public void OnItemDelete(Packets.Client.CM_ITEM_DELETE p)
        {
            foreach (uint i in p.ItemIDs)
            {
                ItemCache.Instance.Delete(i);
            }
        }

        public void OnItemInventoryGet(Packets.Client.CM_ITEM_INVENTORY_GET p)
        {
            long session = p.SessionID;
            List<uint> ids = ItemDB.Instance.GetItemIDs(p.CharID);
            foreach (uint i in ItemCache.Instance.GetItemIDsForChar(p.CharID))
            {
                if (!ids.Contains(i))
                {
                    ids.Add(i);
                }
            }
            if (ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    SM_ITEM_INVENTORY_ITEM p1 = new SM_ITEM_INVENTORY_ITEM()
                    {
                        SessionID = session,
                        End = i == (ids.Count - 1),
                        Item = ItemCache.Instance[ids[i]]
                    };
                    Network.SendPacket(p1);
                }
            }
            else
            {
                SM_ITEM_INVENTORY_ITEM p1 = new SM_ITEM_INVENTORY_ITEM()
                {
                    SessionID = session,
                    End = true,
                    Item = null
                };
                Network.SendPacket(p1);
            }
        }
    }
}
