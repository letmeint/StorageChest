using HarmonyLib;

namespace StorageChest.Patchers;

[HarmonyPatch(typeof(LunasShop))]
public class LunasShopPatch
{
    [HarmonyPatch("Start")]
    public static void Prefix(LunasShop __instance)
    { 
        if(__instance.gameObject.GetComponent<LunasShopModified>() == null)
            __instance.gameObject.AddComponent(typeof(LunasShopModified));
      
        
    }
}


