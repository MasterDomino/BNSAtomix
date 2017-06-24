
using SmartEngine.Network.Map;
using SagaBNS.GameServer.Scripting;

namespace Maps.BambooVillageNight
{
    public class NPC204 : NPCScriptHandler
    {
        public override ushort NpcID
        {
            get { return 204; }
        }

        public override void OnActorAppears(Actor aActor)
        {
            base.OnActorAppears(aActor);
            if (aActor.ActorType == ActorType.PC)
            {
                BeginTask();
                Delay(5);
                NPCChat(1615, 50);
                StartTask();
            }
        }
    }
}
