using HarmonyLib;
using UnityEngine.UI;
using static DialogueNode;

namespace StorageChest.Patchers;

[HarmonyPatch(typeof(ChoiceController))]
public class ChoiceControllerPatch
{

    [HarmonyPatch("SetUpClick")]
    public static bool Prefix(ChoiceController __instance)
    {
        Plugin.Logger.LogInfo("ChoiceController SetUpClick Prefix");
        if (__instance.choice.onClickTrigger == (Trigger)999)
        {
            __instance.button.enabled = true;
            __instance.gameObject.GetComponent<Button>().onClick.AddListener(__instance.EndDialogue);
            __instance.gameObject.GetComponent<Button>().onClick.AddListener(__instance.OpenLunasShop);
            __instance.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                LunasShopModified shopMod = UnityEngine.Object.FindObjectOfType<LunasShopModified>();
                if (shopMod != null)
                {
                    shopMod.OpenStorage();
                }
            });
            return false;
        }


        return true;
    }
}


