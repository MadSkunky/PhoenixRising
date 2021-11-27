using System;
using System.IO;
using System.Reflection;
using Harmony;

namespace PhoenixRising.SkillRework
{
    public class SkillReworkMain
    {
        // New config field.
        internal static Settings Config;
        internal static string LogPath;
        internal static string ModDirectory;
        public static void MainMod(Func<string, object, object> api)
        {
            // Read config and assign to config field.
            Config = api("config", null) as Settings ?? new Settings();
            // Path for own logging
            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogPath = Path.Combine(ModDirectory, "SelectClassTraits.log");
            // Initialize Logger
            Logger.Initialize(LogPath, Config.Debug, ModDirectory, nameof(SkillReworkMain));

            // Patch all patches
            HarmonyInstance.Create("SkillRework.PhoenixRising.MadSkunky").PatchAll();

            // Modnix logging
            api("log verbose", "Mod Initialised.");
        }
    }
}
