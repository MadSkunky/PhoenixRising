using Base;
using Base.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhoenixRising.SkillRework
{
    internal struct ClassSpecDef
    {
        public string ClassName;
        internal ClassDef Class;
        public string[] MainSpec;
        public ClassSpecDef(ClassDef classDef, string[] mainSpec)
        {
            ClassName = classDef.Name;
            Class = classDef;
            MainSpec = mainSpec;
            MainSpec[0] = classDef.Proficiency + " - Changes will not apply!";
            MainSpec[3] = "2nd Class selection - Changes will not apply!";
        }
    }
    internal struct ClassDef
    {
        public string Name { get; }
        public string Proficiency { get; }
        public ClassDef(string name, string proficiency)
        {
            Name = name;
            Proficiency = proficiency;
        }
    }
    internal struct PersonalPerksDef
    {
        internal const string NoKey = "NoKey";
        internal Dictionary<string, Dictionary<string, List<string>>> PerkDictionary { get; set; }
        public string PerkKey { get; set; }
        public bool IsRandom { get; set; }
        public int SPcost { get; set; }

        public List<string> UnrelatedRandomPerks
        {
            get
            {
                bool isRNG = IsRandom && PerkDictionary.ContainsKey(NoKey)
                             && PerkDictionary.Any(i => i.Value.ContainsKey(NoKey));
                return isRNG ? (PerkDictionary?.FirstOrDefault().Value?.FirstOrDefault().Value) : null;
            }
            set => PerkDictionary = new Dictionary<string, Dictionary<string, List<string>>>
                { { NoKey, new Dictionary<string, List<string>> { { NoKey, value } } } };
        }

        public Dictionary<string, Dictionary<string, string>> RelatedFixedPerks
        {
            get
            {
                if (IsRandom || PerkDictionary.ContainsKey(NoKey) || PerkDictionary.Any(i => i.Value.ContainsKey(NoKey)))
                {
                    return null;
                }

                Dictionary<string, Dictionary<string, string>> tmpDict = new Dictionary<string, Dictionary<string, string>>();
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> outer in PerkDictionary)
                {
                    tmpDict.Add(outer.Key, new Dictionary<string, string>());
                    foreach (KeyValuePair<string, List<string>> inner in outer.Value)
                    {
                        tmpDict[outer.Key].Add(inner.Key, inner.Value.FirstOrDefault());
                    }
                }
                return tmpDict;
            }
            set
            {
                if (value == null) return;
                PerkDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                foreach (KeyValuePair<string, Dictionary<string, string>> outer in value)
                {
                    PerkDictionary.Add(outer.Key, new Dictionary<string, List<string>>());
                    foreach (KeyValuePair<string, string> inner in outer.Value)
                    {
                        PerkDictionary[outer.Key].Add(inner.Key, new List<string>() { inner.Value });
                    }
                }
            }
        }
        public static implicit operator List<string>(PersonalPerksDef ppk)
        {
            return ppk.UnrelatedRandomPerks;
        }

        public PersonalPerksDef(string perkKey, bool isRandom, int spCost, Dictionary<string, Dictionary<string, List<string>>> perkDict)
        {
            PerkKey = perkKey;
            IsRandom = isRandom;
            SPcost = spCost;
            PerkDictionary = perkDict;
        }
        public PersonalPerksDef(string key, int spc, List<string> rngList)
        {
            Dictionary<string, Dictionary<string, List<string>>> tmp = new Dictionary<string, Dictionary<string, List<string>>>
                { { NoKey, new Dictionary<string, List<string>> { { NoKey, rngList } } } }; ;
            bool isRNG = rngList.Count != 1;
            this = new PersonalPerksDef(key, isRNG, spc, tmp);
        }
        public PersonalPerksDef(string perkKey, bool isRandom, int spCost, Dictionary<string, Dictionary<string, string>> relList)
        {
            PerkKey = perkKey;
            IsRandom = isRandom;
            SPcost = spCost;
            if (relList != null)
            {
                PerkDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                foreach (KeyValuePair<string, Dictionary<string, string>> outer in relList)
                {
                    PerkDictionary.Add(outer.Key, new Dictionary<string, List<string>>());
                    foreach (KeyValuePair<string, string> inner in outer.Value)
                    {
                        PerkDictionary[outer.Key].Add(inner.Key, new List<string>() { inner.Value });
                    }
                }
            }
            else
            {
                PerkDictionary = null;
            }
        }

        internal (string perk, int spCost) GetPerk(Settings config, string className = null, string faction = null, List<string> exclusionList = null)
        {
            string abilityName = null;
            if (!IsRandom)
            {
                switch (PerkKey)
                {
                    case PerkType.Background:
                    case PerkType.Proficiency:
                        return (perk: default, spCost: default);
                    case PerkType.Class_1:
                    case PerkType.Class_2:
                        if (className != null
                            && RelatedFixedPerks != null
                            && RelatedFixedPerks.Count > 0
                            && RelatedFixedPerks.ContainsKey(FactionKeys.All)
                            && RelatedFixedPerks[FactionKeys.All] is Dictionary<string, string> classDict
                            && classDict.ContainsKey(className))
                        {
                            abilityName = classDict[className];
                            return (perk: Helper.AbilityNameToDefMap[abilityName], spCost: SPcost);
                        }
                        else
                        {
                            return (perk: default, spCost: default);
                        }
                    case PerkType.Faction_1:
                        if (faction != null
                            && RelatedFixedPerks != null
                            && RelatedFixedPerks.Count > 0
                            && RelatedFixedPerks.ContainsKey(ClassKeys.AllClasses.Name)
                            && RelatedFixedPerks[ClassKeys.AllClasses.Name] is Dictionary<string, string> factionDict
                            && factionDict.ContainsKey(faction))
                        {
                            abilityName = factionDict[faction];
                            return (perk: Helper.AbilityNameToDefMap[abilityName], spCost: SPcost);
                        }
                        else
                        {
                            return (perk: default, spCost: default);
                        }
                    case PerkType.Faction_2:
                        if (faction != null
                            && className != null
                            && RelatedFixedPerks != null
                            && RelatedFixedPerks.Count > 0
                            && RelatedFixedPerks is Dictionary<string, Dictionary<string, string>> factionClassDict
                            && factionClassDict.ContainsKey(faction)
                            && factionClassDict[faction].ContainsKey(className))
                        {
                            abilityName = factionClassDict[faction][className];
                            return (perk: Helper.AbilityNameToDefMap[abilityName], spCost: SPcost);
                        }
                        break;
                    default:
                        return (perk: default, spCost: default);
                }
            }
            else
            {
                Random rnd = new Random((int)DateTime.Now.Ticks);
                int safeguard = 0;
                bool usedFound;
                bool proficienyAlreadySet = false;
                if (UnrelatedRandomPerks != null)
                {
                    do
                    {
                        abilityName = UnrelatedRandomPerks.GetRandomElement(rnd);
                        usedFound = exclusionList.Contains(abilityName);
                        if (PerkKey.Equals(PerkType.Proficiency))
                        {
                            proficienyAlreadySet = config.RadomSkillExclusionMap[abilityName].Contains(className);
                        }
                        safeguard++;
                    } while ((usedFound || proficienyAlreadySet) && safeguard <= UnrelatedRandomPerks.Count * 2);
                    exclusionList.Add(abilityName);
                }
            }
            if (abilityName != null && Helper.AbilityNameToDefMap.ContainsKey(abilityName))
            {
                return (perk: Helper.AbilityNameToDefMap[abilityName], spCost: SPcost);
            }
            else return (perk: default, spCost: default);
        }
    }
    internal readonly struct ClassKeys
    {
        public static readonly ClassDef Assault = new ClassDef("Assault", "ASSAULT TRAINING");
        public static readonly ClassDef Heavy = new ClassDef("Heavy", "HEAVY TRAINING");
        public static readonly ClassDef Sniper = new ClassDef("Sniper", "SNIPER TRAINING");
        public static readonly ClassDef Berserker = new ClassDef("Berserker", "BERSERKER TRAINING");
        public static readonly ClassDef Priest = new ClassDef("Priest", "PRIEST TRAINING");
        public static readonly ClassDef Technician = new ClassDef("Technician", "TECHNICIAN TRAINING");
        public static readonly ClassDef Infiltrator = new ClassDef("Infiltrator", "INFILTRATOR TRAINING");
        public static readonly ClassDef AllClasses = new ClassDef("All Classes", "none");
    }
    internal readonly struct FactionKeys
    {
        public const string PX = "Phoenix";
        public const string Anu = "Anu";
        public const string NJ = "NewJericho";
        public const string Syn = "Synedrion";
        public const string IN = "Independent";
        public const string PU = "Pure";
        public const string FS = "Forsaken";
        public const string All = "All Factions";
    }
    internal readonly struct PerkType
    {
        public const string Background = "Background";
        public const string Proficiency = "Proficiency";
        public const string Class_1 = "Class 1";
        public const string Class_2 = "Class 2";
        public const string Faction_1 = "Faction 1";
        public const string Faction_2 = "Faction 2";
    }

    internal readonly struct Proficiency
    {
        public const string HG = "Handgun";
        public const string PD = "PDW";
        public const string ML = "Melee";
        public const string AR = "AssaultRifle";
        public const string SG = "Shotgun";
        public const string SR = "SniperRifle";
        public const string HW = "HeavyWeapon";
        public const string MW = "MountedWeapon";
        public const string VW = "ViralWeapon";
        public const string SW = "SilencedWeapon";
        public const string JP = "JetPack";
        public const string Buff = "PercentageToAdd";
    }
    internal struct ViewElement
    {
        public const string DisplayName1 = "DisplayName1";
        public const string DisplayName2 = "DisplayName2";
        public const string Description = "Description";
    }
    internal class Helper
    {
        // SP cost for main specialisation skills per level
        public static readonly int[] SPperLevel = new int[] { 0, 10, 15, 0, 20, 25, 30 };

        // Desearialize dictionary from Json to map ability names to Defs
        public static readonly string AbilitiesJsonFileName = "AbilityDefToNameDict.json";
        public static readonly Dictionary<string, string> AbilityNameToDefMap = ReadJson<Dictionary<string, string>>(AbilitiesJsonFileName);

        // Desearialize dictionary from Json to map non localized texts to ViewDefs
        public static readonly string TextMapFileName = "NotLocalizedTextMap.json";
        public static readonly Dictionary<string, Dictionary<string, string>> NotLocalizedTextMap = ReadJson<Dictionary<string, Dictionary<string, string>>>(TextMapFileName);

        public static void ChangeUItext (ref LocalizedTextBind localizedTextBind, string localizationKey)
        {
            bool doNotLocalize = SkillReworkMain.Config.DoNotLocalizeChangedTexts;
            if (doNotLocalize)
            {
                localizedTextBind = new LocalizedTextBind(localizationKey, doNotLocalize);
            }
        }

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
        public static void WriteJson(string fileName, object obj, bool toFile = false)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
                // Writing in running assembly -- TODO: figure out if possible
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //string source = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
                //if (source != null || source != "")
                //{
                //    using (Stream stream = assembly.GetManifestResourceStream(source))
                //    using (StreamWriter writer = new StreamWriter(stream))
                //    {
                //        writer.Write(jsonString);
                //    }
                //}
                if (toFile)
                {
                    string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    File.WriteAllText(Path.Combine(ModDirectory, fileName), jsonString);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
