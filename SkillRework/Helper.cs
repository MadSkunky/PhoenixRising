using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhoenixRising
{
    class Helper
    {
        public static readonly string[] Row = new string[] { "1stRow", "3rdRow" };
        public static readonly string[] Factions = new string[] { "Phoenix", "Anu", "NewJericho", "Synedrion", "Independent" };
        public static readonly string[] Classes = new string[] { "Assault", "Heavy", "Sniper", "Berserker", "Priest", "Technician", "Infiltrator" };
        public static readonly string[] Proficiencies = new string[] { "Handgun", "PDW", "Melee", "AssaultRifle", "Shotgun", "SniperRifle", "HeavyWeapon", "MountedWeapon" };

        public static readonly int[] SPperLevel = new int[] { 0, 10, 15, 0, 20, 25, 30 };

        // Desearialize dictionary from Json to map ability names to Defs
        public static readonly string AbilityNameToDefJson = "AbilityDefToNameDict.json";
        public static readonly Dictionary<string, string> abilityNameToDefMap = ReadJson<Dictionary<string, string>>(AbilityNameToDefJson);

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
