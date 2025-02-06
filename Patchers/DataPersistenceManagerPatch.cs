using HarmonyLib;
using StorageChest.Data;

namespace StorageChest.Patchers;

[HarmonyPatch(typeof(DataPersistenceManager))]
public class DataPersistenceManagerPatch
{

    [HarmonyPatch("SaveGame")]
    [HarmonyPostfix]
    public static void SaveGamePost(DataPersistenceManager __instance)
    {
        Plugin.Logger.LogInfo("Saving Storage");
        if(InventoryStorage.instance != null)
            InventoryStorage.instance.SaveData();

    }

    //[HarmonyPatch("LoadGame")]
    //[HarmonyPostfix]
    //public static void LoadGamepost(DataPersistenceManager __instance)
    //{
    //    Plugin.Logger.LogInfo("Loading Storage");
    //    if (InventoryStorage.instance != null)
    //        InventoryStorage.instance.LoadData(null);
    //    else
    //    {
    //        Plugin.Logger.LogInfo("InventoryStorage is null, waiting for it to be initialized");
    //    }
    //}
}


