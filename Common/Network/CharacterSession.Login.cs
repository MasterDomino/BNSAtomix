using SmartEngine.Network;

using SagaBNS.Common.Packets;
using SagaBNS.Common.Packets.CharacterServer;

namespace SagaBNS.Common.Network
{
    public abstract partial class CharacterSession<T> : DefaultClient<CharacterPacketOpcode>
    {
        public void OnLoginResult(Packets.CharacterServer.SM_LOGIN_RESULT p)
        {
            switch (p.Result)
            {
                case SM_LOGIN_RESULT.Results.OK:
                    state = SESSION_STATE.IDENTIFIED;
                    break;
                case SM_LOGIN_RESULT.Results.WRONG_PASSWORD:
                    state = SESSION_STATE.REJECTED;
                    break;
            }
        }
    }
}
