using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Map;

namespace Scripts.Maps.BambooVillageNight
{
    public class MapObject2000_71 : MapObjectScriptHandler
    {
        public override void OnOperate(ActorPC pc, Map map)
        {
            if (pc.Quests.ContainsKey(356))
            {
                Utils.SpawnNPC(map, 106, 0, -8504, -7323, -161, 324, 0);
            }
        }

        public override uint MapID
        {
            get { return 2000; }
        }

        public override uint ObjectID
        {
            get { return 71; }
        }
    }
}
