using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC106 : NPCScriptHandler
    {
        private ScriptTaskExecutor exe;
        public override ushort NpcID
        {
            get { return 106; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
          Disappear(2339);
        }

        public override void OnCreate(bool success)
        {
          BeginTask();
          Delay(10);
          Dash(-8498,-7410,-163,275,1394,97);
          exe = StartTask();
        }
    }
}
