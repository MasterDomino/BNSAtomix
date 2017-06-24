using SmartEngine.Network.Tasks;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class DieTask : Buff
    {
        private readonly GameSession client;
        public DieTask(GameSession client)
            : base(client.Character, "DieTask", 80000)
        {
            this.client = client;
            OnAdditionStart += new StartEventHandler(DieTask_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(DieTask_OnAdditionEnd);

            if (client.Character.Tasks.TryRemove("DieTask", out Task removed))
            {
                removed.Deactivate();
            }
        }

        private void DieTask_OnAdditionEnd(SmartEngine.Network.Map.Actor actor, Buff skill, bool cancel)
        {
            client.Character.Tasks.TryRemove("DieTask", out Task removed);
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = client.Character,
                Target = client.Character,
                AdditionSession = 28674,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                SkillSession = 255,
                AdditionID = 65010,
                UpdateType = UpdateTypes.Actor,
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Speed, 62);
                UserData = new byte[] { 9, 3, 0 }
            };
            if (!cancel)
            {
                client.Character.Status.Dying = false;
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dead, 1);
            }
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);

            evt = new UpdateEvent()
            {
                Actor = client.Character,
                Target = client.Character,
                AdditionSession = 28674,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel,
                UpdateType = UpdateTypes.ActorExtension
            };
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
        }

        private void DieTask_OnAdditionStart(SmartEngine.Network.Map.Actor actor, Buff skill)
        {
            UpdateEvent evt = UpdateEvent.NewActorAdditionEvent(client.Character, client.Character, 255, 28674, 65010, UpdateEvent.ExtraUpdateModes.Activate);
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Speed, 6);
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);

            evt = new UpdateEvent()
            {
                Actor = client.Character,
                Target = client.Character,
                AdditionSession = 28674,
                SkillSession = 0,
                AdditionID = 65010,
                RestTime = 80000,
                UpdateType = UpdateTypes.ActorExtension
            };
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
        }
    }
}
