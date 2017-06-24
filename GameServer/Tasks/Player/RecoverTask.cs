using System.Threading;
using SmartEngine.Network.Tasks;
using SagaBNS.GameServer.Network.Client;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Tasks.Player
{
    public class RecoverTask : Buff
    {
        private readonly GameSession client;
        public RecoverTask(GameSession client)
            : base(client.Character, "RecoverTask", 40000)
        {
            this.client = client;
            OnAdditionStart += new StartEventHandler(DieTask_OnAdditionStart);
            OnAdditionEnd += new EndEventHandler(DieTask_OnAdditionEnd);

            if (client.Character.Tasks.TryRemove("RecoverTask", out Task removed))
            {
                removed.Deactivate();
            }
        }

        private void DieTask_OnAdditionEnd(SmartEngine.Network.Map.Actor actor, Buff skill, bool cancel)
        {
            client.Character.Tasks.TryRemove("RecoverTask", out Task removed);
            client.Character.Status.Recovering = false;
            if (!cancel)
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = client.Character,
                    Target = client.Character,
                    UpdateType = UpdateTypes.Actor
                };
                //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dead, 0);
                client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
                Interlocked.Exchange(ref client.Character.HP, (int)(client.Character.MaxHP * 0.7f));
                client.SendPlayerHP();
                client.Character.Status.Dead = false;
                client.Character.Status.Dying = false;

                evt = new UpdateEvent()
                {
                    Actor = client.Character,
                    Target = client.Character,
                    UpdateType = UpdateTypes.PlayerRecover,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Activate
                };
                client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
            }
            else
            {
                UpdateEvent evt = new UpdateEvent()
                {
                    Actor = client.Character,
                    Target = client.Character,
                    UpdateType = UpdateTypes.PlayerRecover,
                    ExtraActivateMode = UpdateEvent.ExtraUpdateModes.Cancel
                };
                client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
            }
        }

        private void DieTask_OnAdditionStart(SmartEngine.Network.Map.Actor actor, Buff skill)
        {
            if (client.Character.Tasks.TryRemove("DieTask", out Task removed))
            {
                removed.Deactivate();
            }

            client.Character.Status.Recovering = true;
            UpdateEvent evt = new UpdateEvent()
            {
                Actor = client.Character,
                Target = client.Character,
                UpdateType = UpdateTypes.PlayerRecover,
                ExtraActivateMode = UpdateEvent.ExtraUpdateModes.None
            };
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);

            evt = new UpdateEvent()
            {
                Actor = client.Character,
                Target = client.Character,
                AdditionID = 65000,
                UpdateType = UpdateTypes.Actor
            };
            //evt.AddActorPara(Common.Packets.GameServer.PacketParameter.Dead, 3);
            client.Map.SendEventToAllActorsWhoCanSeeActor(MapEvents.EVENT_BROADCAST, evt, client.Character, true);
        }
    }
}
