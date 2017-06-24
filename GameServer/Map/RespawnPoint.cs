namespace SagaBNS.GameServer.Map
{
    public struct RespawnPoint
    {
        public uint MapID;
        public short X, Y, Z;
        public ushort Dir, teleportId;
    }
}
