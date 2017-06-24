using System.Collections.Generic;
using SagaBNS.GameServer;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Skills;
using SagaBNS.Common.Quests;

namespace Maps.Startzone
{
    public class NPC673 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 673; }
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
                foreach (ushort i in new ushort[] { 226, 229, 235, 238 })
                {
                    if (pc.Quests.ContainsKey(i))
                    {
                        Quest quest = pc.Quests[i];
                        if (quest.Step == 2 || quest.Step == 3)
                        {
                            quest.Step = 3;
                            quest.NextStep = 3;
                            List<uint> skillIDs = new List<uint>();
                            skillIDs.Add(10120);
                            skillIDs.Add(15201);
                            skillIDs.Add(12251);
                            skillIDs.Add(11129);
                            skillIDs.Add(11111);
                            if ((skillIDs.Contains(arg.Skill.ID)) && result.IsHit())//blade stab
                            {
                                quest.StepStatus = 1;
                                switch (quest.Flag3)
                                {
                                    case 7:
                                        quest.Flag3 = 15;
                                        break;
                                    case 15:
                                        quest.Flag3 = 23;
                                        break;
                                    case 23:
                                        {
                                            ProcessQuest(pc, 3, quest);
                                            SendNPCCommand("q226_2_finished");
                                            Disappear(2339);
                                            //Reset combat status 
                                            pc.Client().ChangeCombatStatus(false);
                                            return;
                                        }
                                }
                                UpdateQuest(pc, quest);
                                Disappear(2339);
                            }
                        }
                    }
                }
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (npc != NPC && command == "q226_2_finished")
            {
                Disappear(2339);
            }
        }
    }
}
