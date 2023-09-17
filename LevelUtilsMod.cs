using BoneLib;
using MelonLoader;
using System;
using UnityEngine;
using BoneLib.BoneMenu.Elements;
using BoneLib.BoneMenu;
using System.IO;
using Newtonsoft.Json;
using static Health;

namespace ExtraLevelMeta
{
    public class LevelUtilsMod : MelonMod
    {
        internal static readonly string META_FOLDER = MelonUtils.UserDataDirectory + "\\LevelUtils";
        internal static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings() { Formatting = Formatting.Indented };
        public static LevelUtilsMod instance;
        public static string metaFile;
        public static LevelUtilsInfo levelMeta = new LevelUtilsInfo();
        public static MenuCategory menu;
        public static BoolElement reloadOnDeath;
        public static EnumElement<HealthMode> healthMode;
        public static MenuCategory newWaypointMenu;
        public static MenuCategory waypointsMenu;
        public static FunctionElement reloadMeta;

        public LevelUtilsMod() => instance = this;

        public override void OnInitializeMelon()
        {
            menu = MenuManager.CreateCategory("Level Utils", Color.white);
            reloadOnDeath = menu.CreateBoolElement("Reload on Death", Color.white, true, v => {
                Player.rigManager.GetComponent<Player_Health>().reloadLevelOnDeath = v;
                levelMeta.reloadOnDeath = v;
                saveMeta();
            });
            healthMode = menu.CreateEnumElement<HealthMode>("Mortality", Color.red, HealthMode.Mortal, (cur) => {
                Player.rigManager.GetComponent<Player_Health>().SetHealthMode((int) cur);
                levelMeta.mortality = cur;
                saveMeta();
            });
            waypointsMenu = menu.CreateCategory("Waypoints", Color.white);
            newWaypointMenu = waypointsMenu.CreateCategory("New Waypoint", Color.white);
            addNewWaypointColor("white", Color.white);
            addNewWaypointColor("grey", Color.grey);
            addNewWaypointColor("black", Color.black);
            addNewWaypointColor("red", Color.red);
            addNewWaypointColor("yellow", Color.yellow);
            addNewWaypointColor("green", Color.green);
            addNewWaypointColor("cyan", Color.cyan);
            addNewWaypointColor("blue", Color.blue);
            addNewWaypointColor("magenta", Color.magenta);
            reloadMeta = menu.CreateFunctionElement("Reload meta", Color.white, loadMeta);
            Hooking.OnLevelInitialized += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;
        }

        public static void addNewWaypointColor(string name, Color color)
        {
            newWaypointMenu.CreateFunctionElement(name, color, () =>
            {
                addWaypoint(color);
                if (MenuManager.ActiveCategory == newWaypointMenu) MenuManager.SelectCategory(waypointsMenu);
            });
        }

        public static void addWaypoint(Color color)
        {
            Waypoint waypoint;
            levelMeta.waypoints.Add(waypoint = new Waypoint(Convert.ToString(levelMeta.waypoints.Count + 1), color, Player.rigManager.animationRig.transform.position));
            addWaypointToMenus(waypoint);
            saveMeta();
        }

        public static void resetWaypointsMenus()
        {
            waypointsMenu.Elements.Clear();
            waypointsMenu.Elements.Add(newWaypointMenu);
            foreach (Waypoint waypoint in levelMeta.waypoints) addWaypointToMenus(waypoint);
        }

        internal static void addWaypointToMenus(Waypoint waypoint)
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

        /*
        public static bool isLabworks(LevelInfo info)
        {
            return info.levelReference.Crate.Pallet.Title == "LabWorksBoneworksPort" && info.barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksRedactedChamber" && info.barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksMainMenu";
        }
        */

        internal static void LevelInitialized(LevelInfo info)
        {
            metaFile = META_FOLDER + "\\" + info.levelReference.Crate.Pallet.Title + "\\" + info.barcode + ".json";
            loadMeta();
        }

        internal static void LevelUnloaded()
        {
            levelMeta.waypoints.Clear();
        }

        internal static void saveMeta()
        {
            Log("Saving level meta to " + metaFile);
            string folder = Path.GetDirectoryName(metaFile);
            if (!Directory.Exists(folder))
            {
                DirectoryInfo info = Directory.CreateDirectory(folder);
                Log("Level meta folder did not exist, created at " + info.FullName);
            }
            File.WriteAllText(metaFile, JsonConvert.SerializeObject(levelMeta, JSON_SETTINGS));
            Log("Level meta saved.");
        }

        internal static void loadMeta()
        {
            levelMeta.waypoints.Clear();
            Player_Health health = Player.rigManager.GetComponent<Player_Health>();
            reloadOnDeath.SetValue(levelMeta.reloadOnDeath = health.reloadLevelOnDeath);
            healthMode.SetValue(levelMeta.mortality = health.healthMode);
            if (File.Exists(metaFile))
            {
                Log("Loading level meta from " + metaFile);
                JsonConvert.PopulateObject(File.ReadAllText(metaFile), levelMeta, JSON_SETTINGS);
                Log("Level meta loaded.");
                reloadOnDeath.SetValue(health.reloadLevelOnDeath = levelMeta.reloadOnDeath);
                healthMode.SetValue(levelMeta.mortality);
                health.SetHealthMode((int) levelMeta.mortality);
            }
            resetWaypointsMenus();
        }

        internal static void Log(string str) => instance.LoggerInstance.Msg(str);

        internal static void Log(object obj) => instance.LoggerInstance.Msg(obj?.ToString() ?? "null");

        internal static void Warn(string str) => instance.LoggerInstance.Warning(str);

        internal static void Warn(object obj) => instance.LoggerInstance.Warning(obj?.ToString() ?? "null");

        internal static void Error(string str) => instance.LoggerInstance.Error(str);

        internal static void Error(object obj) => instance.LoggerInstance.Error(obj?.ToString() ?? "null");
    }
}
