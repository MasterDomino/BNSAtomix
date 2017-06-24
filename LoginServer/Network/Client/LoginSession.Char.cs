using System;
using System.Collections.Generic;
using System.Text;

using SmartEngine.Core;
using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.Common;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;
using SagaBNS.Common.Actors;
using SagaBNS.LoginServer.Network.AccountServer;
using SagaBNS.LoginServer.Network.CharacterServer;

namespace SagaBNS.LoginServer.Network.Client
{
    public partial class LoginSession : Session<LoginPacketOpcode>
    {
        private int characterSerial, removeSerial;
        private Dictionary<byte, ActorPC> chars;
        private int currentCharIndex;
        private ActorPC createdPC, removePC;
        public void OnCharList(int serial)
        {
            characterSerial = serial;
            CharacterSession.Instance.RequestCharList(account.AccountID, this);
        }

        public void OnGotCharList(List<ActorPC> chars)
        {
            this.chars=new Dictionary<byte,ActorPC>();
            foreach (ActorPC i in chars)
            {
                this.chars[i.SlotID] = i;
            }

            currentCharIndex = 0;
            LoadInventory();
        }

        public void SendSlotList(int serial)
        {
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial
            };
            string content = "<Reply type=\"array\">\n";
            for (byte i = 0; i < (3 + account.ExtraSlots); i++)
            {
                content += string.Format("<Slot>\n<SlotId>{0}</SlotId>\n<AppGroupId>2</AppGroupId>\n<SlotType>char</SlotType>\n<SystemSlot>1</SystemSlot>\n<SlotData/>\n<Changed>2012-04-25T05:52:22Z</Changed>\n<Registered>2012-04-25T05:52:22Z</Registered>\n</Slot>\n",
                    ((uint)i).ToGUID().ToString().ToUpper());
            }

            p.Content = content + "</Reply>\n";
            p.WritePacket();

            Network.SendPacket(p);
        }

        public void SendCharList()
        {
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = characterSerial
            };
            string content = "<Reply>\n";

            for (byte i = 0; i < 5; i++)
            {
                if (chars.ContainsKey(i))
                {
                    ActorPC pc = chars[i];
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    pc.AppearenceToSteam(ms);

                    content += string.Format("<CharSlot>\n<SlotId>{0}</SlotId>\n<CharId>{1}</CharId>\n<WorldCode>{4}</WorldCode>\n<CharName>{2}</CharName>\n<CharData>&lt;bns>&lt;pcdbid>{1}&lt;/pcdbid>&lt;showcase>{3}&lt;/showcase>&lt;/bns></CharData>\n</CharSlot>\n",
                        ((uint)pc.SlotID).ToGUID(), pc.CharID, pc.Name, Convert.ToBase64String(ms.ToArray()), pc.WorldID);
                    ms.Close();
                    ms = null;
                }
                else
                {
                    content += string.Format("<CharSlot><SlotId>{0}</SlotId>\n</CharSlot>\n", ((uint)i).ToGUID());
                }
            }
            p.Content = content + "</Reply>\n";
            p.WritePacket();

            Network.SendPacket(p);
        }

        public void OnCharSlotRequest(int serial, byte slotID)
        {
            if (chars.ContainsKey(slotID))
            {
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = serial
                };
                string content = "<Reply>\n";
                ActorPC pc = chars[slotID];
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                pc.AppearenceToSteam(ms);

                content += string.Format("<SlotId>{0}</SlotId>\n<CharId>{1}</CharId>\n<WorldCode>{4}</WorldCode>\n<CharName>{2}</CharName>\n<CharData>&lt;bns>&lt;pcdbid>{1}&lt;/pcdbid>&lt;showcase>{3}&lt;/showcase>&lt;/bns></CharData>\n",
                    ((uint)pc.SlotID).ToGUID(), pc.CharID, pc.Name, Convert.ToBase64String(ms.ToArray()), pc.WorldID);
                ms.Close();
                ms = null;
                p.Content = content + "</Reply>\n";
                p.WritePacket();

                Network.SendPacket(p);
            }
        }

        public void OnCharDelete(int serial, byte slotID)
        {
            if (chars.ContainsKey(slotID))
            {
                removePC = chars[slotID];
                removeSerial = serial;
                CharacterSession.Instance.DeleteChar(removePC.CharID, this);
            }
        }

        public void OnCharDeleteResult(SM_CHAR_DELETE_RESULT.Results Result)
        {
            if (Result == SM_CHAR_DELETE_RESULT.Results.OK && removePC != null)
            {
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = removeSerial,
                    Content = string.Format("<bns>\n<protocol>Game.bns.{1}</protocol>\n<command>DeletePc</command>\n<result>OK</result>\n<slotid>{0}</slotid>\n</bns>\n",
                    ((uint)removePC.SlotID).ToGUID(), removePC.WorldID)
                };
                p.WritePacket();
                Network.SendPacket(p);
                if (chars.ContainsKey(removePC.SlotID))
                {
                    chars.Remove(removePC.SlotID);
                }
            }
            else
            {
                removePC = null;
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = removeSerial,
                    Content = string.Format("<bns>\n<protocol>Game.bns.{1}</protocol>\n<command>DeletePc</command>\n<result>Fail</result>\n<slotid>{0}</slotid>\n<reason>202</reason></bns>\n",
                    ((uint)removePC.SlotID).ToGUID(), removePC.WorldID)
                };
                p.WritePacket();
                Network.SendPacket(p);
            }
        }

        public void OnAuthGameToken(int serial)
        {
            account.LoginToken = ((uint)Global.Random.Next()).ToGUID();
            Logger.Log.Info("gametoken:" + account.LoginToken.ToString().ToUpper());

            account.TokenExpireTime = DateTime.Now.AddMinutes(10);
            AccountSession.Instance.AccountSave(account, this);

            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial,
                Content = string.Format("<Reply>\n<Token>{0}</Token>\n</Reply>\n", account.LoginToken.ToString().ToUpper())
            };
            p.WritePacket();

            Network.SendPacket(p);
        }

        public void OnAuthToken(int serial)
        {
            BNSLoginPacket p = new BNSLoginPacket()
            {
                Command = "STS/1.0 200 OK",
                Serial = serial
            };
            string token = account.AccountID.ToGUID().ToString().ToUpper() + ":" + ((uint)Global.Random.Next()).ToGUID().ToString().ToUpper();
            Logger.Log.Info("Auth Token: " + token);
            p.Content = string.Format("<Reply>\n<AuthnToken>{0}</AuthnToken>\n</Reply>\n", Convert.ToBase64String(Encoding.Default.GetBytes(token)));
            p.WritePacket();

            Network.SendPacket(p);
        }

        public void OnCharCreate(int serial, byte worldID, byte slotID, string charName, byte[] charData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(charData);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            ms.Position = 2;
            byte[] display1 = Conversions.HexStr2Bytes("6363634E63737364777777636464776464646464646464");
            br.ReadBytes(br.ReadInt16()).CopyTo(display1, 0);
            ActorPC pc = new ActorPC()
            {
                Level = 1,
                AccountID = account.AccountID,
                Name = charName,
                SlotID = slotID,
                WorldID = worldID,
                Appearence1 = display1
            };
            ms.Position += 2;
            pc.Race = (Race)br.ReadByte();
            ms.Position += 2;
            pc.Gender = (Gender)br.ReadByte();
            ms.Position += 2;
            pc.Job = (Job)br.ReadByte();
            ms.Position += 2;
            pc.Appearence2 = br.ReadBytes(br.ReadInt16());
            pc.MapID = 1101;
            pc.X = -3177;
            pc.Y = 9243;
            pc.Z = 599;
            pc.Dir = 45;
            pc.UISettings = string.Empty;
            pc.InventorySize = 32;
            switch (pc.Job)
            {
                case Job.Assassin:
                    pc.HP = 66;
                    pc.MP = 0;
                    pc.MaxHP = 66;
                    pc.MaxMP = 10;
                    break;
                case Job.ForceMaster:
                    pc.HP = 61;
                    pc.MP = 10;
                    pc.MaxHP = 61;
                    pc.MaxMP = 10;
                    break;
                case Job.KungfuMaster:
                    pc.HP = 109;
                    pc.MaxHP = 109;
                    pc.MP = 0;
                    pc.MaxMP = 10;
                    break;
                case Job.BladeMaster:
                default:
                    pc.HP = 99;
                    pc.MP = 0;
                    pc.MaxHP = 99;
                    pc.MaxMP = 10;
                    break;
            }

            characterSerial = serial;
            createdPC = pc;
            CharacterSession.Instance.CreateChar(pc, this);
        }

        public void OnCharCreateResult(uint charID,SM_CHAR_CREATE_RESULT.Results Result)
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
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = characterSerial,
                    Content = string.Format("<bns>\n<protocol>Game.bns.{1}</protocol>\n<command>CreatePc</command>\n<result>OK</result>\n<slotid>{0}</slotid>\n</bns>\n",
                    ((uint)createdPC.SlotID).ToGUID(), createdPC.WorldID)
                };
                p.WritePacket();
                Network.SendPacket(p);
                if (!chars.ContainsKey(createdPC.SlotID))
                {
                    chars[createdPC.SlotID] = createdPC;
                }
            }
            else
            {
                BNSLoginPacket p = new BNSLoginPacket()
                {
                    Command = "STS/1.0 200 OK",
                    Serial = characterSerial,
                    Content = string.Format("<bns>\n<protocol>Game.bns.{1}</protocol>\n<command>CreatePc</command>\n<result>Fail</result>\n<slotid>{0}</slotid>\n<reason>202</reason>\n</bns>\n",
                    ((uint)createdPC.SlotID).ToGUID(), createdPC.WorldID)
                };
                p.WritePacket();

                Network.SendPacket(p);
            }
        }
    }
}
