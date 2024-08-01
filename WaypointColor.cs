using System.Text.Json.Serialization;
using UnityEngine;

namespace ExtraLevelMeta
{
    public class WaypointColor
    {
        [JsonIgnore]
        public Color color;
        [JsonInclude]
        public float r { get => color.r; set => color.r = value; }
        [JsonInclude]
        public float g { get => color.g; set => color.g = value; }
        [JsonInclude]
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
