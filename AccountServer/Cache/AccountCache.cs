
using SmartEngine.Network.Database.Cache;
using SagaBNS.Common.Account;
using SagaBNS.AccountServer.Database;

namespace SagaBNS.AccountServer.Cache
{
    public class AccountCache : Cache<uint, Account>
    {
        private static readonly AccountCache instance = new AccountCache();

        public static AccountCache Instance { get { return instance; } }

        public AccountCache()
            : base(AccountDB.Instance)
        {

        }

        protected override uint IncraseIdentity(uint oriKey)
        {
            return oriKey + 1;
        }
    }
}
