using Newtonsoft.Json.Linq;
using StorageChest.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace StorageChest.Data
{
    public class InventoryStorage : MonoBehaviour
    {
        public List<Item> items = new List<Item>();
        public int space = 40;

        public event Action OnItemChanged;

        public static InventoryStorage instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public bool AddItem(Item item)
        {
            if (items.Count >= space)
            {
                return false;
            }
            items.Add(item);
            if (this.OnItemChanged != null)
            {
                this.OnItemChanged.Invoke();
            }
            return true;
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
            if (this.OnItemChanged != null)
            {
                this.OnItemChanged.Invoke();
            }
        }

        public void LoadData()
        {
            Plugin.Logger.LogInfo("Loading InventoryStorage data.");
            
            JObject data1 = IOHelper.ReadJson(Globals.defaultStorageSavePath + "\\saves.json");

            Plugin.Logger.LogWarning(data1.ToString()); 
            if (data1 is JObject stateObject)
            {
                items = new List<Item>();
                IDictionary<string, JToken> stateDict = stateObject;
                for (int i = 0; i < space; i++)
                {
                    if (stateDict.ContainsKey(i.ToString()) && stateDict[i.ToString()] is JObject itemState)
                    {
                        IDictionary<string, JToken> itemStateDict = itemState;
                        
                        Plugin.Logger.LogInfo("Searching in item dict");

                        if (itemStateDict["item"] != null && MasterDictionary.instance.itemDictionary.ContainsKey(itemStateDict["item"].ToString()))
                        {
                            Plugin.Logger.LogInfo("item found");

                            Item invItem = MasterDictionary.instance.itemDictionary[itemStateDict["item"].ToString()];
                            items.Add(invItem);
                            Plugin.Logger.LogInfo("item added");

                        }
                    }
                }
            }
            LunasShopModified.UpdateUI();
        }

        public void SaveData()
        {
            if (items?.Count == 0) return;

            Plugin.Logger.LogInfo("Saving InventoryStorage data.");

            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            for (int i = 0; i < space; i++)
            {
                JObject itemState = new JObject();
                IDictionary<string, JToken> itemStateDict = itemState;
                if (items.Count > i && items[i] != null)
                {
                    Plugin.Logger.LogInfo(items.Count + " " + i);
                    itemState["item"] = JToken.FromObject(items[i].name);
                }
               
                stateDict[i.ToString()] = itemState;
                IOHelper.WriteJson(Globals.defaultStorageSavePath + "\\saves.json", (JObject)stateDict);
            }
        }
    }
}
