using Newtonsoft.Json;
using UnityEngine;

namespace ExtraLevelMeta
{
    public class WaypointColor
    {
        [JsonIgnore]
        public Color color;
        public float r { get => color.r; set => color.r = value; }
        public float g { get => color.g; set => color.g = value; }
        public float b { get => color.b; set => color.b = value; }

        public WaypointColor()
        {
            color = Color.white;
        }

        public WaypointColor(Color color)
        {
            this.color = color;
        }
    }
}
