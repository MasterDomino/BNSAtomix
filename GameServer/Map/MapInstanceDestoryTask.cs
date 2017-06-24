
using SmartEngine.Network.Tasks;

namespace SagaBNS.GameServer.Map
{
    public class MapInstanceDestoryTask : Task
    {
        private Map map;
        public MapInstanceDestoryTask(Map map)
            : base(600000, 60000, "MapInstanceDestory")
        {
            this.map = map;
        }

        public override void CallBack()
        {
            Deactivate();
            MapManager.Instance.DeleteMapInstance(map);
            map = null;
        }
    }
}
