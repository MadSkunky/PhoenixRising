using System.Collections.Generic;

namespace PhoenixRising.SkillRework
{
    internal class Settings
    {
        // Class skills for 1st and 3rd row
        //public List<ClassSpec> ClassSpec = new List<ClassSpec>
        //    {
        //    new ClassSpec(ClassList.Assault,"QUICK AIM","OVERWATCH FOCUS","READY FOR ACTION","ONSLAUGHT","RAPID CLEARANCE","TROOPER","QUARTERBACK"),
        //    new ClassSpec(ClassList.Heavy,"RETURN FIRE","WAR CRY","BOOM BLAST","HEAVY LIFTER","RAGE BURST","STRONGMAN","EXPERT HEAVY WEAPONS"),
        //    new ClassSpec(ClassList.Sniper,"EXTREME FOCUS","ARMOR BREAK","MASTER MARKSMAN","INSPIRE","MARKED FOR DEATH","EXPERT RIFLES AND HANDGUNS","SNIPERIST"),
        //    new ClassSpec(ClassList.Berserker,"DASH","CLOSE QUARTERS EVADE","BLOODLUST","IGNORE PAIN","ADRENALINE RUSH","BRAWLER","EXPERT MELEE"),
        //    new ClassSpec(ClassList.Priest,"MIND CONTROL","INDUCE PANIC","MIND SENSE","PSYCHIC WARD","MIND CRUSH","FARSIGHTED","BIOCHEMIST"),
        //    new ClassSpec(ClassList.Technician,"FAST USE","REMOTE CONTROL","FIELD MEDIC","REMOTE DEPLOYMENT","ELECTRIC REINFORCEMENT","SELF DEFENSE SPECIALIST","HEALER"),
        //    new ClassSpec(ClassList.Infiltrator,"SURPRISE ATTACK","DEPLOY DECOY","WEAK SPOT","VANISH","SNEAK ATTACK","THIEF","CAUTIOUS")
        //    };

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClassSpecs = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
        {
            { ClassKey.Assault.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Assault.Value },
                    { ClassKey.Level_2, "QUICK AIM" },
                    { ClassKey.Level_3, "OVERWATCH FOCUS" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "READY FOR ACTION" },
                    { ClassKey.Level_6, "ONSLAUGHT" },
                    { ClassKey.Level_7, "RAPID CLEARANCE" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> { 
                    { ClassKey.PersSpec_1, "TROOPER" },
                    { ClassKey.PersSpec_2, "QUARTERBACK" } } }
            } },
            { ClassKey.Heavy.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Heavy.Value },
                    { ClassKey.Level_2, "RETURN FIRE" },
                    { ClassKey.Level_3, "WAR CRY" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "BOOM BLAST" },
                    { ClassKey.Level_6, "HEAVY LIFTER" },
                    { ClassKey.Level_7, "RAGE BURST" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "STRONGMAN" },
                    { ClassKey.PersSpec_2, "EXPERT HEAVY WEAPONS" } } }
            } },
            { ClassKey.Sniper.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Sniper.Value },
                    { ClassKey.Level_2, "EXTREME FOCUS" },
                    { ClassKey.Level_3, "ARMOR BREAK" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "MASTER MARKSMAN" },
                    { ClassKey.Level_6, "INSPIRE" },
                    { ClassKey.Level_7, "MARKED FOR DEATH" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "EXPERT RIFLES AND HANDGUNS" },
                    { ClassKey.PersSpec_2, "SNIPERIST" } } }
            } },
            { ClassKey.Berserker.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Berserker.Value },
                    { ClassKey.Level_2, "DASH" },
                    { ClassKey.Level_3, "CLOSE QUARTERS EVADE" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "BLOODLUST" },
                    { ClassKey.Level_6, "IGNORE PAIN" },
                    { ClassKey.Level_7, "ADRENALINE RUSH" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "BRAWLER" },
                    { ClassKey.PersSpec_2, "EXPERT MELEE" } } }
            } },
            { ClassKey.Priest.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Priest.Value },
                    { ClassKey.Level_2, "MIND CONTROL" },
                    { ClassKey.Level_3, "INDUCE PANIC" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "MIND SENSE" },
                    { ClassKey.Level_6, "PSYCHIC WARD" },
                    { ClassKey.Level_7, "MIND CRUSH" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "FARSIGHTED" },
                    { ClassKey.PersSpec_2, "BIOCHEMIST" } } }
            } },
            { ClassKey.Technician.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                     { ClassKey.Level_1, ClassKey.Technician.Value },
                   { ClassKey.Level_2, "FAST USE" },
                    { ClassKey.Level_3, "REMOTE CONTROL" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "FIELD MEDIC" },
                    { ClassKey.Level_6, "REMOTE DEPLOYMENT" },
                    { ClassKey.Level_7, "ELECTRIC REINFORCEMENT" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "SELF DEFENSE SPECIALIST" },
                    { ClassKey.PersSpec_2, "HEALER" } } }
            } },
            { ClassKey.Infiltrator.Key, new Dictionary<string, Dictionary<string,string>> {
                { ClassKey.MainSpecs, new Dictionary<string,string> {
                    { ClassKey.Level_1, ClassKey.Infiltrator.Value },
                    { ClassKey.Level_2, "SURPRISE ATTACK" },
                    { ClassKey.Level_3, "DEPLOY DECOY" },
                    { ClassKey.Level_4, "" },
                    { ClassKey.Level_5, "WEAK SPOT" },
                    { ClassKey.Level_6, "VANISH" },
                    { ClassKey.Level_7, "SNEAK ATTACK" } } },
                { ClassKey.PersSpecs, new Dictionary<string,string> {
                    { ClassKey.PersSpec_1, "THIEF" },
                    { ClassKey.PersSpec_2, "CAUTIOUS" } } }
            } },
        };

        // Faction related skills for 3rd row
        public Dictionary<string, Dictionary<string, string>> FactionSkills = new Dictionary<string, Dictionary<string, string>>
        {
            { Faction.PX, new Dictionary<string, string> { { PersonalLevel.FS1, "GYM RAT" },{ PersonalLevel.FS2, "RAGE BURST+" } } },
            { Faction.Anu, new Dictionary<string, string> { { PersonalLevel.FS1, "CLOSE QUARTERS SPECIALIST" }, { PersonalLevel.FS2, "RALLY THE TROOPS" } } },
            { Faction.NJ, new Dictionary<string, string> { { PersonalLevel.FS1, "BOMBARDIER" },{ PersonalLevel.FS2, "TURRET COMBO" } } },
            { Faction.Syn, new Dictionary<string, string> { { PersonalLevel.FS1, "PAIN CHAMELEON+" },{ PersonalLevel.FS2, "DEADLY DUO" } } },
            { Faction.IN, new Dictionary<string, string> { { PersonalLevel.FS1, "Random" },{ PersonalLevel.FS2, "Random" } } }
        };

        // Additional prficiency skills
        public Dictionary<string, Dictionary<string,string>> ProficiencySkills = new Dictionary<string, Dictionary<string, string>>
        {
            { Proficiency.HG, new Dictionary<string, string> { { PersonalLevel.PS1, "HANDGUN PROFICIENCY" },{ PersonalLevel.PS2, "EXPERT RIFLES AND HANDGUNS" } } },
            { Proficiency.PD, new Dictionary<string, string> { { PersonalLevel.PS1, "PDW PROFICIENCY" },{ PersonalLevel.PS2, "SELF DEFENSE SPECIALIST" } } },
            { Proficiency.ML, new Dictionary<string, string> { { PersonalLevel.PS1, "MELEE WEAPON PROFICIENCY+" },{ PersonalLevel.PS2, "EXPERT MELEE" } } },
            { Proficiency.AR, new Dictionary<string, string> { { PersonalLevel.PS1, "ASSAULT RIFLE PROFICIENCY" },{ PersonalLevel.PS2, "TROOPER" } } },
            { Proficiency.SG, new Dictionary<string, string> { { PersonalLevel.PS1, "SHOTGUN PROFICIENCY" },{ PersonalLevel.PS2, "CLOSE QUARTERS SPECIALIST" } } },
            { Proficiency.SR, new Dictionary<string, string> { { PersonalLevel.PS1, "SNIPER RIFLE PROFICIENCY" },{ PersonalLevel.PS2, "SNIPERIST" } } },
            { Proficiency.HW, new Dictionary<string, string> { { PersonalLevel.PS1, "HEAVY WEAPON PROFICIENCY" },{ PersonalLevel.PS2, "EXPERT HEAVY WEAPONS" } } },
            { Proficiency.MW, new Dictionary<string, string> { { PersonalLevel.PS1, "MOUNTED WEAPON PROFICIENCY" },{ PersonalLevel.PS2, "EXPERT MOUNTED WEAPONS" } } }
        };

        // Background perks
        public string[] BackgroundPerks = new string[]
        {
            "FIRE RESISTANT","POISON RESISTANT","VIRUS RESISTANT","RECKLESS",
            "DEVOTED","JETPACK PROFICIENCY","JUMP",
            "RESOURCEFUL","STEALTH SPECIALIST"
        };

        // Order of the 3rd row skills, BGP = BackgroundPerks
        public string[] OrderOfPersonalSkills = new string[]
        {
            PersonalLevel.BGP,
            PersonalLevel.FS1,
            PersonalLevel.FS2,
            PersonalLevel.CS1,
            PersonalLevel.CS2,
            PersonalLevel.PS1,
            PersonalLevel.PS2
        };

        // Exclusion map for random distributed skills
        public Dictionary<string, List<string>> RadomSkillExclusionMap = new Dictionary<string, List<string>>
        {
            { Proficiency.HG, new List<string> { ClassKey.Sniper.Key, ClassKey.Berserker.Key } },
            { Proficiency.PD, new List<string> { ClassKey.Technician.Key } },
            { Proficiency.ML, new List<string> { ClassKey.Berserker.Key } },
            { Proficiency.AR, new List<string> { ClassKey.Assault.Key } },
            { Proficiency.SG, new List<string> { ClassKey.Assault.Key } },
            { Proficiency.SR, new List<string> { ClassKey.Sniper.Key } },
            { Proficiency.HW, new List<string> { ClassKey.Heavy.Key } },
            { Proficiency.MW, new List<string> { ClassKey.Heavy.Key } },
            { Proficiency.VW, new List<string> { ClassKey.Priest.Key } },
            { Proficiency.SW, new List<string> { ClassKey.Infiltrator.Key } },
            { "JETPACK PROFICIENCY", new List<string> { ClassKey.Heavy.Key } },
            { "STEALTH SPECIALIST", new List<string> { ClassKey.Infiltrator.Key} }
        };

        // Create new ability dictionary as json file in mod directory
        public bool CreateNewJsonForAbilities = false;
        // DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        public int Debug = 1;

    }
}
