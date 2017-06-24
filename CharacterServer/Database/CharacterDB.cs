using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using SmartEngine.Network.Utils;
using SmartEngine.Network.Database;
using SmartEngine.Network.Database.Cache;

using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;

namespace SagaBNS.CharacterServer.Database
{
    internal class CharacterDB: MySQLConnectivity, ICacheDBHandler<uint, ActorPC>
    {
        private static readonly CharacterDB instance = new CharacterDB();

        public static CharacterDB Instance { get { return instance; } }

        public CharacterDB()
            : base()
        {
        }

        public List<uint> GetCharIDs(uint accountID)
        {
            string sqlstr = string.Format("SELECT `char_id` FROM `char` WHERE `account_id`='{0}' LIMIT 5;", accountID);
            List<uint> res = new List<uint>();
            if (SQLExecuteQuery(sqlstr, out DataRowCollection results))
            {
                foreach (DataRow i in results)
                {
                    res.Add((uint)i["char_id"]);
                }
            }
            return res;
        }

        public bool CheckExists(string name, byte worldID)
        {
            string sqlstr = string.Format("SELECT * FROM `char` WHERE `name` = '{0}' AND `world_id` = '{1}' LIMIT 1;", name, worldID);
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        #region ICacheDBHandler<uint,ActorPC> 成员

        public ActorPC LoadData(uint key)
        {
            string sqlstr = string.Format("SELECT * FROM `char` WHERE `char_id` = '{0}' LIMIT 1;", key);
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    ActorPC pc = new ActorPC()
                    {
                        CharID = (uint)result[0]["char_id"],
                        AccountID = (uint)result[0]["account_id"],
                        Name = (string)result[0]["name"],
                        SlotID = (byte)result[0]["slot_id"],
                        WorldID = (byte)result[0]["world_id"],
                        Race = (Race)(byte)result[0]["race"],
                        Gender = (Gender)(byte)result[0]["gender"],
                        Job = (Job)(byte)result[0]["job"],
                        Level = (byte)result[0]["level"],
                        Appearence1 = Conversions.HexStr2Bytes((string)result[0]["appearence"]),
                        Appearence2 = Conversions.HexStr2Bytes((string)result[0]["appearence2"]),
                        UISettings = (string)result[0]["ui_settings"]
                    };
                    if (pc.UISettings == null)
                    {
                        pc.UISettings = string.Empty;
                    }

                    pc.Exp = (uint)result[0]["exp"];
                    pc.HP = (ushort)result[0]["hp"];
                    pc.MP = (ushort)result[0]["mp"];
                    pc.MaxHP = (ushort)result[0]["max_hp"];
                    pc.MaxMP = (ushort)result[0]["max_mp"];
                    pc.Gold = (int)(uint)result[0]["gold"];
                    pc.MapID = (uint)result[0]["map_id"];
                    pc.X = (short)result[0]["x"];
                    pc.Y = (short)result[0]["y"];
                    pc.Z = (short)result[0]["z"];
                    pc.Dir = (ushort)result[0]["dir"];
                    pc.InventorySize = (byte)result[0]["inventory_size"];

                    LoadQuest(pc);
                    LoadSkill(pc);
                    LoadTeleport(pc);
                    return pc;
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

        private void LoadQuest(ActorPC pc)
        {
            string sqlstr = string.Format("SELECT * FROM `quest` WHERE `char_id` = '{0}';", pc.CharID);
            DataRowCollection result;
            lock (pc.Quests)
            {
                pc.Quests.Clear();
                if (SQLExecuteQuery(sqlstr, out result))
                {
                    for (int i = 0; result.Count > i; i++)
                    {
                        Common.Quests.Quest q = new Common.Quests.Quest()
                        {
                            QuestID = (ushort)result[i]["quest_id"],
                            Step = (byte)result[i]["step"],
                            StepStatus = (byte)result[i]["step_status"],
                            NextStep = (byte)result[i]["next_step"],
                            Flag1 = (short)result[i]["flag1"],
                            Flag2 = (short)result[i]["flag2"],
                            Flag3 = (short)result[i]["flag3"]
                        };
                        q.Count[0] = (int)result[i]["count1"];
                        q.Count[1] = (int)result[i]["count2"];
                        q.Count[2] = (int)result[i]["count3"];
                        q.Count[3] = (int)result[i]["count4"];
                        q.Count[4] = (int)result[i]["count5"];
                        pc.Quests[q.QuestID] = q;
                    }
                }
            }

            sqlstr = string.Format("SELECT * FROM `quest_completed` WHERE `char_id` = '{0}';", pc.CharID);
            if (SQLExecuteQuery(sqlstr, out result))
            {
                for (int i = 0; result.Count > i;i++ )
                {
                    pc.QuestsCompleted.Add((ushort)result[i]["quest_id"]);
                }
            }
        }

        private void LoadSkill(ActorPC pc)
        {
            string sqlstr = string.Format("SELECT * FROM `skills` WHERE `char_id` = '{0}';", pc.CharID);
            lock (pc.Skills)
            {
                if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
                {
                    for (int i = 0; result.Count > i; i++)
                    {
                        SkillData data = new SkillData()
                        {
                            ID = (uint)result[i]["skill_id"]
                        };
                        pc.Skills[data.ID] = new Skill(data);
                    }
                }
            }
        }

        private void LoadTeleport(ActorPC pc)
        {
            string sqlstr = string.Format("SELECT * FROM `teleport_locations` WHERE `char_id` = '{0}';", pc.CharID);
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                for (int i = 0; result.Count > i; i++)
                {
                    pc.Locations.Add((ushort)result[i]["teleport_id"]);
                }
            }
        }

        private string SaveQuest(ActorPC pc)
        {
            string cmd = string.Format("DELETE FROM `quest` WHERE `char_id`='{0}';DELETE FROM `quest_completed` WHERE `char_id`='{0}';", pc.CharID);
            lock (pc.Quests)
            {
                foreach (Common.Quests.Quest q in pc.Quests.Values.ToArray())
                {
                    cmd += string.Format("INSERT INTO `quest` (`quest_id`,`char_id`,`step`,`step_status`,`next_step`," +
                        "`flag1`,`flag2`,`flag3`,`count1`,`count2`,`count3`,`count4`,`count5`) " +
                        "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');",
                        q.QuestID, pc.CharID, q.Step, q.StepStatus, q.NextStep, q.Flag1, q.Flag2, q.Flag3,
                        q.Count[0], q.Count[1], q.Count[2], q.Count[3], q.Count[4]);
                }
                foreach (ushort i in pc.QuestsCompleted)
                {
                    cmd += string.Format("INSERT INTO `quest_completed`(`quest_id`,`char_id`) VALUES('{0}','{1}');",
                        i, pc.CharID);
                }
            }

            return cmd;
        }

        private string SaveSkill(ActorPC pc)
        {
            string cmd = string.Format("DELETE FROM `skills` WHERE `char_id`='{0}';", pc.CharID);
            lock (pc.Skills)
            {
                foreach (uint i in pc.Skills.Keys)
                {
                    cmd += string.Format("INSERT INTO `skills`(`skill_id`,`char_id`) VALUES('{0}','{1}');",
                        i, pc.CharID);
                }
            }
            return cmd;
        }

        private string SaveTeleport(ActorPC pc)
        {
            string cmd = string.Format("DELETE FROM `teleport_locations` WHERE `char_id`='{0}';", pc.CharID);
            foreach (ushort i in pc.Locations)
            {
                cmd += string.Format("INSERT INTO `teleport_locations`(`teleport_id`,`char_id`) VALUES('{0}','{1}');", i , pc.CharID);
            }
            return cmd;
        }

        public ICacheDBSaveResult CreateData(uint key, ActorPC data)
        {
            bool retVal = true;
            data.CharID = key;
            string cmd = string.Format("INSERT INTO `char` (`char_id`,`account_id`,`name` ,`slot_id`,`world_id` ,`race`,`gender`,`job`," +
                "`appearence`,`appearence2`,`level`,`exp`,`hp`,`mp`,`max_hp`,`max_mp`,`gold`,`map_id`,`x`,`y`,`z`,`dir`,`inventory_size`" +
                ") VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}'" +
                ",'{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}');",
                key, data.AccountID, data.Name, data.SlotID, data.WorldID, (byte)data.Race, (byte)data.Gender, (byte)data.Job,
                Conversions.bytes2HexString(data.Appearence1), Conversions.bytes2HexString(data.Appearence2), data.Level,
                data.Exp, data.HP, data.MP, data.MaxHP, data.MaxMP, data.Gold, data.MapID, data.X, data.Y, data.Z, data.Dir,data.InventorySize);
            cmd += SaveQuest(data);
            cmd += SaveSkill(data);
            cmd += SaveTeleport(data);
            retVal = retVal && SQLExecuteNonQuery(cmd);
            return retVal ? ICacheDBSaveResult.OK : ICacheDBSaveResult.Fail;
        }

        public bool IsConnected()
        {
            return Connected;
        }

        public uint GetMaxID<T>()
        {
            string sqlstr = "SELECT `char_id` FROM `char` ORDER BY `char_id` DESC LIMIT 1";
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    return (uint)result[0]["char_id"];
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

        public void BeginSaveData(ref List<ActorPC> data)
        {
            throw new NotImplementedException();
        }

        public void SaveData(ref List<ActorPC> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            List<uint> sList = new List<uint>();
            List<uint> fList = new List<uint>();
            success = sList;
            fails = fList;
            foreach (ActorPC i in data)
            {
                string sqlstr = string.Empty;
                //不保存用户名和密码
                sqlstr += string.Format("UPDATE `char` SET `name`='{0}',`account_id`='{9}',`slot_id`='{1}',`race`='{2}',`gender`='{3}'," +
                    "`job`='{4}',`appearence`='{5}',`appearence2`='{6}',`level`='{7}',`exp`='{10}',`hp`='{11}',`mp`='{12}',`max_hp`='{13}'," +
                    "`max_mp`='{14}',`gold`='{15}',`map_id`='{16}',`x`='{17}',`y`='{18}',`z`='{19}',`dir`='{20}',`ui_settings`='{21}', `inventory_size`='{22}' WHERE `char_id`='{8}' LIMIT 1;",
                    CheckSQLString(i.Name), i.SlotID, (byte)i.Race, (byte)i.Gender, (byte)i.Job,
                    Conversions.bytes2HexString(i.Appearence1), Conversions.bytes2HexString(i.Appearence2), i.Level,
                    i.CharID, i.AccountID, i.Exp, i.HP, i.MP, i.MaxHP, i.MaxMP, i.Gold, i.MapID, i.X, i.Y, i.Z, i.Dir,i.UISettings,i.InventorySize);
                sqlstr += SaveQuest(i);
                sqlstr += SaveSkill(i);
                sqlstr += SaveTeleport(i);
                if (!SQLExecuteNonQuery(sqlstr))
                {
                    fList.Add(i.CharID);
                }
                else
                {
                    sList.Add(i.CharID);
                }
            }
        }

        public void BeginCreateData(uint key, ActorPC data)
        {
            throw new NotImplementedException();
        }

        public void BeginDeleteData(ref List<ActorPC> data)
        {
            throw new NotImplementedException();
        }

        public void DeleteData(ref List<ActorPC> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            List<uint> sList = new List<uint>();
            List<uint> fList = new List<uint>();
            success = sList;
            fails = fList;
            foreach (ActorPC i in data)
            {
                string sqlstr = string.Format("DELETE FROM `char` WHERE `char_id`='{0}' LIMIT 1;", i.CharID);
                if (SQLExecuteNonQuery(sqlstr))
                {
                    sList.Add(i.CharID);
                }
                else
                {
                    fList.Add(i.CharID);
                }
            }
        }

        public event SaveResultCallbackHandler<uint, ActorPC> SaveResultCallback;

        public ICacheDBExecuteType ExecuteType
        {
            get { return ICacheDBExecuteType.Sync; }
        }

        #endregion
    }
}
