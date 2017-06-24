using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Map;

namespace Scripts.Maps.BambooVillageNight
{
    public class MapObject2300_3 : MapObjectScriptHandler
    {
        public override void OnOperate(ActorPC pc, Map map)
        {
            if (pc.Quests.ContainsKey(382))
            {
                Utils.SpawnNPC(map, 226, 898, (short)pc.X, (short)pc.Y, (short)pc.Z, pc.Dir, 0);
            }
        }

        public override uint MapID
        {
            get { return 2300; }
        }

        public override uint ObjectID
        {
            get { return 3; }
        }
    }
}
