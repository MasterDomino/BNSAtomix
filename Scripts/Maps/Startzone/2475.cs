﻿using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC2475 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 2475; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {

        }

        public override void OnSkillDamage(SkillArg arg, SkillAttackResult result, int dmg, int bonusCount)
        {
            base.OnSkillDamage(arg, result, dmg, bonusCount);
            System.Threading.Interlocked.Exchange(ref NPC.HP, NPC.MaxHP);
            if (arg.Caster is ActorPC pc)
            {
                if (pc.Quests.ContainsKey(235))
                {
                    Quest quest = pc.Quests[235];
                    if (quest.Step == 1 || quest.Step == 2 && quest.NextStep < 3)
                    {
                        quest.Step = 2;
                        quest.NextStep = 2;
                        if (arg.Skill.ID == 12106 && bonusCount >= 3)//Energy missile
                        {
                            quest.StepStatus = 1;
                            switch (quest.Flag3)
                            {
                                case 1:
                                    quest.Flag3 = 3;
                                    break;
                                case 3:
                                    quest.Flag3 = 5;
                                    break;
                                case 5:
                                    {
                                        quest.Flag3 = 7;
                                        quest.NextStep = 3;
                                        quest.Flag1 = 3;
                                        quest.Flag2 = 3;
                                        SendNPCCommand("q226_1_finished");
                                        BeginTask();
                                        Delay(3);
                                        SpawnNPCTask(673, 1542, -2146, 11145, 555, 280, 367);
                                        Delay(3);
                                        SpawnNPCTask(673, 1542, -2215, 11061, 555, 1, 367);
                                        Delay(3);
                                        SpawnNPCTask(673, 1542, -2144, 10968, 555, 75, 367);
                                        Delay(3);
                                        SpawnNPCTask(673, 1542, -2038, 10997, 555, 130, 367);
                                        Delay(3);
                                        SpawnNPCTask(673, 1542, -2038, 11112, 555, 205, 367);
                                        StartTask();
                                    }
                                    break;
                            }
                            Disappear(2339);
                            UpdateQuest(pc, quest);
                        }
                    }
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (npc != NPC && command == "q226_1_finished")
            {
                Disappear(2339);
            }
        }
    }
}
