using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network.Database.Cache;
using SagaBNS.Common.Actors;
using SagaBNS.CharacterServer.Database;

namespace SagaBNS.CharacterServer.Cache
{
    public class CharacterCache : Cache<uint, ActorPC>
    {
        private static readonly CharacterCache instance = new CharacterCache();

        public static CharacterCache Instance { get { return instance; } }

        public CharacterCache()
            : base(CharacterDB.Instance)
        {
            capacity = 5000;
            EldestRemoveCount = 500;
            MaxSaveRetryTimes = 0;
        }

        protected override uint IncraseIdentity(uint oriKey)
        {
            return oriKey + 1;
        }

        public List<uint> GetCharIDsForAccount(uint accountID)
        {
            var res = from i in Find((pc) =>
             {
                 return pc.AccountID == accountID;
             })
                      select i.CharID;
            return res.ToList();
        }
    }
}
