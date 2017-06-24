
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC203 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 203; }
        }

        public override void OnActorAppears(Actor aActor)
        {
            base.OnActorAppears(aActor);
            if (aActor.ActorType == ActorType.PC)
            {
                BeginTask();
                Delay(50);
                NPCChat(1614, 230);
                StartTask();
            }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 303:
                    if (step == 3)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1616, 230);
                        StartTask();
                    }
                    break;
            }
        }
    }
}
