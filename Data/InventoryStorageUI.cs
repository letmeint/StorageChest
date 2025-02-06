using StorageChest.Data;
using UnityEngine;

public class InventoryStorageUI : MonoBehaviour 
{
    InventoryStorage inventoryStorage;
    Inventory playerInventory;

    InventorySlot[] storageSlots;
    InventorySlot[] playerSlots;

    GameObject UI;

    void Start()
    {
        playerInventory = Inventory.instance;
        UI = GameObject.Find("InventoryUI");
    }
}
