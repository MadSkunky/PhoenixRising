﻿using Base.Core;
using Base.Defs;
using Harmony;
using I2.Loc;
using Newtonsoft.Json;
using PhoenixPoint.Common.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhoenixRising.BetterClasses
{
    internal class Helper
    {
        // Get config, definition repository (and shared data, not neccesary currently)
        private static readonly Settings Config = BetterClassesMain.Config;
        private static readonly DefRepository Repo = BetterClassesMain.Repo;
        private static readonly SharedData Shared = BetterClassesMain.Shared;

        internal static string ModDirectory;
        internal static string ManagedDirectory;
        internal static string TexturesDirectory;
        internal static string LocalizationDirectory;

        public static readonly string SkillLocalizationFileName = "PR_BC_Localization.csv";
        public static readonly string FsStoryLocalizationFileName = "PR_FS_Story_Localization.csv";

        // SP cost for main specialisation skills per level
        public static readonly int[] SPperLevel = new int[] { 0, 10, 15, 0, 20, 25, 30 };

        // Desearialize dictionary from Json to map ability names to Defs
        public static readonly string AbilitiesJsonFileName = "AbilityDefToNameDict.json";
        public static Dictionary<string, string> AbilityNameToDefMap;

        // Desearialize dictionary from Json to map non localized texts to ViewDefs
        public static readonly string TextMapFileName = "NotLocalizedTextMap.json";
        public static Dictionary<string, Dictionary<string, string>> NotLocalizedTextMap;

        public static void Initialize()
        {
            try
            {
                ModDirectory = BetterClassesMain.ModDirectory;
                ManagedDirectory = BetterClassesMain.ManagedDirectory;
                TexturesDirectory = BetterClassesMain.TexturesDirectory;
                LocalizationDirectory = BetterClassesMain.LocalizationDirectory;
                if (File.Exists(Path.Combine(LocalizationDirectory, SkillLocalizationFileName)))
                {
                    AddLocalizationFromCSV(SkillLocalizationFileName, null);
                }
                if (File.Exists(Path.Combine(LocalizationDirectory, FsStoryLocalizationFileName)))
                {
                    AddLocalizationFromCSV(FsStoryLocalizationFileName, null);
                }
                AbilityNameToDefMap = ReadJson<Dictionary<string, string>>(AbilitiesJsonFileName);
                NotLocalizedTextMap = ReadJson<Dictionary<string, Dictionary<string, string>>>(TextMapFileName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        // Read localization from CSV file
        public static void AddLocalizationFromCSV(string LocalizationFileName, string Category = null)
        {
            try
            {
                string CSVstring = File.ReadAllText(Path.Combine(LocalizationDirectory, LocalizationFileName));
                if (!CSVstring.EndsWith("\n"))
                {
                    CSVstring += "\n";
                }
                LanguageSourceData SourceToChange = Category == null ? // if category is not given
                    LocalizationManager.Sources[0] :                   // use fist language source
                    LocalizationManager.Sources.First(source => source.GetCategories().Contains(Category));
                if (SourceToChange != null)
                {
                    int numBefore = SourceToChange.mTerms.Count;
                    _ = SourceToChange.Import_CSV(string.Empty, CSVstring, eSpreadsheetUpdateMode.AddNewTerms, ',');
                    LocalizationManager.LocalizeAll(true);    // Force localing all enabled labels/sprites with the new data
                    int numAfter = SourceToChange.mTerms.Count;
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    Logger.Always($"Added {numAfter - numBefore} terms from {LocalizationFileName} in localization source {SourceToChange}, category: {Category}");
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                }
                else
                {
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    Logger.Always($"No language source with category {Category} found!");
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                }
                Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                Logger.Debug("CSV Data:" + Environment.NewLine + CSVstring);
                foreach (LanguageSourceData source in LocalizationManager.Sources)
                {
                    Logger.Debug($"Source owner {source.owner}{Environment.NewLine}Categories:{Environment.NewLine}{{source.GetCategories().Join()}}{Environment.NewLine}", false);
                }
                Logger.Debug("----------------------------------------------------------------------------------------------------", false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        // Creating new runtime def by cloning from existing def
        public static T CreateDefFromClone<T>(T source, string guid, string name) where T : BaseDef
        {
            try
            {
                Logger.Debug("CreateDefFromClone called ... ");
                Logger.Debug($"CreateDefFromClone, check if GUID <{guid}> already exist in Repo ...");
                if (Repo.GetDef(guid) != null)
                {
                    if (!(Repo.GetDef(guid) is T tmp))
                    {
                        throw new TypeAccessException($"An item with the GUID <{guid}> has already been added to the Repo, but the type <{Repo.GetDef(guid).GetType().Name}> does not match <{typeof(T).Name}>!");
                    }
                    else
                    {
                        if (tmp != null)
                        {
                            Logger.Debug($"CreateDefFromClone, <{guid}> already in Repo, <{tmp}> returned as result.");
                            return tmp;
                        }
                    }
                }
                Logger.Debug($"CreateDefFromClone, additional check if GUID <{guid}> already exist in Repo RuntimeDefs ...");
                T tmp2 = Repo.GetRuntimeDefs<T>(true).FirstOrDefault(rt => rt.Guid.Equals(guid));
                if (tmp2 != null)
                {
                    Logger.Debug($"CreateDefFromClone, <{guid}> already in Repo RunTimeDefs, <{tmp2}> returned as result.");
                    return tmp2;
                }
                Logger.Debug($"CreateDefFromClone, start name creation with parameter '{name}' ...");
                Type type = null;
                string resultName = "";
                if (source != null)
                {
                    int start = source.name.IndexOf('[') + 1;
                    int end = source.name.IndexOf(']');
                    string toReplace = !name.Contains("[") && start > 0 && end > start ? source.name.Substring(start, end - start) : source.name;
                    resultName = source.name.Replace(toReplace, name);
                    Logger.Debug($"CreateDefFromClone, name '{resultName}' created, start cloning from <{source.name}> with type <{source.GetType().Name}> ...");
                }
                else
                {
                    type = typeof(T);
                    resultName = name;
                    Logger.Debug($"CreateDefFromClone, name '{resultName}' created, start creating Def of type <{typeof(T).Name}> ...");
                }
                T result = (T)Repo.CreateRuntimeDef(
                    source,
                    type,
                    guid);
                result.name = resultName;
                Logger.Debug($"CreateDefFromClone, <{result.name}> of type <{result.GetType().Name}> sucessful created.");
                Logger.Debug("----------------------------------------------------", false);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public static Sprite CreateSpriteFromImageFile(string imageFileName, int width = 128, int height = 128, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipChain = true)
        {
            try
            {
                string filePath = Path.Combine(TexturesDirectory, imageFileName);
                byte[] data = File.Exists(filePath) ? File.ReadAllBytes(filePath) : throw new FileNotFoundException("File not found: " + filePath);
                Texture2D texture = new Texture2D(width, height, textureFormat, mipChain);
                return ImageConversion.LoadImage(texture, data)
                    ? Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f))
                    : null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
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
                string filePath = Path.Combine(ManagedDirectory, fileName);
                DateTime fileLastChanged = File.GetLastWriteTime(filePath);
                DateTime assemblyLastChanged = File.GetLastWriteTime(assembly.Location);
                if (source != null && source != "" && fileLastChanged < assemblyLastChanged)
                {
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    Logger.Always("Read JSON from assembly: " + source);
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    using (Stream stream = assembly.GetManifestResourceStream(source))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                if (json == null || json == "")
                {
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    Logger.Always("Read JSON from file: " + filePath);
                    Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    json = File.Exists(filePath) ? File.ReadAllText(filePath) : throw new FileNotFoundException(filePath);
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return default;
            }
        }

        // Write to external json file
        public static void WriteJson(string fileName, object obj, bool toFile = true)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
                if (toFile)
                {
                    //string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string filePath = Path.Combine(ManagedDirectory, fileName);
                    if (File.Exists(filePath))
                    {
                        File.WriteAllText(Path.Combine(ManagedDirectory, fileName), jsonString);
                        Logger.Always("----------------------------------------------------------------------------------------------------", false);
                        Logger.Always("Write JSON to file: " + filePath);
                        Logger.Always("----------------------------------------------------------------------------------------------------", false);
                    }
                    else
                    {
                        throw new FileNotFoundException(filePath);
                    }
                }
                // Writing in running assembly -- TODO: if really needed -> figure out to make it possible
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
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
