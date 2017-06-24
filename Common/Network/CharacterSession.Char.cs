using System.Collections.Generic;
using SmartEngine.Network;

using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.Common.Network
{
    public abstract partial class CharacterSession<T> : DefaultClient<CharacterPacketOpcode>
    {
        public void RequestCharList(uint accountID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_CHAR_LIST_REQUEST p = new CM_CHAR_LIST_REQUEST()
            {
                SessionID = session,
                AccountID = accountID
            };
            Network.SendPacket(p);
        }

        public void RequestCharInfo(uint charID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_ACTOR_INFO_REQUEST p = new CM_ACTOR_INFO_REQUEST()
            {
                SessionID = session,
                CharID = charID
            };
            Network.SendPacket(p);
        }

        public void OnCharList(SM_CHAR_LIST p)
        {
            long session = p.SessionID;
            if (packetSessions.TryRemove(session, out T client))
            {
                OnCharList(client, p.Characters);
            }
        }

        protected abstract void OnCharList(T client, List<ActorPC> chars);

        public void OnActorInfo(SM_ACTOR_INFO p)
        {
            long session = p.SessionID;
            if (packetSessions.TryRemove(session, out T client))
            {
                OnActorInfo(client, p.Character);
            }
        }

        protected abstract void OnActorInfo(T client, Common.Actors.ActorPC character);

        public void CreateChar(ActorPC character, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_CHAR_CREATE p = new CM_CHAR_CREATE()
            {
                SessionID = session,
                Character = character
            };
            Network.SendPacket(p);
        }

        public void OnCharCreateResult(SM_CHAR_CREATE_RESULT p)
        {
            long session = p.SessionID;
            if (packetSessions.TryRemove(session, out T client))
            {
                OnCharCreateResult(client, p.CharID, p.Result);
            }
        }

        protected abstract void OnCharCreateResult(T client, uint charID, SM_CHAR_CREATE_RESULT.Results result);

        public void DeleteChar(uint charID, T client)
        {
            long session = Global.PacketSession;
            packetSessions[session] = client;

            CM_CHAR_DELETE p = new CM_CHAR_DELETE()
            {
                SessionID = session,
                CharID = charID
            };
            Network.SendPacket(p);
        }

        public void OnCharDeleteResult(SM_CHAR_DELETE_RESULT p)
        {
            long session = p.SessionID;
            if (packetSessions.TryRemove(session, out T client))
            {
                OnCharDeleteResult(client, p.Result);
            }
        }

        protected abstract void OnCharDeleteResult(T client, SM_CHAR_DELETE_RESULT.Results result);

        public void CharacterSave(ActorPC chara)
        {
            CharacterSave(chara, true);
        }

        public void CharacterSave(ActorPC chara, bool complete)
        {
            if (complete)
            {
                SaveQuest(chara);
                SaveLocations(chara);
                SaveSkill(chara);
            }
            CM_CHAR_SAVE p = new CM_CHAR_SAVE()
            {
                Character = chara
            };
            Network.SendPacket(p);
        }
    }
}
