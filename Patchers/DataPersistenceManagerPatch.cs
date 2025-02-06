using HarmonyLib;
using StorageChest.Data;
using System.Collections.Generic;

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

    // turn this off for now. Let the inventorystorage load the data on Start

    //[HarmonyPatch(typeof(DataPersistenceManager), "LoadGame")]
    //[HarmonyPostfix]
    //public static void LoadGamepPost(DataPersistenceManager __instance)
    //{
    //    Plugin.Logger.LogInfo("Loading Storage");

    //    //LoadData();

    //    if (InventoryStorage.instance != null)
    //        InventoryStorage.instance.LoadData();
    //    else
    //    {
    //        Plugin.Logger.LogInfo("InventoryStorage is null, waiting for it to be initialized");
    //        __instance.StartCoroutine(WaitForInit());
    //        IEnumerator WaitForInit()
    //        {
    //            yield return new WaitUntil(() => InventoryStorage.instance != null);
    //            InventoryStorage.instance.LoadData();

    //        }

    //    }
    //}
}


