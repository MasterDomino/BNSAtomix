
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC206 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 206; }
        }

        public override void OnActorAppears(Actor aActor)
        {
            base.OnActorAppears(aActor);
            if (aActor.ActorType == ActorType.PC)
            {
                BeginTask();
                Delay(50);
                NPCChat(1455, 280);
                StartTask();
            }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 303:
                    if (step == 3)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1456, 280);
                        StartTask();
                    }
                    break;
            }
        }

        private bool already;
        public override void OnSkillDamage(SagaBNS.Common.Skills.SkillArg arg, SagaBNS.Common.Skills.SkillAttackResult result, int dmg, int bonusCount)
        {
            base.OnSkillDamage(arg, result, dmg, bonusCount);
            if (!already)
            {
                already = true;
                BeginTask();
                Delay(5);
                NPCChat(1612, arg.Caster);
                StartTask();
            }
        }
    }
}
