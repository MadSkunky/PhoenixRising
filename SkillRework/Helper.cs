using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhoenixRising.SkillRework
{
    //internal enum ClassList { Assault, Heavy, Sniper, Berserker, Priest, Technician, Infiltrator }
    internal struct ClassKey
    {
        public static KeyValuePair<string, string> Assault = new KeyValuePair<string, string>("Assault", "ASSAULT TRAINING");
        public static KeyValuePair<string, string> Heavy = new KeyValuePair<string, string>("Heavy", "HEAVY TRAINING");
        public static KeyValuePair<string, string> Sniper = new KeyValuePair<string, string>("Sniper", "SNIPER TRAINING");
        public static KeyValuePair<string, string> Berserker = new KeyValuePair<string, string>("Berserker", "BERSERKER TRAINING");
        public static KeyValuePair<string, string> Priest = new KeyValuePair<string, string>("Priest", "PRIEST TRAINING");
        public static KeyValuePair<string, string> Technician = new KeyValuePair<string, string>("Technician", "TECHNICIAN TRAINING");
        public static KeyValuePair<string, string> Infiltrator = new KeyValuePair<string, string>("Infiltrator", "INFILTRATOR TRAINING");
        public static string MainSpecs = "MainSpecialization";
        public static string Level_1 = "Level 1";
        public static string Level_2 = "Level 2";
        public static string Level_3 = "Level 3";
        public static string Level_4 = "Level 4";
        public static string Level_5 = "Level 5";
        public static string Level_6 = "Level 6";
        public static string Level_7 = "Level 7";
        public static string PersSpecs = "PersonalSpecialization";
        public static string PersSpec_1 = PersonalLevel.CS1;
        public static string PersSpec_2 = PersonalLevel.CS2;
    }
    internal struct PersonalLevel
    {
        public static string BGP = "BGP";
        public static string FS1 = "FS1";
        public static string FS2 = "FS2";
        public static string CS1 = "CS1";
        public static string CS2 = "CS2";
        public static string PS1 = "PS1";
        public static string PS2 = "PS2";
    }
    internal struct Faction
    {
        public static string PX = "Phoenix";
        public static string Anu = "Anu";
        public static string NJ = "NewJericho";
        public static string Syn = "Synedrion";
        public static string IN = "Independent";
    }
    internal struct Proficiency
    {
        public static string HG = "Handgun";
        public static string PDW = "PDW";
        public static string ML = "Melee";
        public static string AR = "AssaultRifle";
        public static string SG = "Shotgun";
        public static string SR = "SniperRifle";
        public static string HW = "HeavyWeapon";
        public static string MW = "MountedWeapon";
        public static string VW = "ViralWeapon";
        public static string SW = "SilencedWeapon";
    }
    internal class Helper
    {
        //public static readonly string[] Factions = new string[] { "Phoenix", "Anu", "NewJericho", "Synedrion", "Independent" };
        //public static readonly string[] Classes = new string[] { "Assault", "Heavy", "Sniper", "Berserker", "Priest", "Technician", "Infiltrator" };
        //public static readonly string[] Proficiencies = new string[] { "Handgun", "PDW", "Melee", "AssaultRifle", "Shotgun", "SniperRifle", "HeavyWeapon", "MountedWeapon" };

        public static readonly int[] SPperLevel = new int[] { 0, 10, 15, 0, 20, 25, 30 };

        // Desearialize dictionary from Json to map ability names to Defs
        public static readonly string AbilitiesJsonFileName = "AbilityDefToNameDict.json";
        public static readonly Dictionary<string, string> AbilityNameToDefMap = ReadJson<Dictionary<string, string>>(AbilitiesJsonFileName);

        // Read embedded or external json file
        public static T ReadJson<T>(string fileName)
        {
            try
            {
                string json = null;
                Assembly assembly = Assembly.GetExecutingAssembly();
                string source = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
                if (source != null || source != "")
                {
                    using (Stream stream = assembly.GetManifestResourceStream(source))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                if (json == null || json == "")
                {
                    string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    json = File.ReadAllText(Path.Combine(ModDirectory, fileName));
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        // Write embedded and external json file
        public static bool WriteJson(string fileName, object obj, bool toFile = false)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
                Assembly assembly = Assembly.GetExecutingAssembly();
                string source = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
                if (source != null || source != "")
                {
                    using (Stream stream = assembly.GetManifestResourceStream(source))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(jsonString);
                    }
                }
                if (toFile)
                {
                    string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    File.WriteAllText(Path.Combine(ModDirectory, fileName), jsonString);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
