using BoneLib.BoneMenu;
using System.Text.Json.Serialization;
using UnityEngine;

namespace ExtraLevelMeta
{
    public class Waypoint
    {
        [JsonInclude]
        public string name;

        [JsonInclude]
        public WaypointColor color;
        [JsonInclude]
        public WaypointPosition position;
        [JsonIgnore]
        public Element[] elements = new Element[3];

        public Waypoint()
        {
            name = "unnamed";
            color = new WaypointColor();
            position = new WaypointPosition();
        }

        public Waypoint(string name, Color color, Vector3 position)
        {
            this.name = name;
            this.color = new WaypointColor(color);
            this.position = new WaypointPosition(position);
        }

        public Color GetColor()
        {
            return color.color;
        }

        public Vector3 GetPosition()
        {
            return position.position;
        }
    }
}
