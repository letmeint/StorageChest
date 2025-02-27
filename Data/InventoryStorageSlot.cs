﻿using HarmonyLib;
using StorageChest;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryStorageSlot : MonoBehaviour, IHoverable
{
    public enum SlotType
    {
        Player,
        Storage
    }

    public SlotType slotType;

    public Image icon;

    public Image background;

    public Image rarityShine;

    Vector3 infoBoxOffset;

    Sprite hasItemSprite;

    Sprite emptySprite;

    public Item item;

    public void Init(Action<InventoryStorageSlot> onButtonClick, InventorySlotShop item, SlotType slotType)
    {
        this.slotType = slotType;
        CreateChildren(onButtonClick, item);
    }

    private void CreateChildren(Action<InventoryStorageSlot> onButtonClick, InventorySlotShop oldSlot)
    {
        var button = this.transform.GetChild(0);
        for (int i = 0; i < button.childCount; i++)
        {
            if (button.GetChild(i).name == "Icon")
            {
                //Plugin.Logger.LogInfo("got the icon");
                icon = button.GetChild(i).GetComponent<Image>();
                //Plugin.Logger.LogInfo("the icon is " + icon == null);
            }
            else if (button.GetChild(i).name == "Rarity Shine")
            {
                rarityShine = button.GetChild(i).GetComponent<Image>();
            }
        }

        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => LunasShopModified.OnTransferItem(this));

        background = button.GetComponent<Image>();

        hasItemSprite = (Sprite)AccessTools.Field(typeof(InventorySlotShop), "hasItemSprite").GetValue(oldSlot);
        emptySprite = (Sprite)AccessTools.Field(typeof(InventorySlotShop), "emptySprite").GetValue(oldSlot);
    }

    public void CreateItem(Item item)
    {
        this.item = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        background.sprite = hasItemSprite;
        if (item.rarity != 0)
        {
            rarityShine.enabled = true;
            rarityShine.color = MasterDictionary.instance.GetColor(item.rarity);
        }
        else
        {
            rarityShine.enabled = false;
        }

    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        background.sprite = emptySprite;
        rarityShine.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            float num = (float)Screen.height / 1080f;
            ItemInfoMasterUI.instance.DisplayInformation(item, base.transform.position + infoBoxOffset * num);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemInfoMasterUI.instance.Close();
    }
}
