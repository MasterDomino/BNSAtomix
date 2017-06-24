using System.Collections.Generic;
using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.Common.Network
{
    public abstract partial class CharacterSession<T>: DefaultClient<CharacterPacketOpcode>
    {
        public void GetLocations(uint charID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_TELEPORT_GET p = new CM_TELEPORT_GET()
            {
                SessionID = session,
                CharID = charID
            };
            Network.SendPacket(p);
        }

        public void SaveLocations(ActorPC pc)
        {
            CM_TELEPORT_SAVE p = new CM_TELEPORT_SAVE()
            {
                CharID = pc.CharID,
                TeleportLocations = pc.Locations
            };
            Network.SendPacket(p);
        }

        public void OnTeleportInfo(SM_TELEPORT_INFO p)
        {
            long session = p.SessionID;
            if (packetSessions.ContainsKey(session))
            {
                if (packetSessions.TryRemove(session, out T client))
                {
                    OnTeleportInfo(client, p.TeleportLocations);
                }
            }
        }

        protected abstract void OnTeleportInfo(T client, List<ushort> locations);
    }
}
