using System.Collections.Concurrent;

using SmartEngine.Network;
using SmartEngine.Core;
using SagaBNS.Common.Worlds;
namespace SagaBNS.LobbyServer.Manager
{
    public class WorldManager : Singleton<WorldManager>
    {
        private readonly ConcurrentDictionary<uint, World> worlds = new ConcurrentDictionary<uint, World>();

        public ConcurrentDictionary<uint, World> Worlds { get { return worlds; } }

        public void RegisterWorld(World w)
        {
            if (!worlds.ContainsKey(w.ID))
            {
                worlds[w.ID] = w;
            }
            else
            {
                Logger.Log.Warn(string.Format("World ID:{0} already registered!", w.ID));
            }
        }
    }
}
