using HarmonyLib;
using StorageChest.Data;
using StorageChest.Helpers;
using System;
using System.Reflection;
using static DialogueNode;

namespace StorageChest.Patchers;

[HarmonyPatch(typeof(Luna))]
public class LunaPatch
{

    [HarmonyPatch("Start")]
    public static void Prefix(Luna __instance)
    { 
        if(__instance.gameObject.GetComponent<InventoryStorage>() == null)
            __instance.gameObject.AddComponent(typeof(InventoryStorage));


        if (Globals.newChoiceAdded) return;

        FieldInfo dialogueField = typeof(Luna).GetField("standardGreetings", BindingFlags.Instance | BindingFlags.NonPublic);
        if (dialogueField == null)
        {
            Plugin.Logger.LogError("Could not find the private 'standardGreetings' field on Luna.");
            return;
        }

        Dialogue dialogue = dialogueField.GetValue(__instance) as Dialogue;
        if (dialogue == null)
        {
            Plugin.Logger.LogError("Dialogue instance is null.");
            return;
        }
        Plugin.Logger.LogInfo("Dialogue instance retrieved.");

        if (dialogue.storyNodes == null || dialogue.storyNodes.Length == 0)
        {
            Plugin.Logger.LogError("Dialogue has no story nodes.");
            return;
        }
        DialogueNode targetNode = dialogue.storyNodes[1];
        Plugin.Logger.LogInfo("Target DialogueNode selected.");

        Choice newChoice = new Choice();
        newChoice.text = "Open Storage";

        newChoice.onClickTrigger = (Trigger)999;
        newChoice.isLeaveChoice = false;

        Choice[] oldChoices = targetNode.choices;
        int oldLength = oldChoices != null ? oldChoices.Length : 0;
        Choice[] newChoices = new Choice[oldLength + 1];
        if (oldChoices != null)
        {
            Array.Copy(oldChoices, newChoices, oldLength);
        }
        newChoices[oldLength] = newChoice;

        targetNode.choices = newChoices;
        Plugin.Logger.LogInfo("New choice successfully added to the first dialogue node.");

        Globals.newChoiceAdded = true;
    }
}


