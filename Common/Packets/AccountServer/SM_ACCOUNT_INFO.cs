﻿using System;

using SmartEngine.Network;
using SagaBNS.Common.Network;
namespace SagaBNS.Common.Packets.AccountServer
{
    public class SM_ACCOUNT_INFO : Packet<AccountPacketOpcode>
    {
        internal class SM_ACCOUNT_INFO_INTERNAL<T> : SM_ACCOUNT_INFO
        {
            public override Packet<AccountPacketOpcode> New()
            {
                return new SM_ACCOUNT_INFO_INTERNAL<T>();
            }

            public override void OnProcess(Session<AccountPacketOpcode> client)
            {
                ((AccountSession<T>)client).OnAccountInfo(this);
            }
        }

        public SM_ACCOUNT_INFO()
        {
            ID = AccountPacketOpcode.SM_ACCOUNT_INFO;
        }

        public long SessionID
        {
            get
            {
                return GetLong(2);
            }
            set
            {
                PutLong(value, 2);
            }
        }

        public AccountLoginResult Result
        {
            get
            {
                return (AccountLoginResult)GetByte(10);
            }
            set
            {
                PutByte((byte)value, 10);
            }
        }

        public Account.Account Account
        {
            get
            {
                Common.Account.Account account = new Account.Account()
                {
                    AccountID = GetUInt(11),
                    UserName = GetString(),
                    Password = GetString(),
                    GMLevel = GetByte(),
                    ExtraSlots = GetByte(),
                    LastLoginIP = GetString(),
                    LastLoginTime = DateTime.FromBinary(GetLong()),
                    LoginToken = new Guid(GetBytes(16)),
                    TokenExpireTime = DateTime.FromBinary(GetLong())
                };
                return account;
            }
            set
            {
                PutUInt(value.AccountID, 11);
                PutString(value.UserName);
                PutString(value.Password);
                PutByte(value.GMLevel);
                PutByte(value.ExtraSlots);
                PutString(value.LastLoginIP);
                PutLong(value.LastLoginTime.ToBinary());
                PutBytes(value.LoginToken.ToByteArray());
                PutLong(value.TokenExpireTime.ToBinary());
            }
        }
    }
}
