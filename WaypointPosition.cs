using Newtonsoft.Json;
using UnityEngine;

namespace ExtraLevelMeta
{
    public class WaypointPosition
    {
        [JsonIgnore]
        public Vector3 position;
        public float x { get => position.x; set => position.x = value; }
        public float y { get => position.y; set => position.y = value; }
        public float z { get => position.z; set => position.z = value; }

        public WaypointPosition()
        {
            position = new Vector3(0, 0, 0);
        }

        public WaypointPosition(Vector3 color)
        {
            this.position = color;
        }
    }
}
