using BoneLib;
using MelonLoader;
using System;
using UnityEngine;
using BoneLib.BoneMenu;
using System.IO;
using System.Text.Json;
using static Il2CppSLZ.Marrow.Health;
using MelonLoader.Utils;
using Il2CppSLZ.Marrow;
using System.Collections.Generic;

namespace ExtraLevelMeta
{
    public class LevelUtilsMod : MelonMod
    {
        internal static readonly string META_FOLDER = Path.Combine(MelonEnvironment.UserDataDirectory, "LevelUtils");
        internal static readonly JsonSerializerOptions JSON_SETTINGS = new()
        {
            WriteIndented = true
        };
        private static LevelUtilsMod instance;
        private static string metaFile;
        private static LevelUtilsInfo levelMeta = new();
        private static Page menu;
        private static BoolElement reloadOnDeath;
        private static EnumElement healthMode;
        private static Page newWaypointMenu;
        private static StringElement waypointName;
        private static Page waypointsMenu;

        public LevelUtilsMod() => instance = this;

        public override void OnInitializeMelon()
        {
            menu = Menu.CreatePage("Level Utils", Color.white);
            reloadOnDeath = menu.CreateBool("Reload on Death", Color.white, true, v => {
                Player.RigManager.GetComponent<Player_Health>().reloadLevelOnDeath = v;
                levelMeta.reloadOnDeath = v;
                SaveMeta();
            });
            healthMode = menu.CreateEnum("Mortality", Color.red, HealthMode.Mortal, (cur) => {
                Player.RigManager.GetComponent<Player_Health>().SetHealthMode((int) (HealthMode) cur);
                levelMeta.mortality = (HealthMode) cur;
                SaveMeta();
            });
            waypointsMenu = menu.CreatePage("Waypoints", Color.white, maxElements: 9); //TODO cannot upload new version until either https://github.com/yowchap/BoneLib/issues/70 or https://github.com/yowchap/BoneLib/issues/71 are resolved
            // hack that lets us set the name field when the page is opened
            newWaypointMenu = waypointsMenu.CreatePage("New Waypoint", Color.white, maxElements: 9, createLink: false);
            PageLinkElement link = new(newWaypointMenu.Name, newWaypointMenu.Color, () => {
                string name = "New Waypoint";
                HashSet<string> names = new(levelMeta.waypoints.Count);
                foreach (Waypoint waypoint in levelMeta.waypoints) names.Add(waypoint.name);
                int num = 2;
                while (names.Contains(name))
                {
                    name = "New Waypoint (" + num + ")";
                    num++;
                }
                waypointName.Value = name;
                Menu.OpenPage(newWaypointMenu);
            });
            waypointsMenu.Add(link);
            link.AssignPage(newWaypointMenu);
            waypointName = newWaypointMenu.CreateString("name", Color.white, "temp", value => { });
            waypointName.Value = "temp";
            AddNewWaypointColor("white", Color.white);
            AddNewWaypointColor("grey", Color.grey);
            AddNewWaypointColor("black", Color.black);
            AddNewWaypointColor("red", Color.red);
            AddNewWaypointColor("yellow", Color.yellow);
            AddNewWaypointColor("green", Color.green);
            AddNewWaypointColor("cyan", Color.cyan);
            AddNewWaypointColor("blue", Color.blue);
            AddNewWaypointColor("magenta", Color.magenta);
            menu.CreateFunction("Reload meta", Color.white, LoadMeta);
            Hooking.OnLevelLoaded += LevelLoaded;
            Hooking.OnLevelUnloaded += LevelUnloaded;
        }

        public static void AddNewWaypointColor(string name, Color color)
        {
            newWaypointMenu.CreateFunction(name, color, () =>
            {
                AddWaypoint(color, waypointName.Value);
                if (Menu.CurrentPage == newWaypointMenu) Menu.OpenPage(waypointsMenu);
            });
        }

        public static void AddWaypoint(Color color, string name)
        {
            Waypoint waypoint;
            levelMeta.waypoints.Add(waypoint = new Waypoint(name, color, Player.RigManager.animationRig.transform.position));
            AddWaypointToMenus(waypoint);
            SaveMeta();
        }

        public static void ResetWaypointsMenus()
        {
            while (waypointsMenu.ElementCount > 1) waypointsMenu.Remove(waypointsMenu.Elements[1]);
            foreach (Waypoint waypoint in levelMeta.waypoints) AddWaypointToMenus(waypoint);
        }

        internal static void AddWaypointToMenus(Waypoint waypoint)
        {
            //TODO sub-panels, how I miss them...
            waypoint.elements[0] = waypointsMenu.CreateString("Waypoint", waypoint.GetColor(), waypoint.name, newName => { 
                waypoint.name = newName;
                waypoint.elements[1].ElementName = "Teleport to " + waypoint.name;
                waypoint.elements[2].ElementName = "Remove " + waypoint.name;
                SaveMeta();
            });
            ((StringElement) waypoint.elements[0]).Value = waypoint.name; //fix bug where string elements do not set their value to their initial value
            waypoint.elements[1] = waypointsMenu.CreateFunction("Teleport to " + waypoint.name, waypoint.GetColor(), () => Player.RigManager.Teleport(waypoint.GetPosition()));
            waypoint.elements[2] = waypointsMenu.CreateFunction("Remove " + waypoint.name, waypoint.GetColor(), () =>
            {
                levelMeta.waypoints.Remove(waypoint);
                waypointsMenu.Remove(waypoint.elements);
                if (Menu.CurrentPage == waypointsMenu) Menu.OpenPage(waypointsMenu);
            });
        }

        internal static void LevelLoaded(LevelInfo info)
        {
            metaFile = Path.Combine(META_FOLDER, info.levelReference.Crate.Pallet.Title, info.barcode + ".json");
            LoadMeta();
        }

        internal static void LevelUnloaded()
        {
            levelMeta.waypoints.Clear();
        }

        internal static void SaveMeta()
        {
            Log("Saving level meta to " + metaFile);
            string folder = Path.GetDirectoryName(metaFile);
            if (!Directory.Exists(folder))
            {
                DirectoryInfo info = Directory.CreateDirectory(folder);
                Log("Level meta folder did not exist, created at " + info.FullName);
            }
            File.WriteAllText(metaFile, JsonSerializer.Serialize(levelMeta, JSON_SETTINGS));
            Log("Level meta saved.");
        }

        internal static void LoadMeta()
        {
            levelMeta.waypoints.Clear();
            Player_Health health = Player.RigManager.GetComponent<Player_Health>();
            reloadOnDeath.Value = levelMeta.reloadOnDeath = health.reloadLevelOnDeath;
            healthMode.Value = levelMeta.mortality = health.healthMode;
            if (File.Exists(metaFile))
            {
                Log("Loading level meta from " + metaFile);
                levelMeta = JsonSerializer.Deserialize<LevelUtilsInfo>(File.ReadAllText(metaFile), JSON_SETTINGS);
                Log("Level meta loaded.");
                reloadOnDeath.Value = health.reloadLevelOnDeath = levelMeta.reloadOnDeath;
                healthMode.Value = levelMeta.mortality;
                health.SetHealthMode((int) levelMeta.mortality);
            }
            ResetWaypointsMenus();
        }

        internal static void Log(string str) => instance.LoggerInstance.Msg(str);

        internal static void Log(string str, Exception ex) => instance.LoggerInstance.Msg(str, ex);

        internal static void Log(object obj) => instance.LoggerInstance.Msg(obj?.ToString() ?? "null");

        internal static void Warn(string str) => instance.LoggerInstance.Warning(str);

        internal static void Warn(string str, Exception ex) => instance.LoggerInstance.Warning(str, ex);

        internal static void Warn(object obj) => instance.LoggerInstance.Warning(obj?.ToString() ?? "null");

        internal static void Error(string str) => instance.LoggerInstance.Error(str);

        internal static void Error(string str, Exception ex) => instance.LoggerInstance.Error(str, ex);

        internal static void Error(object obj) => instance.LoggerInstance.Error(obj?.ToString() ?? "null");
    }
}
