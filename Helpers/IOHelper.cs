using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace StorageChest.Helpers;

internal static class IOHelper
{
    internal static void CreateFolders()
    {
        if (!Directory.Exists(Globals.defaultStorageSavePath))
        {
            Directory.CreateDirectory(Globals.defaultStorageSavePath);
        }
    }

    internal static JObject ReadJson(string saveFile)
    {
        if (!File.Exists(saveFile))
        {
            return new JObject();
        }

        using (var textReader = File.OpenText(saveFile))
        {
            using (var reader = new JsonTextReader(textReader))
            {
                reader.FloatParseHandling = FloatParseHandling.Double;

                return JObject.Load(reader);
            }
        }
    }

    internal static void WriteJson(string saveFile, JObject state)
    {
        using (var textWriter = File.CreateText(saveFile))
        {
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                state.WriteTo(writer);
            }
        }
    }
}

