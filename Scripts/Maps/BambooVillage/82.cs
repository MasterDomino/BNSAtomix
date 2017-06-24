using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC82 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 82; }
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
                if (NPC.DistanceToPoint2D(-12588, -13840) < 100)
                {
                    BeginTask();
                    Delay(20);
                    Move(-12586, -13848, -663, 285, 500, false);
                    Move(-12584, -13855, -661, 285, 437, false);
                    Move(-12582, -13862, -656, 287, 500, false);
                    Move(-12580, -13869, -653, 287, 500, false);
                    Move(-12578, -13876, -650, 287, 500, false);
                    Move(-12576, -13883, -647, 287, 500, false);
                    Move(-12574, -13890, -645, 287, 500, false);
                    Move(-12572, -13897, -645, 287, 500, false);
                    Move(-12570, -13904, -645, 287, 500, false);
                    Move(-12568, -13911, -645, 287, 500, false);
                    Move(-12566, -13919, -645, 287, 500, false);
                    Move(-12564, -13927, -645, 287, 500, false);
                    Move(-12562, -13930, -645, 287, 187, false);
                    Move(-12560, -13937, -645, 288, 500, false);
                    Move(-12558, -13944, -645, 288, 500, false);
                    Move(-12556, -13951, -645, 288, 500, false);
                    Move(-12554, -13958, -641, 288, 500, false);
                    Move(-12552, -13965, -636, 288, 500, false);
                    Move(-12550, -13972, -636, 288, 500, false);
                    Move(-12548, -13979, -636, 288, 500, false);
                    Move(-12546, -13986, -636, 288, 500, false);
                    Move(-12544, -13993, -636, 288, 500, false);
                    Move(-12541, -14000, -636, 288, 500, false);
                    Move(-12538, -14008, -636, 288, 500, false);
                    Move(-12531, -14010, -636, 344, 500, false);
                    Move(-12523, -14012, -636, 344, 500, false);
                    Move(-12515, -14015, -636, 344, 500, false);
                    Move(-12507, -14015, -636, 0, 500, false);
                    Move(-12499, -14015, -636, 0, 500, false);
                    Move(-12491, -14015, -636, 0, 500, false);
                    Move(-12483, -14015, -636, 0, 500, false);
                    Move(-12476, -14015, -636, 0, 437, false);
                    Move(-12470, -14011, -636, 36, 500, false);
                    Move(-12468, -14009, -636, 36, 125, false);
                    StartTask();
                }
            }
        }
    }
}
