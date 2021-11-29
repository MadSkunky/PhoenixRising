using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PhoenixRising.SkillRework
{//PhoenixRising.SkillRework.Settings
    internal class Settings
    {
        //internal static ConfigHelper configHelper = new ConfigHelper();
        internal static string[] Factions = new string[] { "Phoenix", "Anu", "NewJericho", "Synedrion" };
        internal static string[] Classes = new string[] { "Assault", "Heavy", "Sniper", "Berserker", "Priest", "Technician", "Infiltrator" };
        internal static string[] Row = new string[] { "1stRow", "3rdRow" };

        internal static int[] SPperLevel = new int[] { 0, 10, 15, 0, 20, 25, 30 };

        internal static string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static string json = File.ReadAllText(Path.Combine(ModDirectory, "AbilityDefToNameDict.json"));
        internal static Dictionary<string, string> abilityDefToName = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        public Dictionary<string, Dictionary<string, string[]>> ClassSkills = new Dictionary<string, Dictionary<string, string[]>>
        {
            { Classes[0], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "ASSAULT TRAINING",
                    "QUICK AIM",
                    "OVERWATCH FOCUS",
                    null,
                    "READY FOR ACTION",
                    "ONSLAUGHT",
                    "RAPID CLEARANCE"
                } },
                { Row[1], new string[] {
                    "TROOPER",
                    "QUARTERBACK"
                } } } },
            {Classes[1], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "HEAVY TRAINING",
                    "RETURN FIRE",
                    "WAR CRY",
                    null,
                    "BOOM BLAST",
                    "HEAVY LIFTER",
                    "RAGE BURST"
                } },
                { Row[1], new string[] {
                    "STRONGMAN",
                    "BOMBARDIER"
                } } } },
            { Classes[2], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "SNIPER TRAINING",
                    "EXTREME FOCUS",
                    "ARMOR BREAK",
                    null,
                    "MASTER MARKSMAN",
                    "INSPIRE",
                    "MARKED FOR DEATH"
                } },
                { Row[1], new string[] {
                    "EXPERT RIFLES AND HANDGUNS",
                    "SNIPERIST"
                } } } },
            {Classes[3], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "BERSERKER TRAINING",
                    "DASH",
                    "CLOSE QUARTERS EVADE",
                    null,
                    "BLOODLUST",
                    "IGNORE PAIN",
                    "ADRENALINE RUSH"
                } },
                { Row[1], new string[] {
                    "BRAWLER",
                    "RECKLESS"
                } } } },
            {Classes[4], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "PRIEST TRAINING",
                    "MIND CONTROL",
                    "INDUCE PANIC",
                    null,
                    "MIND SENSE",
                    "PSYCHIC WARD",
                    "MIND CRUSH"
                } },
                { Row[1], new string[] {
                    "FARSIGHTED",
                    "BIOCHEMIST"
                } } } },
            { Classes[5], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "TECHNICIAN TRAINING",
                    "FAST USE",
                    "REMOTE CONTROL",
                    null,
                    "FIELD MEDIC",
                    "REMOTE DEPLOYMENT",
                    "ELECTRIC REINFORCEMENT"
                } },
                { Row[1], new string[] {
                    "SELF DEFENSE SPECIALIST",
                    "HEALER"
                } } } },
            {Classes[6], new Dictionary<string, string[]> {
                { Row[0], new string[] {
                    "INFILTRATOR TRAINING",
                    "SURPRISE ATTACK",
                    "DEPLOY DECOY",
                    null,
                    "WEAK SPOT",
                    "VANISH",
                    "SNEAK ATTACK"
                } },
                { Row[1], new string[] {
                    "THIEF",
                    "CAUTIOUS"
                } } } }
        };

        // Create new ability dictionary as json file in mod directory
        public bool CreateNewJsonForAbilities = false;
        // DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        public int Debug = 1;

    }
}
