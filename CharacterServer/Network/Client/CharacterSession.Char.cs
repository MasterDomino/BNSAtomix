using System.Collections.Generic;

using SmartEngine.Core;
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Actors;
using SagaBNS.CharacterServer.Database;
using SagaBNS.CharacterServer.Cache;

namespace SagaBNS.CharacterServer.Network.Client
{
    public partial class CharacterSession : Session<CharacterPacketOpcode>
    {
        public void OnActorInfoRequest(Packets.Client.CM_ACTOR_INFO_REQUEST p)
        {
            SM_ACTOR_INFO r = new SM_ACTOR_INFO()
            {
                SessionID = p.SessionID,
                Character = CharacterCache.Instance[p.CharID]
            };
            Network.SendPacket(r);
        }

        public void OnCharListRequest(Packets.Client.CM_CHAR_LIST_REQUEST p)
        {
            Logger.Log.Info(string.Format(this + ":Player is requesting char list for account ID:{0}", p.AccountID));

            SM_CHAR_LIST p1 = new SM_CHAR_LIST()
            {
                SessionID = p.SessionID
            };
            List<uint> ids = CharacterDB.Instance.GetCharIDs(p.AccountID);
            foreach(uint i in CharacterCache.Instance.GetCharIDsForAccount(p.AccountID))
            {
                if(!ids.Contains(i))
                {
                    ids.Add(i);
                }
            }
            List<ActorPC> res = new List<ActorPC>();
            foreach (uint i in ids)
            {
                ActorPC pc = CharacterCache.Instance[i];
                if (pc != null)
                {
                    res.Add(pc);
                }
            }
            p1.Characters = res;
            Logger.Log.Info(string.Format(this + ":Got {0} Characters", res.Count));

            Network.SendPacket(p1);
        }

        public void OnCharCreate(Packets.Client.CM_CHAR_CREATE p)
        {
            SM_CHAR_CREATE_RESULT p1 = new SM_CHAR_CREATE_RESULT()
            {
                SessionID = p.SessionID
            };
            ActorPC chara = p.Character;
            if (CharacterDB.Instance.CheckExists(chara.Name, chara.WorldID))
            {
                p1.Result = SM_CHAR_CREATE_RESULT.Results.ERROR;
                p1.CharID = p.Character.CharID;
            }
            else
            {
                p1.Result = SM_CHAR_CREATE_RESULT.Results.OK;
                p1.CharID = CharacterCache.Instance.Create(p.Character);
            }
            Network.SendPacket(p1);
        }

        public void OnCharSave(Packets.Client.CM_CHAR_SAVE p)
        {
            ActorPC pc = p.Character;
            BackupData(pc,CharacterCache.Instance[pc.CharID]);
            CharacterCache.Instance.Save(pc.CharID, pc);
        }

        public void OnCharDelete(Packets.Client.CM_CHAR_DELETE p)
        {
            Logger.Log.Info(string.Format("Requested deletion of character ID {0}", p.CharID));
            CharacterCache.Instance.Delete(p.CharID);
            SM_CHAR_DELETE_RESULT p1 = new SM_CHAR_DELETE_RESULT()
            {
                SessionID = p.SessionID,
                Result = SM_CHAR_DELETE_RESULT.Results.OK
            };
            Network.SendPacket(p1);
        }

        private void BackupData(ActorPC newPC,ActorPC oldPC)
        {
            if (oldPC == null)
            {
                return;
            }

            newPC.Quests = oldPC.Quests;
            newPC.QuestsCompleted = oldPC.QuestsCompleted;
            newPC.Skills = oldPC.Skills;
            newPC.Locations = oldPC.Locations;
        }
    }
}
