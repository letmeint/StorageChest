//using StorageChest.Helpers;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEngine;

//namespace StorageChest.Data;

//[Serializable]
//internal struct SpriteData
//{
//    Sprite sprite;
//    public string SpritePath { get; set; }
//    public string[] HypnoTypes { get; set; }
//    public int XpGiven { get; set; }

//    [JsonIgnore]
//    public Sprite Sprite
//    {
//        get
//        {
//            if (sprite == null)
//            {
//                sprite = ImageHelper.LoadSprite(SpritePath);
//            }
//            return sprite;
//        }
//    }
//}

//internal class SpriteDataCollection
//{
//    public List<SpriteData> SpriteData { get; set; }
//}


//internal static class SpriteDataHelper
//{
//    internal static Dictionary<string, SpriteData> HypnoInstances = new Dictionary<string, SpriteData>();

//    internal static void CreateSpriteData()
//    {
//        Plugin.Logger.LogWarning($"starting.");

//        if (!Directory.Exists(ConfigFile.spriteFolderPath.Value))
//        {
//            return;
//        }

//        //load from files
//        var dataFiles = Directory.GetFiles(ConfigFile.VRHeadSetDataPath.Value, "*.json", SearchOption.AllDirectories);
//        SpriteDataCollection masterData = new SpriteDataCollection();
//        masterData.SpriteData = new List<SpriteData>();

//        LoadFromDataFiles(dataFiles, masterData);

//        LoadFromLooseImages(masterData);
//    }

//    private static void LoadFromLooseImages(SpriteDataCollection masterData)
//    {
//        var files = Directory.GetFiles(ConfigFile.spriteFolderPath.Value, "*.png");
//        foreach (var file in files)
//        {
//            var fileName = Path.GetFileNameWithoutExtension(file);
//            if (HypnoInstances.ContainsKey(fileName))
//            {
//                Plugin.Logger.LogInfo($"{fileName} already exists. Skipping.");
//                continue;
//            }

//            var sd = new SpriteData()
//            {
//                SpritePath = file,
//                HypnoTypes = new string[] { "Anal", "Oral", "Sissy", "Cum", "Masturbation", "Bimbo", "Feet" },
//                XpGiven = 1
//            };

//            masterData.SpriteData.Add(sd);

//            _ = sd.Sprite; //cache it

//            HypnoInstances.Add(fileName, sd);
//        }
//    }

//    private static void LoadFromDataFiles(string[] dataFiles, SpriteDataCollection masterData)
//    {
//        foreach (var dataFile in dataFiles)
//        {
//            SpriteDataCollection data = IOHelper.ReadJson<SpriteDataCollection>(dataFile);
//            masterData.SpriteData.AddRange(data.SpriteData);
//        }

//        masterData.SpriteData = masterData?.SpriteData?.GroupBy(x => x.SpritePath).Select(y => y.First()).ToList();

//        foreach (var sd in masterData.SpriteData)
//        {
//            if (sd.Sprite == null) //cache it
//                continue;

//            var fileName = Path.GetFileNameWithoutExtension(sd.SpritePath);
//            if (!HypnoInstances.ContainsKey(fileName))
//                HypnoInstances.Add(fileName, sd);
//        }
//    }
//}
