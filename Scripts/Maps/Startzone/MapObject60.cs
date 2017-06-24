using SagaBNS.Common.Quests;
using SagaBNS.Common.Actors;
using SagaBNS.GameServer.Scripting;
using SagaBNS.GameServer.Map;

namespace Scripts.Maps.Startzone
{
    public class MapObject60 : MapObjectScriptHandler
    {
        public override void OnOperate(ActorPC pc, Map map)
        {
            if (pc.Quests.ContainsKey(221))
            {
                Quest q = pc.Quests[221];
                if (q.Step == 6)
                {
                    SpawnNPC(map, 751, -3159, 9169, 583, 340, 374);
                    SpawnNPC(map, 752, -3086, 9085, 588, 45, 374);
                }
            }
        }

        public override uint MapID
        {
            get { return 1101; }
        }

        public override uint ObjectID
        {
            get { return 60; }
        }
    }
}
