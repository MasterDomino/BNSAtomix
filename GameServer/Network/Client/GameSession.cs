using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Network.CharacterServer;

namespace SagaBNS.GameServer.Network.Client
{
    public partial class GameSession : Session<GamePacketOpcode>
    {
        public override void OnConnect()
        {
            base.OnConnect();
        }

        public override void OnDisconnect()
        {
            base.OnDisconnect();
            if (account != null)
            {
                string content = "Player:" + account.UserName;
                if (chara != null)
                {
                    content += "(" + chara.Name + ")";
                }

                content += " log out.";
                Logger.Log.Info(content);
                map?.DeleteActor(chara);

                broadcastService?.Deactivate();

                broadcastService = null;
                //MapManager.Instance.DeleteMapInstance(map);
                if (chara != null)
                {
                    if (chara.Party == null)
                    {
                        foreach (SmartEngine.Network.Tasks.Task i in chara.Tasks.Values)
                        {
                            i.Deactivate();
                        }

                        chara.Tasks.Clear();
                    }
                    else
                    {
                        Tasks.Player.PartyOfflineTask task = new Tasks.Player.PartyOfflineTask(chara);
                        task.Activate();
                        Party.PartyManager.Instance.PartyMemberOfflineChange(chara.Party, chara, true);
                    }
                    chara.EventHandler = null;
                    CharacterSession.Instance.CharacterSave(chara);
                    CharacterSession.Instance.SaveInventory(chara.Inventory);
                    chara = null;
                    partyInviteTable.Clear();
                }
                account = null;
                map = null;
                if (ClientManager.Clients.Contains(this))
                {
                    Logger.Log.Warn("Seesion isn't removed from ClientManager?");
                }
            }
        }

        public override string ToString()
        {
            if (account != null && chara != null)
            {
                return string.Format("{0}({1}) {2}", chara.Name, account.UserName, Network.Socket.RemoteEndPoint);
            }
            else
            {
                return Network.Socket.RemoteEndPoint.ToString();
            }
        }
    }
}
