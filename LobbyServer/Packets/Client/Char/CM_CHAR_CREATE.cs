using System.Text;

using SmartEngine.Network;
using SmartEngine.Network.Utils;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Actors;
using SagaBNS.Common;
using SagaBNS.LobbyServer.Network.Client;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class CM_CHAR_CREATE : Packet<LobbyPacketOpcode>
    {
        public CM_CHAR_CREATE()
        {
            ID = LobbyPacketOpcode.CM_CHAR_CREATE;
        }

        public ActorPC Character
        {
            get
            {
                ActorPC pc = new ActorPC();
                byte[] guid = GetBytes(16, 2);
                for (int i = 0; i < 10; i++)
                {
                    if (Conversions.bytes2HexString(guid).Equals(Conversions.bytes2HexString(Utils.slot2GuidBytes(i))))
                    {
                        pc.SlotID = (byte)i;
                        break;
                    }
                }
                pc.WorldID = (byte)GetUShort(18);
                pc.Name = Encoding.Unicode.GetString(GetBytes((ushort)(GetUShort(20) * 2)));
                offset += 4;
                pc.Appearence1 = Conversions.HexStr2Bytes(Conversions.bytes2HexString(GetBytes(GetUShort())) + "7364777777636464776464646464646464");
                offset += 2;
                pc.Race = (Race)GetByte();
                offset += 2;
                pc.Gender = (Gender)GetByte();
                offset += 2;
                pc.Job = (Job)GetByte();
                offset += 2;
                pc.Appearence2 = GetBytes(GetUShort());

                //Outside Packet
                pc.Level = 1;
                pc.MapID = 1101;
                pc.X = -3177;
                pc.Y = 9243;
                pc.Z = 599;
                pc.Dir = 45;
                pc.UISettings = string.Empty;
                pc.InventorySize = 32;
                pc.HP = 99;
                pc.MaxHP = 99;
                switch (pc.Job)
                {
                    case Job.Assassin:
                        pc.MP = 0;
                        pc.MaxMP = 10;
                        break;
                    case Job.ForceMaster:
                        pc.MP = 10;
                        pc.MaxMP = 10;
                        break;
                    case Job.KungfuMaster:
                        pc.MP = 0;
                        pc.MaxMP = 10;
                        break;
                    case Job.BladeMaster:
                    default:
                        pc.MP = 0;
                        pc.MaxMP = 10;
                        break;
                }

                return pc;
            }
        }

        public override Packet<LobbyPacketOpcode> New()
        {
            return new CM_CHAR_CREATE();
        }

        public override void OnProcess(Session<LobbyPacketOpcode> client)
        {
            ((LobbySession)client).OnCharCreate(this);
        }
    }
}
