using HarmonyLib;

namespace StorageChest.Patchers;

[HarmonyPatch(typeof(CorruptionManager))]
public class CorruptionManagerPatch
{
    [HarmonyPatch("StartEventCheck")]
    public static bool Prefix(CorruptionManager __instance, ref bool __result)
    {
        Plugin.Logger.LogInfo("Starting Event Check");

        if (EnemySpawner.instance == null) return true;

        bool hasPauseReason = (bool)AccessTools.Field(typeof(EnemySpawner), "hasReasonToPause").GetValue(EnemySpawner.instance);

        if (hasPauseReason)
        {
            Plugin.Logger.LogInfo("Paused, not doing corruption events");
            __result = false;
            return false;//do not run rest of method
        }
         
        return true;
    }
}


