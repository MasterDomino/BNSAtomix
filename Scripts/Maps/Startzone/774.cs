
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC774 : NPCScriptHandler
    {
        private ScriptTaskExecutor exe;
        private int q227_step;
        public override ushort NpcID
        {
            get { return 774; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                #region 225
                case 225:
                case 228:
                case 234:
                case 237:
                    if (step == 1)
                    {
                        SendNPCCommand("q225.Start");
                        BeginTask();
                        Delay(5);
                        NPCChat(1573, 250);
                        StartTask();
                    }
                    if (step == 2)
                    {
                        SendNPCCommand("q225.End");
                        BeginTask();
                        NPCChat(1556, pc);
                        Delay(40);
                        NPCChat(1574, pc);
                        Delay(42);
                        Move(-2253, 10645, 557, 14, 64);
                        Move(-2243, 10674, 557, 70, 500);
                        Move(-2233, 10703, 557, 70, 500);
                        Move(-2223, 10732, 555, 70, 500);
                        Move(-2213, 10761, 555, 70, 500);
                        Move(-2210, 10768, 555, 70, 112);
                        Move(-2201, 10797, 555, 72, 500);
                        Move(-2192, 10826, 555, 72, 500);
                        Move(-2183, 10855, 555, 72, 500);
                        Move(-2174, 10884, 555, 72, 500);
                        Move(-2165, 10913, 555, 72, 500);
                        Move(-2156, 10942, 555, 72, 500);
                        Move(-2146, 10971, 555, 72, 500);
                        Move(-2136, 11000, 555, 72, 500);
                        Move(-2126, 11029, 555, 72, 500);
                        Move(-2116, 11058, 555, 72, 500);
                        Move(-2112, 11070, 555, 72, 193);
                        Move(-2115, 11061, 555, 249, 500, false);
                        Move(-2117, 11057, 555, 249, 222, false);
                        exe = StartTask();
                    }
                    break;
                #endregion

                #region 226
                case 226:
                case 229:
                case 238:
                    {
                        if (step == 1)
                        {
                            BeginTask();
                            SpawnNPCTask(672, 1542, -2198, 11005, 555, 45, 367);
                            Delay(3);
                            SpawnNPCTask(672, 1542, -2089, 10969, 555, 120, 367);
                            Delay(3);
                            SpawnNPCTask(672, 1542, -2191, 11117, 555, 300, 367);
                            Delay(3);
                            SpawnNPCTask(672, 1542, -2023, 11054, 555, 180, 367);
                            Delay(3);
                            SpawnNPCTask(672, 1542, -2090, 11146, 555, 250, 367);
                            StartTask();
                        }
                    }
                    break;
                case 235:
                    {
                        if (step == 1)
                        {
                            BeginTask();
                            SpawnNPCTask(2475, 1542, -2192, 11004, 555, 45, 367);
                            Delay(3);
                            SpawnNPCTask(2475, 1542, -2087, 10968, 555, 120, 367);
                            Delay(3);
                            SpawnNPCTask(2475, 1542, -2194, 11111, 555, 300, 367);
                            Delay(3);
                            SpawnNPCTask(2475, 1542, -2026, 11057, 555, 180, 367);
                            Delay(3);
                            SpawnNPCTask(2475, 1542, -2086, 11143, 555, 250, 367);
                            StartTask();
                        }
                    }
                    break;
                #endregion

                #region 227
                case 227:
                case 230:
                case 236:
                case 239:
                    if (step == 1)
                    {
                        BeginTask();
                        Delay(10);
                        NPCChat(1561, pc);
                        Delay(4);
                        Dash(-2118, 11054, 555, 252, 1425, 75);
                        exe = StartTask();
                        q227_step = 1;
                    }
                    break;
                #endregion
            }
        }

        public override void OnActorStartsMoving(Actor mActor, MoveArg arg)
        {
            if (mActor is ActorPC target)
            {
                if (exe?.Activated == false)
                {
                    ushort[] questIDs = new ushort[] { 225, 228, 234, 237 };
                    foreach (ushort i in questIDs)
                    {
                        if (target.Quests.ContainsKey(i))
                        {
                            Quest q = target.Quests[i];
                            if (q.Step == 2)
                            {
                                if (NPC.DistanceToPoint2D(arg.X, arg.Y) < 100)
                                {
                                    ProcessQuest(target, 3, q);
                                    target = null;
                                }
                            }
                            return;
                        }
                    }
                    questIDs = new ushort[] { 227, 230, 236, 239 };
                    foreach (ushort i in questIDs)
                    {
                        if (target.Quests.ContainsKey(i))
                        {
                            Quest q = target.Quests[i];
                            if (q.Step == 1)
                            {
                                if (NPC.DistanceToPoint2D(arg.X, arg.Y) < 100)
                                {
                                    switch (q227_step)
                                    {
                                        case 1:
                                            {
                                                BeginTask();
                                                Dash(-2123, 11037, 555, 254, 0, 0);
                                                Dash(-2313, 10448, 556, 253, 0, 0);
                                                Dash(-2399, 10184, 502, 252, 0, 0);
                                                q227_step = 2;
                                                exe = StartTask();
                                            }
                                            break;
                                        case 2:
                                            {
                                                BeginTask();
                                                NPCChat(1572, target);
                                                Delay(10);
                                                Dash(-2400, 10174, 504, 265, 1426, 43);
                                                q227_step = 3;
                                                exe = StartTask();
                                            }
                                            break;
                                        case 3:
                                            {
                                                BeginTask();
                                                Dash(-3183, 10442, 421, 161, 0, 0);
                                                q227_step = 4;
                                                exe = StartTask();
                                            }
                                            break;
                                        case 4:
                                            {
                                                BeginTask();
                                                NPCChat(2405, target);
                                                Delay(10);
                                                Dash(-3186, 10445, 643, 135, 1427, 43);
                                                q227_step = 5;
                                                exe = StartTask();
                                            }
                                            break;
                                        case 5:
                                            {
                                                BeginTask();
                                                Dash(-4258, 10987, 319, 153, 0, 0);
                                                q227_step = 6;
                                                exe = StartTask();
                                            }
                                            break;
                                        case 6:
                                            {
                                                ProcessQuest(target, 2, q);
                                                target = null;
                                                exe = null;
                                            }
                                            break;
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
}
