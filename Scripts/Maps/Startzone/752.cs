using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC752 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 752; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {

        }

        public override void OnCreate(bool success)
        {
            if (success)
            {
                if (NPC.DistanceToPoint2D(-3086, 9085) < 100)
                {
                    BeginTask();
                    Delay(20);
                    Move(-3085, 9091, 583, 80, 96);
                    Move(-3082, 9115, 583, 82, 387);
                    Move(-3098, 9141, 583, 121, 500);
                    Move(-3110, 9160, 583, 121, 354);
                    Move(-3113, 9160, 583, 164, 500, false);
                    Move(-3116, 9161, 583, 164, 500, false);
                    Move(-3119, 9162, 583, 164, 500, false);
                    Move(-3121, 9163, 583, 164, 333, false);
                    StartTask();
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q222.Start")
            {
                BeginTask();
                Move(-3120, 9164, 583, 45, 16);
                Move(-3099, 9186, 583, 45, 500);
                Move(-3077, 9208, 583, 45, 500);
                Move(-3055, 9230, 583, 45, 500);
                Move(-3033, 9252, 583, 45, 500);
                Move(-3030, 9255, 582, 45, 64);
                Move(-3008, 9276, 582, 44, 500);
                Move(-2986, 9297, 582, 44, 500);
                Move(-2964, 9318, 582, 44, 500);
                Move(-2942, 9339, 580, 44, 500);
                Move(-2920, 9360, 569, 44, 500);
                Move(-2898, 9381, 560, 44, 500);
                Move(-2876, 9402, 547, 44, 500);
                Move(-2855, 9423, 544, 44, 500);
                Move(-2833, 9445, 544, 44, 500);
                Move(-2818, 9460, 544, 44, 338);
                Move(-2811, 9490, 544, 75, 500);
                Move(-2803, 9520, 544, 75, 500);
                Move(-2795, 9550, 544, 75, 500);
                Move(-2787, 9580, 541, 75, 500);
                Move(-2783, 9595, 540, 75, 241);
                Move(-2790, 9625, 536, 103, 500);
                Move(-2797, 9655, 533, 103, 500);
                Move(-2804, 9685, 528, 103, 500);
                Move(-2811, 9715, 523, 103, 500);
                Move(-2818, 9745, 515, 103, 500);
                Move(-2825, 9775, 510, 103, 500);
                Move(-2833, 9805, 505, 103, 500);
                Move(-2841, 9835, 501, 103, 500);
                Move(-2846, 9854, 500, 103, 306);
                Move(-2866, 9878, 497, 129, 500);
                Move(-2886, 9902, 496, 129, 500);
                Move(-2901, 9920, 495, 129, 370);
                Move(-2930, 9911, 492, 199, 500);
                Move(-2959, 9902, 493, 199, 500);
                Move(-2988, 9892, 491, 199, 500);
                Move(-3017, 9882, 486, 199, 500);
                Move(-3046, 9872, 481, 199, 500);
                Move(-3075, 9862, 471, 199, 500);
                Move(-3104, 9852, 469, 199, 500);
                Move(-3113, 9849, 470, 199, 145);
                Move(-3133, 9826, 471, 230, 500);
                Move(-3152, 9803, 471, 230, 500);
                Move(-3172, 9780, 471, 230, 500);
                Move(-3192, 9756, 471, 230, 500);
                Move(-3212, 9732, 471, 230, 500);
                Move(-3232, 9708, 471, 230, 500);
                Move(-3238, 9701, 471, 230, 145);
                Move(-3236, 9703, 471, 48, 500, false);
                Move(-3234, 9705, 471, 48, 500, false);
                Move(-3233, 9707, 471, 48, 500, false);
                Move(-3231, 9709, 471, 48, 333, false);
                StartTask();
            }
            if (command == "q224.Start")
            {
                Disappear();
            }
            if (command == "q224.Start2")
            {
                BeginTask();
                Delay(58);
                NPCChat(1568, 300);
                StartTask();
            }
        }
    }
}
