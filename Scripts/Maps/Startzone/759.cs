
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC759 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 759; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {

        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor.ActorType == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                int distance = NPC.DistanceToActor(pc);
                if (distance < 150)
                {
                    if (pc.Quests.ContainsKey(243))
                    {
                        Quest q = pc.Quests[243];
                        if (q.Step == 0)
                        {
                            NextStep(q, 1, 1, 1, 1);
                            UpdateQuest(pc, q);
                        }
                    }
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q225.Start")
            {
                AI.Deactivate();
                AI.Pause = true;
                BeginTask();
                Move(-2115, 10695, 555, 244, 32);
                Move(-2146, 10690, 555, 190, 500);
                Move(-2177, 10685, 555, 190, 500);
                Move(-2181, 10684, 555, 190, 64);
                Move(-2183, 10683, 555, 204, 500, false);
                Move(-2185, 10682, 555, 204, 500, false);
                Move(-2188, 10681, 555, 204, 500, false);
                Move(-2191, 10680, 555, 204, 500, false);
                Move(-2194, 10679, 555, 204, 500, false);
                Move(-2195, 10678, 555, 204, 166, false);
                NPCChat(1560, 204);
                StartTask();
            }
            if (command == "q225.End")
            {
                AI.DueTime = 25000;
                AI.Activate();
                AI.Pause = false;
                BeginTask();
                Delay(180);
                Move(-2194, 10677, 555, 315, 16);
                Move(-2165, 10686, 555, 17, 500);
                Move(-2135, 10695, 555, 17, 500);
                Move(-2127, 10698, 555, 17, 129);
                Move(-2124, 10698, 555, 347, 500, false);
                Move(-2121, 10698, 555, 347, 500, false);
                Move(-2118, 10697, 555, 347, 500, false);
                Move(-2115, 10696, 555, 347, 500, false);
                Move(-2114, 10695, 555, 347, 166, false);
                Move(-2114, 10695, 555, 357, 0);
                NPCChat(2337, 357);
                StartTask();
            }
        }
    }
}
