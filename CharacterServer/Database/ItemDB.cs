using System;
using System.Collections.Generic;
using System.Data;
using SmartEngine.Network.Database;
using SmartEngine.Network.Database.Cache;
using SagaBNS.Common.Item;

namespace SagaBNS.CharacterServer.Database
{
    public class ItemDB : MySQLConnectivity, ICacheDBHandler<uint, Item>
    {
        private static readonly ItemDB instance = new ItemDB();

        public static ItemDB Instance { get { return instance; } }

        public ItemDB()
            : base()
        {
        }

        public List<uint> GetItemIDs(uint charID)
        {
            string sqlstr = string.Format("SELECT `id` FROM `item` WHERE `char_id`='{0}' LIMIT 200;", charID);
            List<uint> res = new List<uint>();
            if (SQLExecuteQuery(sqlstr, out DataRowCollection results))
            {
                foreach (DataRow i in results)
                {
                    res.Add((uint)i["id"]);
                }
            }
            return res;
        }

        #region ICacheDBHandler<uint,Item> 成员

        public Item LoadData(uint key)
        {
            string sqlstr = string.Format("SELECT * FROM `item` WHERE `id` = '{0}' LIMIT 1;", key);
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    uint itemID = (uint)result[0]["item_id"];
                    ItemData data = new ItemData()
                    {
                        ItemID = itemID
                    };
                    Item item = new Item(data)
                    {
                        ID = (uint)result[0]["id"],
                        CharID = (uint)result[0]["char_id"],
                        SlotID = (ushort)result[0]["slot_id"],
                        Container = (Common.Item.Containers)(byte)result[0]["container"],
                        Count = (ushort)result[0]["count"],
                        Synthesis = (byte)result[0]["synthesis"]
                    };
                    return item;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ICacheDBSaveResult CreateData(uint key, Item data)
        {
            bool retVal = true;
            data.ID = key;
            string cmd = string.Format("INSERT INTO `item` (`id`,`char_id`,`item_id` ,`slot_id` ,`container`,`count`,`synthesis`" +
                ") VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');",
                key, data.CharID, data.ItemID, data.SlotID, (byte)data.Container, data.Count,data.Synthesis);
            retVal = retVal && SQLExecuteNonQuery(cmd);
            return retVal ? ICacheDBSaveResult.OK : ICacheDBSaveResult.Fail;
        }

        public bool IsConnected()
        {
            return Connected;
        }

        public uint GetMaxID<T>()
        {
            string sqlstr = "SELECT `id` FROM `item` ORDER BY `id` DESC LIMIT 1";
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    return (uint)result[0]["id"];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public void BeginSaveData(ref List<Item> data)
        {
            throw new NotImplementedException();
        }

        public void SaveData(ref List<Item> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            int count = 0;
            //StringBuilder sqlstr = new StringBuilder();
            List<uint> successList = new List<uint>();
            List<uint> failList = new List<uint>();
            List<uint> ids = new List<uint>();
            success = successList;
            fails = failList;
            foreach (Item i in data)
            {
                string sqlstr = (string.Format("UPDATE `item` SET `item_id`='{0}',`char_id`='{1}',`slot_id`='{2}',`container`='{3}',`count`='{4}',`synthesis`='{5}'" +
                     " WHERE `id`='{6}' LIMIT 1;",
                    i.ItemID, i.CharID, i.SlotID, (byte)i.Container, i.Count,i.Synthesis, i.ID));
                if (!SQLExecuteNonQuery(sqlstr))
                {
                    failList.Add(i.ID);
                }
                else
                {
                    successList.Add(i.ID);
                }
            }
        }

        public void BeginCreateData(uint key, Item data)
        {
            throw new NotImplementedException();
        }

        public void BeginDeleteData(ref List<Item> data)
        {
            throw new NotImplementedException();
        }

        public void DeleteData(ref List<Item> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            List<uint> successList = new List<uint>();
            List<uint> failList = new List<uint>();
            success = successList;
            fails = failList;
            foreach (Item i in data)
            {
                string sqlstr;
                sqlstr = string.Format("DELETE FROM `item` WHERE `id`='{0}' LIMIT 1;", i.ID);
                if (!SQLExecuteNonQuery(sqlstr))
                {
                    failList.Add(i.ID);
                }
                else
                {
                    successList.Add(i.ID);
                }
            }
        }

        public event SaveResultCallbackHandler<uint, Item> SaveResultCallback;

        public ICacheDBExecuteType ExecuteType
        {
            get { return ICacheDBExecuteType.Sync; }
        }

        #endregion
    }
}
