using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhoenixRising.SkillRework
{//PhoenixRising.SkillRework.Settings
    internal class Settings
    {
        // Class skills for 1st and 3rd row
        public Dictionary<string, Dictionary<string, string[]>> ClassSkills = new Dictionary<string, Dictionary<string, string[]>>
        {
            { Helper.Classes[0], new Dictionary<string, string[]> {
                { Helper.Row[0], new string[] { "ASSAULT TRAINING", "QUICK AIM", "OVERWATCH FOCUS", "", "READY FOR ACTION", "ONSLAUGHT", "RAPID CLEARANCE" } },
                { Helper.Row[1], new string[] { "TROOPER", "QUARTERBACK" } }
            } },
            { Helper.Classes[1], new Dictionary<string, string[]> {
                { Helper.Row[0], new string[] { "HEAVY TRAINING", "RETURN FIRE", "WAR CRY", "", "BOOM BLAST", "HEAVY LIFTER", "RAGE BURST" } },
                { Helper.Row[1], new string[] { "STRONGMAN", "EXPERT HEAVY WEAPONS" } }
            } },
            { Helper.Classes[2], new Dictionary<string, string[]> {
                { Helper.Row[0], new string[] { "SNIPER TRAINING", "EXTREME FOCUS", "ARMOR BREAK", "", "MASTER MARKSMAN", "INSPIRE", "MARKED FOR DEATH" } },
                { Helper.Row[1], new string[] { "EXPERT RIFLES AND HANDGUNS", "SNIPERIST" } }
            } },
            { Helper.Classes[3], new Dictionary<string, string[]> {
                { Helper.Row[0], new string[] { "BERSERKER TRAINING", "DASH", "CLOSE QUARTERS EVADE", "", "BLOODLUST", "IGNORE PAIN", "ADRENALINE RUSH" } },
                { Helper.Row[1], new string[] { "BRAWLER", "EXPERT MELEE" } }
            } },
            { Helper.Classes[4] , new Dictionary < string, string[] > { 
                { Helper.Row[0], new string[] { "PRIEST TRAINING", "MIND CONTROL", "INDUCE PANIC", "", "MIND SENSE", "PSYCHIC WARD", "MIND CRUSH" } },
                { Helper.Row[1], new string[] { "FARSIGHTED", "BIOCHEMIST" } }
            } },
            { Helper.Classes[5] , new Dictionary < string, string[] > {
                { Helper.Row[0], new string[] { "TECHNICIAN TRAINING", "FAST USE", "REMOTE CONTROL", "", "FIELD MEDIC", "REMOTE DEPLOYMENT", "ELECTRIC REINFORCEMENT" } },
                { Helper.Row[1], new string[] { "SELF DEFENSE SPECIALIST", "HEALER" } }
            } },
            { Helper.Classes[6] , new Dictionary < string, string[] > {
                { Helper.Row[0], new string[] { "INFILTRATOR TRAINING", "SURPRISE ATTACK", "DEPLOY DECOY", "", "WEAK SPOT", "VANISH", "SNEAK ATTACK" } },
                { Helper.Row[1], new string[] { "THIEF", "CAUTIOUS" } }
            } }
        };

        // Faction related skills for 3rd row
        public Dictionary<string, string[]> FactionSkills = new Dictionary<string, string[]>
        {
            { Helper.Factions[0], new string[] { "GYM RAT", "RAGE BURST+" } },
            { Helper.Factions[1], new string[] { "CLOSE QUARTERS SPECIALIST", "RALLY THE TROOPS" } },
            { Helper.Factions[2], new string[] { "BOMBARDIER", "TURRET COMBO" } },
            { Helper.Factions[3], new string[] { "PAIN CHAMELEON+", "DEADLY DUO" } },
            { Helper.Factions[4], new string[] { "Random", "Random" } }
        };

        // Additional prficiency skills
        public Dictionary<string, string[]> ProficiencySkills = new Dictionary<string, string[]>
        {
            { Helper.Proficiencies[0], new string[] { "HANDGUN PROFICIENCY", "EXPERT RIFLES AND HANDGUNS" } },
            { Helper.Proficiencies[1], new string[] { "PDW PROFICIENCY", "SELF DEFENSE SPECIALIST" } },
            { Helper.Proficiencies[2], new string[] { "MELEE WEAPON PROFICIENCY+", "EXPERT MELEE" } },
            { Helper.Proficiencies[3], new string[] { "ASSAULT RIFLE PROFICIENCY", "TROOPER" } },
            { Helper.Proficiencies[4], new string[] { "SHOTGUN PROFICIENCY", "CLOSE QUARTERS SPECIALIST" } },
            { Helper.Proficiencies[5], new string[] { "SNIPER RIFLE PROFICIENCY", "EXPERT SHOOTER" } },
            { Helper.Proficiencies[6], new string[] { "HEAVY WEAPON PROFICIENCY", "EXPERT HEAVY WEAPONS" } },
            { Helper.Proficiencies[7], new string[] { "MOUNTED WEAPON PROFICIENCY", "EXPERT MOUNTED WEAPONS" } },
        };

        // Background perks
        public string[] BackgroundPerks = new string[]
        {
            "GYM RAT","FIRE RESISTANT","POISON RESISTANT","VIRUS RESISTANT","QUARTERBACK","RECKLESS","CAUTIOUS",
            "CLOSE QUARTERS EVADE","DEVOTED","FARSIGHTED","HEALER","HEAVY LIFTER","JETPACK PROFICIENCY","JUMP",
            "RESOURCEFUL","STEALTH SPECIALIST"
        };

        // Create new ability dictionary as json file in mod directory
        public bool CreateNewJsonForAbilities = false;
        // DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        public int Debug = 1;
    }
}
