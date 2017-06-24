using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Map;

namespace Scripts.Maps.BambooVillageNight
{
    public class MapObject2000_29 : MapObjectScriptHandler
    {
        public override void OnOperate(ActorPC pc, Map map)
        {
            Utils.SpawnNPC(map, 256, 0, -7838, -7859, -280, 270, 0);
            Utils.SpawnNPC(map, 256, 0, -7015, -7475, -264, 111, 0);
            Utils.SpawnNPC(map, 256, 0, -7514, -8704, -298, 223, 0);
        }

        public override uint MapID
        {
            get { return 2000; }
        }

        public override uint ObjectID
        {
            get { return 29; }
        }
    }
}
