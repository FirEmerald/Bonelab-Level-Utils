using BoneLib;
using MelonLoader;
using System;
using UnityEngine;
using BoneLib.BoneMenu.Elements;
using BoneLib.BoneMenu;
using System.IO;
using System.Text.Json;
using static Il2CppSLZ.Marrow.Health;
using MelonLoader.Utils;
using Il2CppSLZ.Marrow;

namespace ExtraLevelMeta
{
    public class LevelUtilsMod : MelonMod
    {
        internal static readonly string META_FOLDER = Path.Combine(MelonEnvironment.UserDataDirectory + "LevelUtils");
        internal static readonly JsonSerializerOptions JSON_SETTINGS = new()
        {
            WriteIndented = true
        };
        private static LevelUtilsMod instance;
        private static string metaFile;
        private static LevelUtilsInfo levelMeta = new();
        private static MenuCategory menu;
        private static BoolElement reloadOnDeath;
        private static EnumElement<HealthMode> healthMode;
        private static MenuCategory newWaypointMenu;
        private static MenuCategory waypointsMenu;

        public LevelUtilsMod() => instance = this;

        public override void OnInitializeMelon()
        {
            menu = MenuManager.CreateCategory("Level Utils", Color.white);
            reloadOnDeath = menu.CreateBoolElement("Reload on Death", Color.white, true, v => {
                Player.rigManager.GetComponent<Player_Health>().reloadLevelOnDeath = v;
                levelMeta.reloadOnDeath = v;
                SaveMeta();
            });
            healthMode = menu.CreateEnumElement<HealthMode>("Mortality", Color.red, HealthMode.Mortal, (cur) => {
                Player.rigManager.GetComponent<Player_Health>().SetHealthMode((int) cur);
                levelMeta.mortality = cur;
                SaveMeta();
            });
            waypointsMenu = menu.CreateCategory("Waypoints", Color.white);
            newWaypointMenu = waypointsMenu.CreateCategory("New Waypoint", Color.white);
            AddNewWaypointColor("white", Color.white);
            AddNewWaypointColor("grey", Color.grey);
            AddNewWaypointColor("black", Color.black);
            AddNewWaypointColor("red", Color.red);
            AddNewWaypointColor("yellow", Color.yellow);
            AddNewWaypointColor("green", Color.green);
            AddNewWaypointColor("cyan", Color.cyan);
            AddNewWaypointColor("blue", Color.blue);
            AddNewWaypointColor("magenta", Color.magenta);
            menu.CreateFunctionElement("Reload meta", Color.white, LoadMeta);
            Hooking.OnLevelInitialized += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;
        }

        public static void AddNewWaypointColor(string name, Color color)
        {
            newWaypointMenu.CreateFunctionElement(name, color, () =>
            {
                AddWaypoint(color);
                if (MenuManager.ActiveCategory == newWaypointMenu) MenuManager.SelectCategory(waypointsMenu);
            });
        }

        public static void AddWaypoint(Color color)
        {
            Waypoint waypoint;
            levelMeta.waypoints.Add(waypoint = new Waypoint(Convert.ToString(levelMeta.waypoints.Count + 1), color, Player.rigManager.animationRig.transform.position));
            AddWaypointToMenus(waypoint);
            SaveMeta();
        }

        public static void ResetWaypointsMenus()
        {
            waypointsMenu.Elements.Clear();
            waypointsMenu.Elements.Add(newWaypointMenu);
            foreach (Waypoint waypoint in levelMeta.waypoints) AddWaypointToMenus(waypoint);
        }

        internal static void AddWaypointToMenus(Waypoint waypoint)
        {
            waypoint.panel = waypointsMenu.CreateSubPanel(waypoint.name, waypoint.GetColor());
            waypoint.panel.CreateFunctionElement("Teleport to", Color.white, () => Player.rigManager.Teleport(waypoint.GetPosition()));
            waypoint.panel.CreateFunctionElement("Remove", Color.white, () =>
            {
                levelMeta.waypoints.Remove(waypoint);
                waypointsMenu.Elements.Remove(waypoint.panel);
                if (MenuManager.ActiveCategory == waypointsMenu) MenuManager.SelectCategory(waypointsMenu);
            });
        }

        internal static void LevelInitialized(LevelInfo info)
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
            Player_Health health = Player.rigManager.GetComponent<Player_Health>();
            reloadOnDeath.SetValue(levelMeta.reloadOnDeath = health.reloadLevelOnDeath);
            healthMode.SetValue(levelMeta.mortality = health.healthMode);
            if (File.Exists(metaFile))
            {
                Log("Loading level meta from " + metaFile);
                levelMeta = JsonSerializer.Deserialize<LevelUtilsInfo>(File.ReadAllText(metaFile), JSON_SETTINGS);
                Log("Level meta loaded.");
                reloadOnDeath.SetValue(health.reloadLevelOnDeath = levelMeta.reloadOnDeath);
                healthMode.SetValue(levelMeta.mortality);
                health.SetHealthMode((int) levelMeta.mortality);
            }
            ResetWaypointsMenus();
        }

        internal static void Log(string str) => instance.LoggerInstance.Msg(str);

        internal static void Log(object obj) => instance.LoggerInstance.Msg(obj?.ToString() ?? "null");

        internal static void Warn(string str) => instance.LoggerInstance.Warning(str);

        internal static void Warn(object obj) => instance.LoggerInstance.Warning(obj?.ToString() ?? "null");

        internal static void Error(string str) => instance.LoggerInstance.Error(str);

        internal static void Error(object obj) => instance.LoggerInstance.Error(obj?.ToString() ?? "null");
    }
}
