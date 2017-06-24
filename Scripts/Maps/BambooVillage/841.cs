
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Quests;

namespace Maps.BambooVillageNight
{
    public class NPC841 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 841; }
        }

        public override void OnActorAppears(Actor aActor)
        {
            base.OnActorAppears(aActor);
            if (aActor.ActorType == ActorType.NPC)
            {
                if (((ActorNPC)aActor).NpcID == 1126)
                {
                    BeginTask();
                    Delay(20);
                    NPCChat(1627, aActor);
                    StartTask();
                }
            }
        }

        public override void OnQuest(ActorPC pc, ushort questID, byte step, Quest quest)
        {
            switch (questID)
            {
                case 303:
                    if (step == 5)
                    {
                        BeginTask();
                        Delay(5);
                        Move(-12741, -16324, -815, 156, 145);
                        Move(-12769, -16311, -816, 155, 500);
                        Move(-12797, -16299, -817, 155, 500);
                        Move(-12825, -16286, -817, 155, 500);
                        Move(-12853, -16273, -816, 155, 500);
                        Move(-12881, -16260, -815, 155, 500);
                        Move(-12908, -16247, -815, 155, 467);
                        NPCChat(1609, 155);
                        StartTask();
                    }
                    if (step == 7)
                    {
                        BeginTask();
                        Delay(48);
                        NPCChat(1611, pc);
                        Delay(7);
                        Move(-12932, -16249, -815, 185, 387);
                        Move(-12963, -16250, -815, 183, 500);
                        Move(-12986, -16251, -815, 183, 370);
                        Move(-13014, -16263, -814, 205, 500);
                        Move(-13039, -16275, -813, 205, 435);
                        Move(-13060, -16297, -813, 227, 500);
                        Move(-13081, -16319, -812, 227, 500);
                        Move(-13091, -16329, -816, 227, 225);
                        StartTask();
                    }
                    if (step == 8)
                    {
                        NPC.X_Ori = -13091;
                        NPC.Y_Ori = -16329;
                        NPC.Z_Ori = -816;
                        BeginTask();
                        Delay(48);
                        NPCChat(1619, 227);
                        StartTask();
                        NPC.Faction = Factions.SelfDefenceForce;
                    }
                    break;
            }
        }

        public override void OnReceiveNPCCommand(ActorNPC npc, string command)
        {
            if (command == "q303_end")
            {
                BeginTask();
                Delay(18);
                NPCChat(1281, 25);
                Delay(44);
                NPCChat(1624, 71);
                StartTask();
            }
        }
    }
}
