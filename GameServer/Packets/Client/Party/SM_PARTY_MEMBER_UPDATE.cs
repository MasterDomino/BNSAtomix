
using SmartEngine.Network;

using SagaBNS.Common.Packets.GameServer;
using SagaBNS.Common.Packets;
using SagaBNS.GameServer.Map;

namespace SagaBNS.GameServer.Packets.Client
{
    public class SM_PARTY_MEMBER_UPDATE : Packet<GamePacketOpcode>
    {
        public SM_PARTY_MEMBER_UPDATE()
        {
            ID = GamePacketOpcode.SM_PARTY_MEMBER_UPDATE;
        }

        public ulong ActorID
        {
            set
            {
                PutULong(value, 2);
            }
        }

        public UpdateEvent Updates
        {
            set
            {
                PutShort((short)value.ActorUpdateParameters.Count);
                ushort offsetLen = offset;
                PutShort(0);
                foreach (ActorUpdateParameter i in value.ActorUpdateParameters)
                {
                    PutShort((short)i.Parameter);
                    i.Write(this);
                }
                ushort offsetAfter = offset;
                offset = offsetLen;
                PutShort((short)(offsetAfter - offsetLen - 2));
                offset = offsetAfter;
                PutUInt(0);
                PutShort(0);
                PutByte(0);//Channel?
            }
        }
    }
}
