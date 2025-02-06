using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace StorageChest.Patchers;

//[HarmonyPatch(typeof(EnemySpawner))]
public class EnemySpawnerPatch
{
    static GameObject chestPrefab;

    //[HarmonyPatch("Start")]
    public static void Prefix()
    {
        Plugin.Logger.LogInfo("EnemySpawner Start Prefix");
        GetChestPrefab();

        GameObject gameObject = new();
        gameObject.transform.position = Vector2.zero;

        Chest chestInstance = GameObject.Instantiate(chestPrefab, gameObject.transform.position, gameObject.transform.rotation).GetComponent<Chest>();

        chestInstance.StartCoroutine(SetupChest());

        IEnumerator SetupChest()
        {
            yield return new WaitForEndOfFrame();
           
            var interactible = chestInstance.GetComponent<ActiveInteractible>();
            interactible.onInteractionCallback = () =>
            {
                Plugin.Logger.LogError("Chest was interacted with.");
            };
            
            GameObject.Destroy(chestInstance.GetComponent<LootBag>());
            GameObject.Destroy(chestInstance.GetComponent<Health>());
            GameObject.Destroy(chestInstance.GetComponent<Animator>());

        }

        GameObject.Destroy(gameObject);

    }

    private static GameObject GetChestPrefab()
    {
        Plugin.Logger.LogInfo("MyPlugin Start: Looking for ObjectSpawner instance...");

        ObjectSpawner spawner = Object.FindObjectOfType<ObjectSpawner>();
        if (spawner == null)
        {
            Plugin.Logger.LogError("Could not find an ObjectSpawner in the scene!");
            return null;
        }
        Plugin.Logger.LogInfo("Found ObjectSpawner instance.");

        Type spawnerType = typeof(ObjectSpawner);
        FieldInfo spawnableObjectsField = spawnerType.GetField("spawnableObjects", BindingFlags.Instance | BindingFlags.NonPublic);
        if (spawnableObjectsField == null)
        {
            Plugin.Logger.LogError("Could not find the private field 'spawnableObjects'.");
            return null;
        }

        object spawnableObjectsValue = spawnableObjectsField.GetValue(spawner);
        if (spawnableObjectsValue == null)
        {
            Plugin.Logger.LogError("'spawnableObjects' field is null.");
            return null;
        }

        IEnumerable spawnableObjectsEnumerable = spawnableObjectsValue as IEnumerable;
        if (spawnableObjectsEnumerable == null)
        {
            Plugin.Logger.LogError("'spawnableObjects' is not enumerable.");
            return null;
        }

        Type spawnableObjectType = spawnerType.GetNestedType("SpawnableObject", BindingFlags.Public);
        if (spawnableObjectType == null)
        {
            Plugin.Logger.LogError("Could not get the nested type 'SpawnableObject'.");
            return null;
        }

        FieldInfo prefabField = spawnableObjectType.GetField("objectPrefab", BindingFlags.Public | BindingFlags.Instance);
        if (prefabField == null)
        {
            Plugin.Logger.LogError("Could not find the field 'objectPrefab' in SpawnableObject.");
            return null;
        }

        foreach (object item in spawnableObjectsEnumerable)
        {
            GameObject prefab = prefabField.GetValue(item) as GameObject;
            if (prefab == null)
                continue;

            Plugin.Logger.LogError(prefab.name);

            if (prefab.name.Equals("Chest", StringComparison.Ordinal))
            {
                Plugin.Logger.LogInfo($"Found the Chest prefab: {prefab}");
                chestPrefab = prefab;

                if (chestPrefab == null)
                {
                    Plugin.Logger.LogError("Chest prefab is null.");
                    return null;
                }

                return chestPrefab;
            }

        }
        Plugin.Logger.LogWarning("Could not find the Chest prefab.");
        return null;
    }
}

