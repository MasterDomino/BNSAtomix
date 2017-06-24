using System;

using SmartEngine.Network;
namespace SagaBNS.Common.Packets.AccountServer
{
    public class CM_ACCOUNT_SAVE : Packet<AccountPacketOpcode>
    {
        public CM_ACCOUNT_SAVE()
        {
            ID = AccountPacketOpcode.CM_ACCOUNT_SAVE;
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

        public Account.Account Account
        {
            get
            {
                Common.Account.Account account = new Account.Account()
                {
                    AccountID = GetUInt(10),
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
                PutUInt(value.AccountID,10);
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
