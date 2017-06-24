using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC642 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 642; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 223:
                    if (step == 4)
                    {
                        SendNPCCommand("q224.Start");
                        SpawnNPC(751, -2224, 9754, 588, 300, 374);
                        SpawnNPC(752, -2240, 9743, 588, 300, 374);
                        SpawnNPC(692, -2194, 9741, 588, 135, 376);
                        SpawnNPC(642, -2202, 9779, 588, 300, 367);
                        SpawnNPC(636, -2203, 9727, 588, 135, 375);
                        BeginTask();
                        Delay(5);
                        NPCChat(1549, 230);
                        Delay(45);
                        Dash(-2017, 8478, 658, 135, 1415, 130);
                        Delay(130);
                        Dash(-1012, 9987, 714, 56, 0, 0);
                        StartTask();
                    }
                    break;
                case 227:
                case 230:
                case 236:
                case 239:
                    {
                        if (step == 7)
                        {
                            BeginTask();
                            NPCChat(2368, pc);
                            Delay(30);
                            Dash(-5329, 11035, 253, 256, 1414, 130);
                            StartTask();
                        }
                    }
                    break;
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q224.Start2")
            {
                BeginTask();
                Delay(50);
                NPCChat(1565, 300);
                StartTask();
            }
        }
    }
}
