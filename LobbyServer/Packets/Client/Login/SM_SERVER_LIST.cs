using System.Collections.Generic;
using System.Text;

using SmartEngine.Network;
using SagaBNS.Common.Worlds;
using SagaBNS.Common.Packets;

namespace SagaBNS.LobbyServer.Packets.Client
{
    public class SM_SERVER_LIST : Packet<LobbyPacketOpcode>
    {
        public SM_SERVER_LIST()
        {
            ID = LobbyPacketOpcode.SM_SERVER_LIST;
        }

        public ICollection<World> Worlds
        {
            set
            {
                PutUShort((ushort)value.Count, 6);
                foreach (World w in value)
                {
                    PutUShort((ushort)w.ID);
                    PutUShort((ushort)w.Name.Length);
                    PutBytes(Encoding.Unicode.GetBytes(w.Name));
                    PutByte(1);//Unknown
                    PutByte(1);//Unknown
                    PutBytes(w.IPAsArray);
                    PutUShort(w.Port);
                    PutInt(0);//Unknown
                    PutByte(1);// 0 offline 1 online
                    PutByte(1);//Unknown
                    PutInt(1);//Unknown
                    PutByte(1);//Unknown
                    PutUShort((ushort)w.MaxPlayerCount);
                    PutUShort((ushort)w.PlayerCount);
                    PutInt(w.MaxPlayerCount);
                }
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
