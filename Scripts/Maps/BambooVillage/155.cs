using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC155 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 155; }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 303:
                    if (step == 7)
                    {
                        BeginTask();
                        Delay(5);
                        NPCChat(1610, pc);
                        Delay(36);
                        Move(-12950, -16198, -815, 304, 225);
                        Move(-12931, -16222, -815, 308, 500);
                        Move(-12922, -16234, -815, 308, 241);
                        Move(-12949, -16248, -815, 208, 500);
                        Move(-12976, -16262, -815, 208, 500);
                        Move(-12979, -16264, -815, 208, 48);
                        Move(-13007, -16277, -814, 206, 500);
                        Move(-13030, -16288, -813, 206, 403);
                        Move(-13049, -16312, -813, 232, 500);
                        Move(-13068, -16336, -816, 232, 500);
                        Move(-13075, -16344, -817, 232, 161);
                        NPCChat(1609, 155);
                        StartTask();
                    }
                    if (step == 8)
                    {
                        BeginTask();
                        NPC.X_Ori = -13075;
                        NPC.Y_Ori = -16344;
                        NPC.Z_Ori = -817;
                        NPC.Faction = Factions.SelfDefenceForce;
                        Delay(48);
                        SpawnNPCTask(1126, -13291, -16645, -834, 40, 392);
                        Delay(19);
                        NPCChat(1620, 232);
                        StartTask();
                    }
                    break;
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q303_end")
            {
                BeginTask();
                Delay(5);
                NPCChat(1623, 84);
                StartTask();
            }
        }
    }
}
