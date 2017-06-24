namespace SagaBNS.GameServer.Scripting
{
    public static class Utils
    {
        #region Methods

        public static void SpawnNPC(Map.Map map, ushort npcID, short x, short y, short z, ushort dir, ushort motion)
        {
            SpawnNPC(map, npcID, 0, x, y, z, dir, motion);
        }

        public static void SpawnNPC(Map.Map map, ushort npcID, int appearEffect, short x, short y, short z, ushort dir, ushort motion)
        {
            NPC.SpawnData spawn = new NPC.SpawnData()
            {
                MapID = map.ID,
                IsMapObject = false,
                NpcID = npcID,
                Range = 0,
                Count = 1,
                X = x,
                Y = y,
                Z = z,
                Dir = dir,
                Delay = 0,
                Motion = motion,
                AppearEffect = appearEffect
            };
            spawn.DoSpawn(map);
        }

        #endregion
    }
}