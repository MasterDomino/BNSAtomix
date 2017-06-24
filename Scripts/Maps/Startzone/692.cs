
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC692 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 692; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 223:
                    {
                        switch (step)
                        {
                            case 2:
                                {
                                    SendNPCCommand("q223.Start");
                                    //BeginTask();
                                    //Delay(10);
                                    //NPCChat(1597, 300);
                                   // StartTask();
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor.ActorType == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                if (NPC.DistanceToActor(pc) < 100)
                {
                    if (pc.Quests.ContainsKey(223))
                    {
                        Quest q = pc.Quests[223];
                        if (q.Step == 0)
                        {
                            NextStep(q, 0, 1, 1, 1);
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
            if (command == "q224.Start2")
            {
                BeginTask();
                Delay(58);
                NPCChat(1552, 135);
                Delay(60);
                NPCChat(1555, 135);
                StartTask();
            }
        }
    }
}
