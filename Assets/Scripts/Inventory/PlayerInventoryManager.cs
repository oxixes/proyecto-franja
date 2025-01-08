using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    [HideInInspector] public Inventory playerInventory;

    public GameObject pauseMenuObjectsText;
    public GameObject pauseMenuInfoText;

    private TextMeshProUGUI objectsText;
    private TextMeshProUGUI infoText;

    public void Start()
    {
        playerInventory = new Inventory();
        playerInventory.LoadInventory();

        if (pauseMenuObjectsText == null || pauseMenuInfoText == null)
        {
            return; // Tests
        }

        objectsText = pauseMenuObjectsText.GetComponent<TextMeshProUGUI>();
        infoText = pauseMenuInfoText.GetComponent<TextMeshProUGUI>();

        UpdateObjectsText();
        UpdateInfoText();
    }

    public void CollectItem(ItemData item)
    {
        playerInventory.AddItem(item);
        UpdateObjectsText();
    }

    public void RemoveItem(ItemData item)
    {
        playerInventory.RemoveItem(item);
        UpdateObjectsText();
    }

    public void CollectInformation(InformationData info)
    {
        playerInventory.AddInformation(info);
        UpdateInfoText();
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

    public void Delete()
    {
        playerInventory.DeleteInventory();
        UpdateObjectsText();
        UpdateInfoText();
    }

    private void UpdateObjectsText()
    {
        if (objectsText == null)
        {
            return; // Tests
        }

        string text = "";
        int i = 1;
        foreach (ItemData item in playerInventory.GetItems())
        {
            text += i.ToString() + ". " + item.itemName + "\n";
            i++;
        }

        if (playerInventory.GetItems().Count == 0)
        {
            text = "Todavía no has recogido ningún objeto.";
        }

        objectsText.text = text;
    }

    private void UpdateInfoText()
    {
        if (infoText == null)
        {
            return; // Tests
        }

        string text = "";
        int i = 1;
        foreach (InformationData info in playerInventory.GetInformation())
        {
            text += i.ToString() + ". " + info.infoName + "\n";
            i++;
        }

        if (playerInventory.GetInformation().Count == 0)
        {
            text = "Todavía no has recogido ninguna información.";
        }

        infoText.text = text;
    }
}
