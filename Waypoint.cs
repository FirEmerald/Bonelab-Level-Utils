﻿using BoneLib.BoneMenu.Elements;
using Newtonsoft.Json;
using UnityEngine;

namespace ExtraLevelMeta
{
    public class Waypoint
    {
        public string name;

        public WaypointColor color;
        public WaypointPosition position;
        [JsonIgnore]
        public SubPanelElement panel;

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
