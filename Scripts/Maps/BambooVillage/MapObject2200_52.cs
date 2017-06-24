using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Map;

namespace Scripts.Maps.BambooVillageNight
{
    public class MapObject2200_52 : MapObjectScriptHandler
    {
        public override void OnOperate(ActorPC pc, Map map)
        {
            if (pc.Quests.ContainsKey(311))
            {
                Utils.SpawnNPC(map, 853, 1462, -1435, -4832, -37, 316, 0);
            }
        }

        public override uint MapID
        {
            get { return 2200; }
        }

        public override uint ObjectID
        {
            get { return 52; }
        }
    }
}
