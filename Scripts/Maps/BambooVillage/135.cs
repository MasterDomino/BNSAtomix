using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC135 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 135; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 302:
                    if (step == 4)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1603, pc);
                        Delay(7);
                        Move(-13564, -12671, -754, 135, 16);
                        Move(-13574, -12698, -754, 250, 451);
                        Move(-13583, -12727, -754, 252, 500);
                        Move(-13590, -12747, -758, 252, 338);
                        Move(-13595, -12778, -759, 260, 500);
                        Move(-13596, -12781, -759, 260, 48);
                        Move(-13626, -12776, -762, 170, 500);
                        Move(-13656, -12771, -763, 170, 500);
                        Move(-13687, -12766, -764, 170, 500);
                        StartTask();
                    }
                    break;
            }
        }

        public override void OnCreate(bool success)
        {
            if (success)
            {
                if (NPC.DistanceToPoint2D(-13721, -12768) < 100)
                {
                    BeginTask();
                    Delay(40);
                    Move(-13723, -12755, -764, 98, 209);
                    Move(-13693, -12759, -764, 353, 500);
                    Move(-13662, -12763, -763, 353, 500);
                    Move(-13631, -12767, -762, 353, 500);
                    Move(-13608, -12771, -760, 353, 370);
                    Move(-13589, -12747, -758, 52, 500);
                    Move(-13574, -12727, -754, 52, 403);
                    Move(-13568, -12697, -754, 78, 500);
                    Move(-13563, -12672, -754, 78, 403);
                    StartTask();
                }
            }
        }
    }
}
