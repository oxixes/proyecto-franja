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

    // Añadir un nuevo objeto sin duplicar en lista ni JSON
    public void AddItem(ItemData item)
    {
        // Cargar el inventario actual desde JSON para evitar duplicados en archivo
        LoadFromJson();

        // Verificar si el objeto ya existe en la lista o en el JSON
        if (!items.Exists(i => i.name == item.name))
        {
            items.Add(item); // Añadir al inventario en memoria
            SaveToJson();    // Guardar en JSON si es nuevo
            Debug.Log("Objeto añadido y guardado en JSON de verdad.");
        }
        else
        {
            Debug.Log("El objeto ya existe en el JSON y en la lista, no se añadirá.");
        }
    }

    // Añadir nueva información sin duplicar en lista ni JSON
    public void AddInformation(InformationData info)
    {
        LoadFromJson();

        // Verificar si la información ya existe en la lista o en el JSON
        if (!information.Exists(i => i.infoName == info.infoName))
        {
            information.Add(info); // Añadir a la lista en memoria
            SaveToJson();          // Guardar en JSON si es nuevo
            Debug.Log("Información añadida y guardada en JSON y de verdad.");
        }
        else
        {
            Debug.Log("La información ya existe en el JSON y en la lista, no se añadirá.");
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
