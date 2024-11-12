using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public Inventory playerInventory;

    public void Start()
    {
        playerInventory = new Inventory();
        playerInventory.LoadInventory();
    }
    
    public void CollectItem(ItemData item)
    {
        playerInventory.AddItem(item);
    }
    
    public void CollectInformation(InformationData info)
    {
        playerInventory.AddInformation(info);
    }

    public void SaveOnCheckpoint()
    {
        playerInventory.SaveInventory();
        Debug.Log("Inventory saved on checkpoint!");
    }

    public bool HasInformation(InformationData info)
    {
        if (playerInventory == null)
        {
            throw new System.Exception("PlayerInventory is null");
        }
        return playerInventory.HasInformation(info);
    }

    public bool HasItem(ItemData item)
    {
        if (playerInventory == null)
        {
            throw new System.Exception("PlayerInventory is null");
        }
        return playerInventory.HasItem(item);
    }

    public void OnApplicationQuit()
    {
        playerInventory.SaveInventory();
    }
}
