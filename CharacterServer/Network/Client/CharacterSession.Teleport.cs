
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Actors;
using SagaBNS.CharacterServer.Cache;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public void OnTeleportGet(Packets.Client.CM_TELEPORT_GET p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            SM_TELEPORT_INFO p1 = new SM_TELEPORT_INFO()
            {
                SessionID = p.SessionID,
                TeleportLocations = pc.Locations
            };
            Network.SendPacket(p1);
        }

        public void OnTeleportSave(Packets.Client.CM_TELEPORT_SAVE p)
        {
            ActorPC pc = CharacterCache.Instance[p.CharID];
            if (pc != null)
            {
                pc.Locations.Clear();
                foreach (ushort i in p.TeleportLocations)
                {
                    pc.Locations.Add(i);
                }
            }
        }
    }
}
