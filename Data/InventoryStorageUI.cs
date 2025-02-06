using StorageChest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
