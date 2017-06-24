using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC751 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 751; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            if (questID == 221 && step == 7)
            {
                SendNPCCommand("q222.Start");
                BeginTask();
                Delay(3);
                NPCChat(1544, 340);
                Move(-3156, 9172, 583, 45, 64);
                Move(-3127, 9182, 583, 18, 500);
                Move(-3098, 9192, 583, 18, 500);
                Move(-3089, 9195, 583, 18, 145);
                Move(-3067, 9216, 583, 43, 500);
                Move(-3045, 9237, 583, 43, 500);
                Move(-3023, 9258, 582, 43, 500);
                Move(-3001, 9279, 582, 43, 500);
                Move(-2979, 9300, 582, 43, 500);
                Move(-2957, 9321, 581, 43, 500);
                Move(-2935, 9342, 580, 43, 500);
                Move(-2913, 9363, 569, 43, 500);
                Move(-2891, 9384, 556, 43, 500);
                Move(-2869, 9405, 547, 43, 500);
                Move(-2847, 9426, 544, 43, 500);
                Move(-2825, 9447, 544, 43, 500);
                Move(-2811, 9461, 544, 43, 306);
                Move(-2802, 9490, 544, 72, 500);
                Move(-2793, 9519, 543, 72, 500);
                Move(-2784, 9548, 544, 72, 500);
                Move(-2775, 9577, 542, 72, 500);
                Move(-2769, 9595, 540, 72, 290);
                Move(-2776, 9625, 536, 103, 500);
                Move(-2783, 9655, 534, 103, 500);
                Move(-2790, 9685, 528, 103, 500);
                Move(-2797, 9715, 523, 103, 500);
                Move(-2804, 9745, 517, 103, 500);
                Move(-2811, 9775, 511, 103, 500);
                Move(-2818, 9805, 507, 103, 500);
                Move(-2825, 9835, 503, 103, 500);
                Move(-2830, 9854, 502, 103, 306);
                Move(-2851, 9877, 498, 132, 500);
                Move(-2872, 9900, 497, 132, 500);
                Move(-2893, 9923, 496, 132, 500);
                Move(-2902, 9933, 495, 132, 209);
                Move(-2932, 9925, 492, 195, 500);
                Move(-2962, 9917, 493, 195, 500);
                Move(-2992, 9909, 491, 195, 500);
                Move(-3022, 9901, 486, 195, 500);
                Move(-3052, 9893, 480, 195, 500);
                Move(-3082, 9885, 470, 195, 500);
                Move(-3112, 9877, 468, 195, 500);
                Move(-3142, 9869, 471, 195, 500);
                Move(-3172, 9861, 471, 195, 500);
                Move(-3202, 9853, 471, 195, 500);
                Move(-3232, 9845, 471, 195, 500);
                Move(-3243, 9842, 471, 195, 177);
                Move(-3236, 9843, 471, 11, 500, false);
                Move(-3233, 9844, 471, 11, 214, false);
                StartTask();
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
                Delay(55);
                NPCChat(1567, 300);
                Delay(40);
                NPCChat(1553, 300);
                StartTask();
            }
        }
    }
}
