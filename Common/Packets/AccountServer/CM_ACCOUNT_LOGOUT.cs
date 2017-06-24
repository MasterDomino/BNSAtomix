﻿
using SmartEngine.Network;
namespace SagaBNS.Common.Packets.AccountServer
{
    public class CM_ACCOUNT_LOGOUT : Packet<AccountPacketOpcode>
    {
        public CM_ACCOUNT_LOGOUT()
        {
            ID = AccountPacketOpcode.CM_ACCOUNT_LOGOUT;
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

        public uint AccountID
        {
            get
            {
                return GetUInt(10);
            }
            set
            {
                PutUInt(value, 10);
            }
        }
    }
}
