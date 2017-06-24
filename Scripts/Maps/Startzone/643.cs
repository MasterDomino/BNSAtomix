
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC643 : NPCScriptHandler
    {
        private ScriptTaskExecutor exe;
        private int q223State;
        public override ushort NpcID
        {
            get { return 643; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 221:
                    if (step == 4)
                    {
                        BeginTask();
                        NPCChat(1543, 200);
                        Delay(3);
                        Move(-3167, 9273, 583, 0, 64);
                        Move(-3142, 9292, 582, 38, 500);
                        Move(-3119, 9311, 582, 38, 500);
                        Move(-3107, 9321, 582, 38, 241);
                        NPCChat(1571, 38);
                        Move(-3103, 9320, 582, 346, 64);
                        Move(-3076, 9306, 582, 333, 500);
                        Move(-3054, 9294, 582, 333, 403);
                        Move(-3035, 9270, 582, 310, 500);
                        Move(-3024, 9257, 582, 310, 274);
                        Move(-3030, 9252, 582, 220, 437, false);
                        exe = StartTask();
                    }
                    break;
                case 222:
                    if (step == 4)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1545, 33);
                        Move(-3148, 9808, 471, 26, 32);
                        Move(-3129, 9832, 471, 51, 500);
                        Move(-3110, 9856, 470, 51, 500);
                        Move(-3104, 9863, 468, 51, 145);
                        Move(-3075, 9872, 473, 16, 500);
                        Move(-3046, 9881, 481, 16, 500);
                        Move(-3017, 9889, 486, 16, 500);
                        Move(-2988, 9898, 491, 16, 500);
                        Move(-2959, 9907, 493, 16, 500);
                        Move(-2930, 9916, 492, 16, 500);
                        Move(-2907, 9923, 494, 16, 387);
                        Move(-2885, 9902, 496, 316, 500);
                        Move(-2863, 9881, 498, 316, 500);
                        Move(-2841, 9860, 500, 316, 500);
                        Move(-2819, 9839, 503, 316, 500);
                        Move(-2797, 9818, 509, 316, 500);
                        Move(-2775, 9796, 517, 316, 500);
                        Move(-2753, 9774, 521, 316, 500);
                        Move(-2744, 9765, 525, 316, 193);
                        Move(-2714, 9769, 530, 7, 500);
                        Move(-2683, 9773, 534, 7, 500);
                        Move(-2652, 9777, 536, 7, 500);
                        Move(-2627, 9781, 537, 7, 403);
                        Move(-2622, 9811, 537, 79, 500);
                        Move(-2616, 9842, 537, 79, 500);
                        Move(-2614, 9849, 537, 79, 112);
                        Move(-2618, 9856, 537, 119, 500, false);
                        Move(-2622, 9863, 537, 119, 500, false);
                        StartTask();
                    }
                    break;
            }
        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor is ActorPC target)
            {
                if (exe?.Activated == false)
                {
                    if (target.Quests.ContainsKey(221))
                    {
                        Quest q = target.Quests[221];
                        if (q.Step == 4)
                        {
                            if (NPC.DistanceToPoint2D(arg.X, arg.Y) < 50)
                            {
                                NextStep(q, 0, 31, 31, 31);
                                UpdateQuest(target, q);

                                BeginTask();
                                Delay(10);
                                Move(-3031, 9250, 582, 244, 125, false);
                                Move(-3036, 9245, 583, 225, 500, false);
                                Move(-3042, 9239, 583, 225, 500, false);
                                Move(-3045, 9236, 583, 225, 250, false);
                                StartTask();
                                target = null;
                            }
                        }
                        return;
                    }
                    if (target.Quests.ContainsKey(222))
                    {
                        Quest q = target.Quests[222];
                        if (q.Step == 0)
                        {
                            if (NPC.DistanceToPoint2D(arg.X, arg.Y) < 100)
                            {
                                NextStep(q, 0, 1, 1, 1);
                                UpdateQuest(target, q);
                            }
                        }
                    }
                    if (target.Quests.ContainsKey(223))
                    {
                        Quest q = target.Quests[223];
                        if (q.Step == 2)
                        {
                            if (NPC.DistanceToPoint2D(arg.X, arg.Y) < 100)
                            {
                                switch (q223State)
                                {
                                    case 0:
                                        BeginTask();
                                        //NPCChat(1604, 292);
                                        NPCChat(1570, 119);
                                        Delay(15);
                                        Dash(-2495, 9344, 553, 180, 1397, 32);
                                        Delay(35);
                                        Dash(-2208, 8577, 552, 291, 0, 0);
                                        exe = StartTask();
                                        q223State = 1;
                                        break;
                                    case 1:
                                        BeginTask();
                                        NPCChat(1548, 291);
                                        Delay(5);
                                        Move(-2206, 8564, 552, 279, 209);
                                        Move(-2206, 8533, 550, 269, 500);
                                        Move(-2207, 8502, 557, 269, 500);
                                        Move(-2208, 8471, 567, 269, 500);
                                        Move(-2209, 8464, 568, 269, 112);
                                        Move(-2219, 8435, 586, 250, 500);
                                        Move(-2230, 8406, 600, 250, 500);
                                        Move(-2241, 8377, 611, 250, 500);
                                        Move(-2252, 8348, 622, 250, 500);
                                        Move(-2263, 8319, 632, 250, 500);
                                        Move(-2266, 8311, 634, 250, 129);
                                        Move(-2248, 8286, 641, 306, 500);
                                        Move(-2230, 8261, 645, 306, 500);
                                        Move(-2227, 8256, 645, 306, 80);
                                        Move(-2196, 8262, 646, 11, 500);
                                        Move(-2177, 8266, 649, 11, 306);
                                        Move(-2159, 8291, 650, 53, 500);
                                        Move(-2141, 8316, 655, 53, 500);
                                        Move(-2123, 8341, 657, 53, 500);
                                        Move(-2105, 8366, 658, 53, 500);
                                        Move(-2087, 8391, 657, 53, 500);
                                        Move(-2068, 8416, 657, 53, 500);
                                        Move(-2055, 8433, 658, 53, 338);
                                        exe = StartTask();
                                        q223State = 2;
                                        break;
                                    case 2:
                                        {
                                            CutScene(target, 46);
                                            NextStep(q, 0, 7, 7, 7);
                                            UpdateQuest(target, q);
                                            BeginTask();
                                            Move(-2050, 8439, 658, 47, 500, false);
                                            Move(-2044, 8445, 658, 47, 500, false);
                                            exe = StartTask();
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            switch (command)
            {
                case "q222.Start":
                    {
                        BeginTask();
                        Delay(10);
                        Move(-3045, 9234, 583, 270, 32);
                        Move(-3023, 9255, 582, 44, 500);
                        Move(-3001, 9276, 582, 44, 500);
                        Move(-2979, 9297, 582, 44, 500);
                        Move(-2957, 9318, 582, 44, 500);
                        Move(-2935, 9339, 580, 44, 500);
                        Move(-2913, 9360, 569, 44, 500);
                        Move(-2891, 9381, 558, 44, 500);
                        Move(-2869, 9402, 547, 44, 500);
                        Move(-2848, 9423, 544, 44, 500);
                        Move(-2826, 9445, 544, 44, 500);
                        Move(-2806, 9465, 544, 44, 451);
                        Move(-2800, 9495, 544, 77, 500);
                        Move(-2794, 9525, 544, 77, 500);
                        Move(-2787, 9555, 544, 77, 500);
                        Move(-2781, 9585, 541, 77, 500);
                        Move(-2778, 9594, 540, 77, 145);
                        Move(-2784, 9624, 536, 102, 500);
                        Move(-2791, 9654, 533, 102, 500);
                        Move(-2798, 9684, 528, 102, 500);
                        Move(-2805, 9714, 523, 102, 500);
                        Move(-2812, 9744, 516, 102, 500);
                        Move(-2819, 9774, 511, 102, 500);
                        Move(-2826, 9804, 506, 102, 500);
                        Move(-2833, 9834, 502, 102, 500);
                        Move(-2838, 9855, 501, 102, 338);
                        Move(-2857, 9879, 498, 129, 500);
                        Move(-2876, 9903, 497, 129, 500);
                        Move(-2895, 9925, 496, 129, 467);
                        Move(-2924, 9917, 493, 197, 500);
                        Move(-2953, 9909, 493, 197, 500);
                        Move(-2982, 9900, 492, 197, 500);
                        Move(-3011, 9892, 488, 197, 500);
                        Move(-3040, 9883, 483, 197, 500);
                        Move(-3069, 9874, 475, 197, 500);
                        Move(-3098, 9865, 468, 197, 500);
                        Move(-3118, 9859, 470, 197, 322);
                        Move(-3136, 9834, 471, 234, 500);
                        Move(-3154, 9809, 471, 234, 500);
                        Move(-3162, 9799, 471, 234, 193);
                        Move(-3156, 9803, 471, 33, 500, false);
                        Move(-3150, 9807, 471, 33, 437, false);
                        exe = StartTask();
                    }
                    break;
                case "q223.Start":
                    BeginTask();
                    NPCChat(1546, 119);
                    Delay(35);
                    Move(-2622, 9862, 537, 270, 16);
                    Move(-2626, 9832, 537, 262, 500);
                    Move(-2630, 9802, 537, 262, 500);
                    Move(-2635, 9772, 537, 262, 500);
                    Move(-2640, 9741, 536, 262, 500);
                    Move(-2643, 9725, 535, 262, 258);
                    NPCChat(1547, 262);
                    Delay(30);
                    Dash(-2494, 9344, 553, 292, 1396, 12);
                    exe = StartTask();
                    break;
                case "q224.Start":
                    BeginTask();
                    Delay(35);
                    NPCChat(1550, 47);
                    Delay(30);
                    Dash(-2048, 8448, 658, 143, 1398, 90);
                    Delay(90);
                    Dash(-2548, 9394, 555, 117, 0, 0);
                    Dash(-2535, 9396, 555, 8, 0, 0);
                    Delay(65);
                    NPCChat(1564, 8);
                    Move(-2528, 9398, 556, 15, 112);
                    Move(-2509, 9422, 558, 51, 500);
                    Move(-2490, 9446, 562, 51, 500);
                    Move(-2471, 9470, 564, 51, 500);
                    Move(-2452, 9494, 567, 51, 500);
                    Move(-2433, 9518, 569, 51, 500);
                    Move(-2414, 9542, 571, 51, 500);
                    Move(-2395, 9566, 573, 51, 500);
                    Move(-2376, 9590, 575, 51, 500);
                    Move(-2357, 9614, 577, 51, 500);
                    Move(-2338, 9638, 580, 51, 500);
                    Move(-2319, 9662, 585, 51, 500);
                    Move(-2300, 9686, 587, 51, 500);
                    Move(-2281, 9710, 588, 51, 500);
                    Move(-2262, 9734, 588, 51, 500);
                    Move(-2243, 9758, 588, 51, 500);
                    Move(-2226, 9780, 588, 51, 435);
                    Move(-2222, 9774, 588, 304, 500, false);
                    Move(-2218, 9767, 588, 304, 500, false);
                    Move(-2216, 9765, 588, 304, 125, false);
                    exe = StartTask();
                    break;
                case "q224.Start2":
                    BeginTask();
                    Delay(59);
                    NPCChat(1566, 135);
                    Delay(59);
                    NPCChat(1569, 135);
                    StartTask();
                    break;
            }
        }
    }
}
