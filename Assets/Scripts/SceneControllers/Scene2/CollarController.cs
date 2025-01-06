using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollarController : MonoBehaviour
{
    public ItemData collarItem;
    public ItemData collarCleanItem;

    public GameObject collarTrigger;
    public List<GameObject> collarCleanTriggers;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject trigger in collarCleanTriggers)
        {
            trigger.SetActive(false);
        }

        if (SaveManager.GetInstance().Get<int>("Scene2CollarDialogueFinished") == 1)
        {
            OnCollarPickup(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene2CollarCleanDialogueFinished") == 1)
        {
            OnCollarClean(null, null, null);
        }

        DialogueSystem.GetInstance().HandleNotification("CollarFound", OnCollarPickup);
        DialogueSystem.GetInstance().HandleNotification("CollarCleaned", OnCollarClean);
    }

    void OnCollarPickup(string dialogueID, string notificationID, string notificationData)
    {
        SaveManager.GetInstance().Set("Scene2CollarDialogueFinished", 1);
        player.GetComponent<PlayerInventoryManager>().CollectItem(collarItem);
        collarTrigger.SetActive(false);
        foreach (GameObject trigger in collarCleanTriggers)
        {
            trigger.SetActive(true);
        }
    }

    void OnCollarClean(string dialogueID, string notificationID, string notificationData)
    {
        foreach (GameObject trigger in collarCleanTriggers)
        {
            trigger.SetActive(false);
        }

        player.GetComponent<PlayerInventoryManager>().CollectItem(collarCleanItem);
        player.GetComponent<PlayerInventoryManager>().RemoveItem(collarItem);

        SaveManager.GetInstance().Set("Scene2CollarCleanDialogueFinished", 1);
    }
}
