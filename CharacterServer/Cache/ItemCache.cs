using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network.Database.Cache;
using SagaBNS.Common.Item;
using SagaBNS.CharacterServer.Database;

namespace SagaBNS.CharacterServer.Cache
{
    public class ItemCache : Cache<uint, Item>
    {
        private static readonly ItemCache instance = new ItemCache();

        public static ItemCache Instance { get { return instance; } }

        public ItemCache()
            : base(ItemDB.Instance)
        {
            capacity = 100000;
            EldestRemoveCount = 10000;
            MaxSaveRetryTimes = 0;
        }

        protected override uint IncraseIdentity(uint oriKey)
        {
            return oriKey + 1;
        }

        public List<uint> GetItemIDsForChar(uint charID)
        {
            var res = from i in Find((item) =>
            {
                return item.CharID == charID;
            })
                      select i.ID;
            return res.ToList();
        }
    }
}
