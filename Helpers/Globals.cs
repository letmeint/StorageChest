using BepInEx;
using System.IO;

namespace StorageChest.Helpers;

internal static class Globals
{
    internal static string defaultGamePath = Path.Combine(Paths.PluginPath, "Mods");
    internal static string defaultStorageSavePath = Path.Combine(defaultGamePath, @"StorageSaves");

}

