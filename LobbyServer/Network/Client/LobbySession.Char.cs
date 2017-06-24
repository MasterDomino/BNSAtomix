using System.Collections.Generic;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.Common;
using SagaBNS.Common.Actors;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.LobbyServer.Packets.Client;
using SagaBNS.LobbyServer.Network.CharacterServer;

namespace SagaBNS.LobbyServer.Network.Client
{
    public partial class LobbySession : Session<LobbyPacketOpcode>
    {
        private Dictionary<byte, ActorPC> chars;
        private int currentCharIndex;
        private ActorPC createdPC, removePC;

        public void OnCharCreate(SagaBNS.LobbyServer.Packets.Client.CM_CHAR_CREATE p)
        {
            createdPC = p.Character;
            createdPC.AccountID = acc.AccountID;
            CharacterSession.Instance.CreateChar(p.Character, this);
        }

        public void OnDeleteChar(SagaBNS.LobbyServer.Packets.Client.CM_CHAR_DELETE p)
        {
            byte slotID = 99;
            for (int i = 0; i < 10; i++)
            {
                string temp1 = Conversions.bytes2HexString(Utils.slot2GuidBytes(i));
                string temp2 = Conversions.bytes2HexString(p.Guid);
                if (temp1.Equals(temp2))
                {
                    slotID = (byte)i;
                    break;
                }
            }
            if (chars.ContainsKey(slotID))
            {
                removePC = chars[slotID];
                CharacterSession.Instance.DeleteChar(removePC.CharID, this);
            }
        }

        public void OnCharDeleteResult(SM_CHAR_DELETE_RESULT.Results Result)
        {
            if (Result == SM_CHAR_DELETE_RESULT.Results.OK && removePC != null)
            {
                SM_CHAR_DELETE p = new SM_CHAR_DELETE()
                {
                    SlotGuid = Utils.slot2GuidBytes(removePC.SlotID),
                    Reason = SM_CHAR_DELETE.Reasons.Okay
                };
                Network.SendPacket(p);

                if (chars.ContainsKey(removePC.SlotID))
                {
                    chars.Remove(removePC.SlotID);
                }
            }
            else
            {
                removePC = null;
            }
        }

        public void OnCharCreateResult(uint charID, SM_CHAR_CREATE_RESULT.Results Result)
        {
            if (Result == SM_CHAR_CREATE_RESULT.Results.OK)
            {
                createdPC.CharID = charID;
                Common.Quests.Quest q = new Common.Quests.Quest()
                {
                    QuestID = 250,
                    Step = 1,
                    NextStep = 1
                };
                createdPC.Quests[q.QuestID] = q;
                CharacterSession.Instance.CharacterSave(createdPC);

                SM_CHAR_CREATE p = new SM_CHAR_CREATE()
                {
                    Character = createdPC
                };
                Network.SendPacket(p);
                chars[createdPC.SlotID] = createdPC;
            }
            else
            {
                SM_CHAR_CREATE_FAILED p1 = new SM_CHAR_CREATE_FAILED()
                {
                    SlotID = Utils.slot2GuidBytes(createdPC.SlotID),
                    Reason = SM_CHAR_CREATE_FAILED.Reasons.NameAlreadyExists
                };
                Network.SendPacket(p1);
            }
        }

        public void OnCharacterList(List<ActorPC> chars)
        {
            this.chars = new Dictionary<byte, ActorPC>();
            foreach (ActorPC i in chars)
            {
                this.chars[i.SlotID] = i;
            }

            currentCharIndex = 0;
            LoadInventory();
        }

        public void SendCharacterList()
        {
            SM_CHARACTER_LIST r = new SM_CHARACTER_LIST();
            List<ActorPC> list = new List<ActorPC>();
            foreach (KeyValuePair<byte, ActorPC> i in chars)
            {
                list.Add(i.Value);
            }

            r.Characters = list;
            Network.SendPacket(r);
        }

        public void OnRequestLogin(CM_REQUEST_LOGIN p)
        {
            SM_REQUEST_LOGIN p1 = new SM_REQUEST_LOGIN();
            int slot = -1;
            for (int i = 0; i < (3 + acc.ExtraSlots); i++)
            {
                if (Conversions.bytes2HexString(p.Guid).Equals(Conversions.bytes2HexString(Utils.slot2GuidBytes(i))))
                {
                    slot = i;
                    break;
                }
            }

            Logger.Log.Info(string.Format("User {0} attempting to login to character {1} on slot {2}",acc.UserName,chars[(byte)slot].Name,slot));
            p1.CharID = chars[(byte)slot].CharID;
            Network.SendPacket(p1);
        }

        public void OnCharacterList(CM_CHARACTER_LIST p)
        {
            CharacterSession.Instance.RequestCharList(acc.AccountID, this);
        }
    }
}
