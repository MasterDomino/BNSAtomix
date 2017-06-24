
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC636 : NPCScriptHandler
    {
        private ScriptTaskExecutor exe;
        public override ushort NpcID
        {
            get { return 636; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 224:
                    if (step == 2)
                    {
                        SendNPCCommand("q224.Start2");
                        BeginTask();
                        Delay(5);
                        NPCChat(1551, 135);
                        Delay(82);
                        Dash(-2206, 9728, 588, 161, 1419, 20);
                        Delay(20);
                        Dash(-1801, 9415, 685, 323, 0, 0);
                        Move(-1804, 9417, 685, 140, 500, false);
                        Move(-1807, 9420, 685, 140, 500, false);
                        exe = StartTask();
                    }
                    break;
            }
        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor.ActorType == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                int distance = NPC.DistanceToActor(pc);
                if (distance < 150)
                {
                    if (pc.Quests.ContainsKey(224))
                    {
                        Quest q = pc.Quests[224];
                        if (q.Step == 0)
                        {
                            NextStep(q, 0, 1, 1, 1);
                            UpdateQuest(pc, q);
                        }
                        if (q.Step == 2 && distance < 100 && exe != null && !exe.Activated)
                        {
                            NextStep(q, 0, 7, 7, 7);
                            UpdateQuest(pc, q);
                        }
                    }
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q224.Start")
            {
                Disappear();
            }
        }
    }
}
