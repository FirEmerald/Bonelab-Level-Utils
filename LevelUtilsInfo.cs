using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Il2CppSLZ.Marrow.Health;

namespace ExtraLevelMeta
{
    public class LevelUtilsInfo
    {
        [JsonInclude]
        public bool reloadOnDeath;
        [JsonInclude]
        public HealthMode mortality = HealthMode.Mortal;
        [JsonInclude]
        public List<Waypoint> waypoints = new();
    }
}
