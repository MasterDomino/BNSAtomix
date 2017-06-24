
using SmartEngine.Network;
using SagaBNS.Common.Network;
namespace SagaBNS.Common.Packets.CharacterServer
{
    public class SM_LOGIN_RESULT : Packet<CharacterPacketOpcode>
    {
        public enum Results
        {
            OK,
            WRONG_PASSWORD,
        }

        internal class SM_LOGIN_RESULT_INTERNAL<T> : SM_LOGIN_RESULT
        {
            public override Packet<CharacterPacketOpcode> New()
            {
                return new SM_LOGIN_RESULT_INTERNAL<T>();
            }

            public override void OnProcess(Session<CharacterPacketOpcode> client)
            {
                ((CharacterSession<T>)client).OnLoginResult(this);
            }
        }

        public SM_LOGIN_RESULT()
        {
            ID = CharacterPacketOpcode.SM_LOGIN_RESULT;
        }

        public Results Result
        {
            get
            {
                return (Results)GetByte(2);
            }
            set
            {
                PutByte((byte)value, 2);
            }
        }
    }
}
