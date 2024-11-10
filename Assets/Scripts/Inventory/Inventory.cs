using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Inventory
{
    public List<ItemData> items = new List<ItemData>();
    public List<InformationData> information = new List<InformationData>();

    private static string filePath = Application.persistentDataPath + "/inventory.json";

    // Cargar inventario desde JSON si existe
    private void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }

    // Guardar inventario en JSON
    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, json);
    }

    // A�adir un nuevo objeto sin duplicar en lista ni JSON
    public void AddItem(ItemData item)
    {
        // Cargar el inventario actual desde JSON para evitar duplicados en archivo
        LoadFromJson();

        // Verificar si el objeto ya existe en la lista o en el JSON
        if (!items.Exists(i => i.name == item.name))
        {
            items.Add(item); // A�adir al inventario en memoria
            SaveToJson();    // Guardar en JSON si es nuevo
            Debug.Log("Objeto a�adido y guardado en JSON de verdad.");
        }
        else
        {
            Debug.Log("El objeto ya existe en el JSON y en la lista, no se a�adir�.");
        }
    }

    // A�adir nueva informaci�n sin duplicar en lista ni JSON
    public void AddInformation(InformationData info)
    {
        LoadFromJson();

        // Verificar si la informaci�n ya existe en la lista o en el JSON
        if (!information.Exists(i => i.infoName == info.infoName))
        {
            information.Add(info); // A�adir a la lista en memoria
            SaveToJson();          // Guardar en JSON si es nuevo
            Debug.Log("Informaci�n a�adida y guardada en JSON y de verdad.");
        }
        else
        {
            Debug.Log("La informaci�n ya existe en el JSON y en la lista, no se a�adir�.");
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
