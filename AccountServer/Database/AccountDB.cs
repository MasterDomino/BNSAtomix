using System;
using System.Collections.Generic;
using System.Data;

using SmartEngine.Network.Database;
using SmartEngine.Network.Database.Cache;

using SagaBNS.Common.Account;
using SagaBNS.Common.Packets.AccountServer;

namespace SagaBNS.AccountServer.Database
{
    public class AccountDB : MySQLConnectivity, ICacheDBHandler<uint, Account>
    {
        private static readonly AccountDB instance = new AccountDB();

        public static AccountDB Instance { get { return instance; } }

        public AccountDB()
            : base()
        {
        }

        public AccountLoginResult GetAccountID(string username, out uint id)
        {
            string sqlstr = string.Format("SELECT `password`,`account_id` FROM `account` WHERE `username`='{0}' LIMIT 1;", CheckSQLString(username));
            if (SQLExecuteQuery(sqlstr, out DataRowCollection results))
            {
                if (results.Count > 0)
                {
                    string pass2 = (string)results[0]["password"];
                    id = (uint)results[0]["account_id"];
                    return AccountLoginResult.OK;
                }
                else
                {
                    id = uint.MaxValue;
                    return AccountLoginResult.NO_SUCH_ACCOUNT;
                }
            }
            else
            {
                id = uint.MaxValue;
                return AccountLoginResult.DB_ERROR;
            }
        }

        #region ICacheDBHandler<uint,Account> 成员

        public Account LoadData(uint key)
        {
            string sqlstr = string.Format("SELECT * FROM `account` WHERE `account_id` = '{0}' LIMIT 1;", key);
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    Account account = new Account()
                    {
                        AccountID = (uint)result[0]["account_id"],
                        UserName = (string)result[0]["username"],
                        Password = (string)result[0]["password"],
                        GMLevel = (byte)result[0]["gm_lv"],
                        ExtraSlots = (byte)result[0]["extra_slot"],
                        LastLoginTime = (DateTime)result[0]["last_login_time"],
                        LastLoginIP = (string)result[0]["last_login_ip"],
                        LoginToken = Guid.NewGuid(),
                        TokenExpireTime = new DateTime(1970, 1, 1)
                    };
                    return account;
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

        public ICacheDBSaveResult CreateData(uint key, Account data)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            return Connected;
        }

        public uint GetMaxID<T>()
        {
            string sqlstr = "SELECT `account_id` FROM `account` ORDER BY `account_id` DESC LIMIT 1";
            if (SQLExecuteQuery(sqlstr, out DataRowCollection result))
            {
                if (result.Count > 0)
                {
                    return (uint)result[0]["account_id"];
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

        #endregion

        #region ICacheDBHandler<uint,Account> 成员

        public void BeginSaveData(ref List<Account> data)
        {
            throw new NotImplementedException();
        }

        public void SaveData(ref List<Account> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            List<uint> sList = new List<uint>();
            List<uint> fList = new List<uint>();
            success = sList;
            fails = fList;
            foreach (Account i in data)
            {
                //不保存用户名和密码
                string sqlstr = string.Format("UPDATE `account` SET `gm_lv`='{0}'," +
                    "`last_login_time`='{1}',`last_login_ip`='{2}',`extra_slot`='{3}' WHERE `account_id`='{4}' LIMIT 1;",
                    i.GMLevel, ToSQLDateTime(i.LastLoginTime), i.LastLoginIP, i.ExtraSlots,i.AccountID);
                if (SQLExecuteNonQuery(sqlstr))
                {
                    sList.Add(i.AccountID);
                }
                else
                {
                    fList.Add(i.AccountID);
                }
            }
        }

        public void BeginCreateData(uint key, Account data)
        {
            throw new NotImplementedException();
        }

        public void BeginDeleteData(ref List<Account> data)
        {
            throw new NotImplementedException();
        }

        public void DeleteData(ref List<Account> data, out IEnumerable<uint> success, out IEnumerable<uint> fails)
        {
            List<uint> sList = new List<uint>();
            List<uint> fList = new List<uint>();
            success = sList;
            fails = fList;
            foreach (Account i in data)
            {
                string sqlstr = string.Format("DELETE FROM `account` WHERE `account_id`='{0}' LIMIT 1;", i.AccountID);
                if (SQLExecuteNonQuery(sqlstr))
                {
                    sList.Add(i.AccountID);
                }
                else
                {
                    fList.Add(i.AccountID);
                }
            }
        }

        public event SaveResultCallbackHandler<uint, Account> SaveResultCallback;

        public ICacheDBExecuteType ExecuteType
        {
            get { return ICacheDBExecuteType.Sync; }
        }

        #endregion
    }
}
