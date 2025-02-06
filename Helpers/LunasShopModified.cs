﻿using StorageChest;
using StorageChest.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public static class LunasShopModified
{
    //inventory storage for the player
    public static GameObject playerItemsParentStorage = null;
    //inventory storage that Luna is holding for the player
    public static GameObject storageItemsParent = null;

    static Dictionary<GameObject, bool> shopUI = new();
    static List<GameObject> storageUI = new();

    static InventoryStorage inventoryStorage;
    static Inventory playerInventory;

    static List<InventoryStorageSlot> storageSlots = new();
    static List<InventoryStorageSlot> playerSlots = new();

    static GameObject inventoryStorageSlotCache = null;
    static GameObject inventorySlotReference = null;

    static int slotCount = 40;

    public static Action<InventoryStorageSlot> OnTransferItem;

    static bool isInit = false;
    static List<GameObject> destroyList = new();

    public static void Init()
    {
        Plugin.Logger.LogWarning($"Loading player");
        playerInventory = Inventory.instance;
        Plugin.Logger.LogWarning($"Loading inv storage");
        inventoryStorage = GameObject.Find("Luna Stand").GetComponent<InventoryStorage>();
        Plugin.Logger.LogWarning($"getting slot count");
        slotCount = inventoryStorage.space;

        playerItemsParentStorage = null;
        storageItemsParent = null;
        shopUI = new();
        storageUI = new();
        storageSlots = new();
        playerSlots = new();
        destroyList = new();



        SetupUI();
        OnTransferItem += TransferItem;

        isInit = true;

        UpdateUI();
    }

    public static void OpenStorage()
    {
        Plugin.Logger.LogInfo("Open Storage.");

        if (CheckNeedsInit())
            Init();
        else
        {
            storageUI.ForEach(go => go.SetActive(true));
            foreach (var obj in shopUI.Keys)
            {
                obj.SetActive(false);
            }
        }

        inventoryStorage.LoadData();
    }

    private static void TransferItem(InventoryStorageSlot slot)
    {
        Plugin.Logger.LogWarning("TransferItem called.");

        if (slot.slotType == InventoryStorageSlot.SlotType.Player)
        {
            Plugin.Logger.LogWarning("TransferItem called on player slot.");
            if (slot.item != null)
            {
                Plugin.Logger.LogWarning("Slot has item.");
                if (inventoryStorage.AddItem(slot.item))
                {
                    Plugin.Logger.LogWarning("Item added to player inventory.");
                    playerInventory.RemoveItem(slot.item);
                    slot.ClearSlot();
                }
            }
        }
        else if (slot.slotType == InventoryStorageSlot.SlotType.Storage)
        {
            Plugin.Logger.LogWarning("TransferItem called on storage slot.");
            if (slot.item != null)
            {
                Plugin.Logger.LogWarning("Slot has item.");
                if (playerInventory.AddItem(slot.item))
                {
                    Plugin.Logger.LogWarning("Item added to storage.");
                    inventoryStorage.RemoveItem(slot.item);
                    slot.ClearSlot();
                }
            }
        }

        UpdateUI();
    }

    private static void CloseShop()
    {
        storageUI.ForEach(go => go.SetActive(false));
        foreach (var obj in shopUI.Keys)
        {
            obj.SetActive(shopUI[obj]);
        }
    }

    public static void UpdateUI()
    {
        if (!isInit)
        {
            Plugin.Logger.LogWarning("UI not initialized.");
            return;
        }

        Plugin.Logger.LogInfo("Update player inventory UI.");

        for (int i = 0; i < playerInventory.space; i++)
        {
            if (i < playerInventory.items.Count)
            {
                playerSlots[i].CreateItem(playerInventory.items[i]);
            }
            else
            {
                playerSlots[i].ClearSlot();
            }
        }

        for (int i = 0; i < inventoryStorage.space; i++)
        {
            if (i < inventoryStorage.items.Count)
            {
                storageSlots[i].CreateItem(inventoryStorage.items[i]);
            }
            else
            {
                storageSlots[i].ClearSlot();
            }
        }
    }

    private static bool CheckNeedsInit()
    {
        bool result = playerItemsParentStorage == null || storageItemsParent == null;
        Plugin.Logger.LogInfo($"Check if needs re-init: {result}");
        return result;
    }

    #region UI setup
    private static void SetupUI()
    {
        GameObject UI = GetUIGameObject();
        if (UI == null)
        {
            Plugin.Logger.LogError("Could not find the 'UI' GameObject.");
            return;
        }
        SetupInventories(UI);
    }

    private static GameObject GetUIGameObject()
    {
        LunasShop shop = GameObject.FindObjectOfType<LunasShop>();
        if (shop == null)
        {
            Plugin.Logger.LogWarning("No LunasShop instance found in the scene.");
            return null;
        }
        Plugin.Logger.LogInfo("Found LunasShop instance.");

        FieldInfo shopUIField = typeof(LunasShop).GetField("shopUI", BindingFlags.Instance | BindingFlags.NonPublic);
        if (shopUIField == null)
        {
            Plugin.Logger.LogError("Could not find the 'shopUI' field on LunasShop.");
            return null;
        }

        GameObject shopUI = shopUIField.GetValue(shop) as GameObject;
        if (shopUI == null)
        {
            Plugin.Logger.LogError("'shopUI' is null.");
            return null;
        }
        Plugin.Logger.LogInfo($"Retrieved shopUI: {shopUI.name}");

        return shopUI;
    }

    private static void SetupInventories(GameObject shopUI)
    {
        var tableArea = shopUI.transform.GetChild(0);

        for (int i = 0; i < tableArea.childCount; i++)
        {
            var child = tableArea.GetChild(i);
            if (child.name == "Store Items Parent")
            {
                LunasShopModified.shopUI.Add(child.gameObject, child.gameObject.activeSelf);
                child.gameObject.SetActive(false);
            }
            else if (child.name == "Player Items Parent")
            {
                Plugin.Logger.LogInfo("caching InventoryShopSlot");
                inventorySlotReference = child.GetChild(0).gameObject;


                Plugin.Logger.LogInfo("Creating InventoryStorageSlots");
                var playerStorageInventoryLayout = GameObject.Instantiate(child.gameObject, child.parent);
                playerStorageInventoryLayout.name = "Player Items Parent-Storage"; //cache this

                playerItemsParentStorage = playerStorageInventoryLayout;
                storageUI.Add(playerItemsParentStorage);

                Plugin.Logger.LogInfo("Disabling old player inventory");
                if (!LunasShopModified.shopUI.ContainsKey(child.gameObject))
                    LunasShopModified.shopUI.Add(child.gameObject, child.gameObject.activeSelf);
                child.gameObject.SetActive(false);

                Plugin.Logger.LogInfo("removing slots for player inventory. childCount: " + playerItemsParentStorage.transform.childCount);
                //turn off InventorySlot and replace with InventoryStorageSlot

                for (; playerItemsParentStorage.transform.childCount > 0;)
                {
                    MoveOutAndDestroy(playerItemsParentStorage.transform.GetChild(0));
                }

                playerInventory.StartCoroutine(DestroyDelay());

                for (int j = 0; j < playerInventory.space; j++)
                {
                    //yield return null;
                    Plugin.Logger.LogInfo("Adding new slot " + j);
                    var newSlot = GameObject.Instantiate(inventorySlotReference, playerItemsParentStorage.transform);
                    newSlot.name = "InventoryStorageSlot";

                    var inventoryStorageItem = newSlot.AddComponent<InventoryStorageSlot>();
                    inventoryStorageItem.Init(OnTransferItem, inventorySlotReference.GetComponent<InventorySlotShop>(), InventoryStorageSlot.SlotType.Player);
                    inventoryStorageItem.ClearSlot();

                    GameObject.Destroy(newSlot.GetComponent<InventorySlotShop>());

                    playerSlots.Add(inventoryStorageItem);

                    if (inventoryStorageSlotCache == null)
                        inventoryStorageSlotCache = inventoryStorageItem.gameObject;
                }
                SetupStorage();
            }
            else if (child.name == "Player Info Layout")
            {
                for (int j = 0; j < child.childCount; j++)
                {
                    var innerChild = child.GetChild(j);
                    if (innerChild.name == "Leave Button")
                    {
                        innerChild.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            Plugin.Logger.LogInfo("Leave Button clicked.");
                            CloseShop();

                        });
                    }
                }

            }
        }
    }

    private static void SetupStorage()
    {
        Plugin.Logger.LogInfo($"Setting up storage");
        //copy of player inventory to make storage
        var newGO = GameObject.Instantiate(playerItemsParentStorage, playerItemsParentStorage.transform.parent);
        newGO.name = "Storage Items Parent"; //cache this

        storageItemsParent = newGO;
        storageUI.Add(storageItemsParent);

        var gridComp = newGO.GetComponent<GridLayoutGroup>();
        gridComp.constraintCount = 8;

        var rect = storageItemsParent.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(-300, rect.offsetMin.y);
        rect.offsetMax = new Vector2(-300, rect.offsetMax.y);

        Plugin.Logger.LogInfo($"Deleting old slots");

        for (; storageItemsParent.transform.childCount > 0;)
        {
            MoveOutAndDestroy(storageItemsParent.transform.GetChild(0));
        }

        playerInventory.StartCoroutine(DestroyDelay());

        Plugin.Logger.LogInfo($"adding new slots {slotCount}");

        //add storage slots
        for (int j = 0; j < slotCount; j++)
        {
            InventoryStorageSlot newSlot = GameObject.Instantiate(inventoryStorageSlotCache, storageItemsParent.transform).GetComponent<InventoryStorageSlot>();
            newSlot.Init(OnTransferItem, inventoryStorageSlotCache.GetComponent<InventorySlotShop>(), InventoryStorageSlot.SlotType.Storage);

            GameObject.Destroy(newSlot.GetComponent<InventorySlotShop>());

            storageSlots.Add(newSlot);
        }
    }
    #endregion

    #region utility methods
    private static void MoveOutAndDestroy(Transform toDestroy)
    {
        //Destroy likes to take it's time, so we'll just move everything out of the way while it's doing its thing
        //plus we don't like to destroy things in the middle of a loop
        toDestroy.SetParent(toDestroy.parent.root);
        destroyList.Add(toDestroy.gameObject);
    }

    private static IEnumerator DestroyDelay()
    {
        while (destroyList.Count > 0)
        {
            yield return null;

            if (destroyList.Count == 0)
                yield break;

            GameObject obj = destroyList[0].gameObject;
            destroyList.RemoveAt(0);
            GameObject.Destroy(obj);
        }
    }

    #endregion
}
