using Base.Core;
using Base.Defs;
using Newtonsoft.Json;
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
        private static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        //private static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();

        internal static string ModDirectory;
        internal static string ManagedDirectory;
        internal static string TexturesDirectory;
        
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
                AbilityNameToDefMap = ReadJson<Dictionary<string, string>>(AbilitiesJsonFileName);
                NotLocalizedTextMap = ReadJson<Dictionary<string, Dictionary<string, string>>>(TextMapFileName);
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
                Type type = null;
                string resultName = "";
                if (source != null)
                {
                    Logger.Debug("CreateDefFromClone with source type: " + source.GetType().Name);
                    Logger.Debug("CreateDefFromClone with source name: " + source.name);
                    int start = source.name.IndexOf('[') + 1;
                    int end = source.name.IndexOf(']');
                    string toReplace = !name.Contains("[") && start > 0 && end > start ? source.name.Substring(start, end - start) : source.name;
                    resultName = source.name.Replace(toReplace, name);
                }
                else
                {
                    Logger.Debug("CreateDefFromClone only with type: " + typeof(T).Name);
                    type = typeof(T);
                    resultName = name;
                }
                T result = (T)Repo.CreateRuntimeDef(
                    source,
                    type,
                    guid);
                result.name = resultName;
                Logger.Debug("CreateDefFromClone result type: " + result.GetType().Name);
                Logger.Debug("CreateDefFromClone result name: " + result.name);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public static Sprite CreateSprite(string textureFileName, int width = 128, int height = 128)
        {
            string filePath = Path.Combine(BetterClassesMain.TexturesDirectory, textureFileName);
            Sprite newSprite = null;
            Texture2D texture = null;
            if (File.Exists(filePath) && LoadTexture2DfromFile(ref texture, filePath, width, height))
            {
                newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.0f, 0.0f));
            }
            return newSprite;
        }

        public static bool LoadTexture2DfromFile(ref Texture2D texture2D, string filePath, int width, int height, TextureFormat textureFormat = TextureFormat.ARGB32, bool mipChain = false)
        {
            try
            {
                byte[] data = File.Exists(filePath) ? File.ReadAllBytes(filePath) : throw new FileNotFoundException(filePath);
                Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                Logger.Debug("Create texture from file: " + filePath);
                Logger.Debug("----------------------------------------------------------------------------------------------------", false);
                texture2D = new Texture2D(width, height, textureFormat, mipChain);
                return ImageConversion.LoadImage(texture2D, data);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
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
