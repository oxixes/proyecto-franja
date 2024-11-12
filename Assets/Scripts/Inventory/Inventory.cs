using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Inventory
{
    public List<ItemData> items = new List<ItemData>();
    public List<InformationData> information = new List<InformationData>();
    private static string filePath;

    // Load inventory from JSON
    private void LoadFromJson()
    {
        filePath = Application.persistentDataPath + "/inventory.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            items = new List<ItemData>();
            information = new List<InformationData>();
        }
    }

    // Save inventory to JSON
    private void SaveToJson()
    {
        filePath = Application.persistentDataPath + "/inventory.json";
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, json);
    }

    // Add a new item to the inventory without duplicating it in the list or JSON
    public void AddItem(ItemData item)
    {
        // Load inventory from JSON to check for duplicates
        LoadFromJson();

        // Check if the item already exists in the list or JSON
        if (!items.Exists(i => i.name == item.name))
        {
            items.Add(item); // Add to the list in memory
            SaveToJson();    // Save to JSON if it's new
            Debug.Log("Added item to inventory!");
        }
        else
        {
            Debug.Log("Player already had item in inventory. No change was made.");
        }
    }

    // Add a new information to the inventory without duplicating it in the list or JSON
    public void AddInformation(InformationData info)
    {
        LoadFromJson();

        // Check if the information already exists in the list or JSON
        if (!information.Exists(i => i.infoName == info.infoName))
        {
            information.Add(info); // Add to the list in memory
            SaveToJson();          // Check if it's new and save to JSON
            Debug.Log("Added information to inventory!");
        }
        else
        {
            Debug.Log("Player already had information in inventory. No change was made.");
        }
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public bool HasInformation(InformationData info)
    {
        return information.Contains(info);
    }

    public void SaveInventory()
    {
        SaveToJson();
    }

    public void LoadInventory()
    {
        LoadFromJson();
    }
}
