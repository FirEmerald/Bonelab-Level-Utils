using System.Collections.Generic;
using static Health;

namespace ExtraLevelMeta
{
    public class LevelUtilsInfo
    {
        public bool reloadOnDeath;
        public HealthMode mortality = HealthMode.Mortal;
        public List<Waypoint> waypoints = new List<Waypoint>();
    }
}
