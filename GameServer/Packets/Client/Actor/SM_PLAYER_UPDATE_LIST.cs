
using SmartEngine.Network;
using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.GameServer;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PLAYER_UPDATE_LIST : Packet<GamePacketOpcode>
    {
        public SM_PLAYER_UPDATE_LIST()
        {
            ID = GamePacketOpcode.SM_PLAYER_UPDATE_LIST;
        }

        public UpdateEvent Parameters
        {
            set
            {
                PutUShort(1, 7);//Set Count
                PutUShort((ushort)value.ActorUpdateParameters.Count);//Count
                ushort lenOff = offset;
                PutUShort(0);
                foreach (ActorUpdateParameter j in value.ActorUpdateParameters)
                {
                    PutShort((short)j.Parameter);
                    j.Write(this);
                }
                PutUShort((ushort)(Length - lenOff - 2), lenOff);
                PutInt((int)Length - 6, 2);
            }
        }
    }
}
