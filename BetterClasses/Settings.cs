using System.Collections.Generic;

namespace PhoenixRising.BetterClasses
{
    internal class Settings
    {
        public Dictionary<string, float> BuffsForAdditionalProficiency = new Dictionary<string, float>
        {
            { Proficiency.Buff, 0.0f }
        };
        public List<ClassSpecDef> ClassSpecializations = new List<ClassSpecDef>
        { 
            new ClassSpecDef(
                classDef: ClassKeys.Assault,
                mainSpec: new string[]
                {
                    "",
                    "QUICK AIM",
                    "KILL'N'RUN",
                    "",
                    "READY FOR ACTION",
                    "ONSLAUGHT",
                    "RAPID CLEARANCE"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Heavy,
                mainSpec: new string[]
                {
                    "",
                    "RETURN FIRE",
                    "WAR CRY",
                    "",
                    "HUNKER DOWN",
                    "DYNAMIC RESISTANCE",
                    "RAGE BURST"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Sniper,
                mainSpec: new string[]
                {
                    "",
                    "EXTREME FOCUS",
                    "ARMOR BREAK",
                    "",
                    "MASTER MARKSMAN",
                    "INSPIRE",
                    "MARKED FOR DEATH"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Berserker,
                mainSpec: new string[]
                {
                    "",
                    "DASH",
                    "CLOSE QUARTERS EVADE",
                    "",
                    "BLOODLUST",
                    "IGNORE PAIN",
                    "ADRENALINE RUSH"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Priest,
                mainSpec: new string[]
                {
                    "",
                    "MIND CONTROL",
                    "INDUCE PANIC",
                    "",
                    "MIND SENSE",
                    "PSYCHIC WARD",
                    "MIND CRUSH"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Technician,
                mainSpec: new string[]
                {
                    "",
                    "FAST USE",
                    "REMOTE CONTROL",
                    "",
                    "FIELD MEDIC",
                    "REMOTE DEPLOYMENT",
                    "ELECTRIC REINFORCEMENT"
                }),
            new ClassSpecDef(
                classDef: ClassKeys.Infiltrator,
                mainSpec: new string[]
                {
                    "",
                    "SURPRISE ATTACK",
                    "DEPLOY DECOY",
                    "",
                    "WEAK SPOT",
                    "VANISH",
                    "SNEAK ATTACK"
                }),
        };
        public string[] OrderOfPersonalPerks = new string[]
            {
                PerkType.Background,
                PerkType.Faction_1,
                PerkType.Class_1,
                PerkType.Proficiency,
                PerkType.Background,
                PerkType.Class_2,
                PerkType.Faction_2
            };
        public List<PersonalPerksDef> PersonalPerks = new List<PersonalPerksDef>()
        {   new PersonalPerksDef(
                key: PerkType.Background,
                //isRandom: true,
                spc: 10,
                rngList: new List<string>
                {
                    "SURVIVOR",
                    "EDUCATED",
                    "SCAV",
                    "HARD LABOR",
                    "SQUATTER",
                    "CORPSE DISPOSER",
                    "VOLUNTEERED",
                    "CONDO RAIDER",
                    "TUNNEL RAT",
                    "DESK JOCKEY",
                    "TROUBLEMAKER",
                    "PARANOID",
                    "A HISTORY OF VIOLENCE",
                    "DAREDEVIL",
                    "DAMAGED AMYGDALA",
                    "SANITATION EXPERT",
                    "LAB ASSISTANT",
                    "ROCKETEER",
                    "TRUE GRIT"
                }),
            new PersonalPerksDef(
                key: PerkType.Proficiency,
                //isRandom: true,
                spc: 15,
                rngList: new List<string>
                {
                    "HANDGUN PROFICIENCY",
                    "PDW PROFICIENCY",
                    "MELEE WEAPON PROFICIENCY+",
                    "ASSAULT RIFLE PROFICIENCY",
                    "SHOTGUN PROFICIENCY",
                    "SNIPER RIFLE PROFICIENCY",
                    "HEAVY WEAPON PROFICIENCY",
                    "MOUNTED WEAPON PROFICIENCY"
                }),
            new PersonalPerksDef(
                perkKey: PerkType.Class_1,
                isRandom: false,
                spCost: 20,
                relList: new Dictionary<string, Dictionary<string, string>>
                {{ FactionKeys.All, new Dictionary<string,string> {
                    { ClassKeys.Assault.Name, "AIMED BURST" },
                    { ClassKeys.Heavy.Name, "JETPACK CONTROL" },
                    { ClassKeys.Sniper.Name, "GUNSLINGER" },
                    { ClassKeys.Berserker.Name, "BRAWLER" },
                    { ClassKeys.Priest.Name, "DEVOTED" },
                    { ClassKeys.Technician.Name, "SELF DEFENSE SPECIALIST" },
                    { ClassKeys.Infiltrator.Name, "STEALTH SPECIALIST" }
                } } }),
            new PersonalPerksDef(
                perkKey: PerkType.Class_2,
                isRandom: false,
                spCost: 20,
                relList: new Dictionary<string, Dictionary<string, string>>
                {{ FactionKeys.All, new Dictionary<string,string> {
                    { ClassKeys.Assault.Name, "QUARTERBACK" },
                    { ClassKeys.Heavy.Name, "BOOM BLAST" },
                    { ClassKeys.Sniper.Name, "KILL ZONE" },
                    { ClassKeys.Berserker.Name, "MELEE SPECIALIST" },
                    { ClassKeys.Priest.Name, "BIOCHEMIST" },
                    { ClassKeys.Technician.Name, "POISON RESISTANT" },
                    { ClassKeys.Infiltrator.Name, "CAUTIOUS" }
                } } }),
            new PersonalPerksDef(
                perkKey: PerkType.Faction_1,
                isRandom: false,
                spCost: 15,
                relList: new Dictionary<string, Dictionary<string, string>>
                {{ ClassKeys.AllClasses.Name, new Dictionary<string,string> {
                    { FactionKeys.PX, "OVERWATCH FOCUS" },
                    { FactionKeys.Anu, "BREATHE MIST" },
                    { FactionKeys.NJ, "TAKEDOWN" },
                    { FactionKeys.Syn, "SHADOWSTEP" },
                    { FactionKeys.IN, "OVERWATCH FOCUS" },
                    { FactionKeys.PU, "ENDURANCE" },
                    { FactionKeys.FS, "BREATHE MIST" }
                } } }),
            new PersonalPerksDef(
                perkKey: PerkType.Faction_2,
                isRandom: false,
                spCost: 20,
                relList: new Dictionary<string, Dictionary<string, string>>
                {
                    { FactionKeys.PX, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "ENDURANCE" },
                        { ClassKeys.Heavy.Name, "ENDURANCE" },
                        { ClassKeys.Sniper.Name, "ENDURANCE" },
                        { ClassKeys.Berserker.Name, "SONIC BLAST" },
                        { ClassKeys.Priest.Name, "RESURRECT" },
                        { ClassKeys.Technician.Name, "AR TARGETING" },
                        { ClassKeys.Infiltrator.Name, "PAIN CHAMELEON" }
                    } },
                    { FactionKeys.Anu, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "SONIC BLAST" },
                        { ClassKeys.Berserker.Name, "SONIC BLAST" },
                        { ClassKeys.Priest.Name, "RESURRECT" },
                    } },
                    { FactionKeys.NJ, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "PEPPER CLOUD" },
                        { ClassKeys.Heavy.Name, "PEPPER CLOUD" },
                        { ClassKeys.Sniper.Name, "PEPPER CLOUD" },
                        { ClassKeys.Technician.Name, "AR TARGETING" },
                    } },
                    { FactionKeys.Syn, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "PHANTOM PROTOCOL" },
                        { ClassKeys.Sniper.Name, "PHANTOM PROTOCOL" },
                        { ClassKeys.Infiltrator.Name, "PAIN CHAMELEON" }
                    } },
                    { FactionKeys.IN, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "ENDURANCE" },
                        { ClassKeys.Heavy.Name, "ENDURANCE" },
                        { ClassKeys.Sniper.Name, "ENDURANCE" },
                    } },
                    { FactionKeys.PU, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "PEPPER CLOUD" },
                        { ClassKeys.Heavy.Name, "PEPPER CLOUD" },
                        { ClassKeys.Sniper.Name, "PEPPER CLOUD" },
                        { ClassKeys.Technician.Name, "AR TARGETING" },
                        { ClassKeys.Infiltrator.Name, "PAIN CHAMELEON" }
                    } },
                    { FactionKeys.FS, new Dictionary<string, string>
                    {
                        { ClassKeys.Assault.Name, "SONIC BLAST" },
                        { ClassKeys.Berserker.Name, "SONIC BLAST" },
                        { ClassKeys.Priest.Name, "RESURRECT" },
                    } },
                })
        };

        // Exclusion map for random distributed skills
        public Dictionary<string, List<string>> RadomSkillExclusionMap = new Dictionary<string, List<string>>
        {
            { "HANDGUN PROFICIENCY", new List<string> { ClassKeys.Sniper.Name, ClassKeys.Berserker.Name } },
            { "PDW PROFICIENCY", new List<string> { ClassKeys.Technician.Name } },
            { "MELEE WEAPON PROFICIENCY+", new List<string> { ClassKeys.Berserker.Name } },
            { "ASSAULT RIFLE PROFICIENCY", new List<string> { ClassKeys.Assault.Name } },
            { "SHOTGUN PROFICIENCY", new List<string> { ClassKeys.Assault.Name } },
            { "SNIPER RIFLE PROFICIENCY", new List<string> { ClassKeys.Sniper.Name } },
            { "HEAVY WEAPON PROFICIENCY", new List<string> { ClassKeys.Heavy.Name } },
            { "MOUNTED WEAPON PROFICIENCY", new List<string> { ClassKeys.Heavy.Name } },
            { Proficiency.VW, new List<string> { ClassKeys.Priest.Name } },
            { Proficiency.SW, new List<string> { ClassKeys.Infiltrator.Name } },
            { "ROCKETEER", new List<string> { ClassKeys.Heavy.Name } },
            { "STEALTH SPECIALIST", new List<string> { ClassKeys.Infiltrator.Name } }
        };

        // Learn the first personal ability = is set rigth from the start
        public bool LearnFirstPersonalSkill = true;

        // Activate story rework
        public bool ActivateStoryRework = false;

        // Flag if UI texts should be changed to default (Enlish) text or set by localization
        public bool DoNotLocalizeChangedTexts = true;
        // Create new ability dictionary as json file in mod directory
        public bool CreateNewJsonFiles = false;
        // DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        public int Debug = 1;
    }
}
