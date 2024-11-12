using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoProvider : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public InformationData infoData;
    public string dialogueId;
    public string alternativeDialogueId;

    // Allow tests to access the player inventory
    [HideInInspector] public PlayerInventoryManager playerInventory;

    void Start()
    {
        // Obtain a reference to the DialogueSystem and PlayerInventoryManager
        dialogueSystem = DialogueSystem.GetInstance();
        playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem not found.");
        }
    }

    void Update()
    {
        // If the player is in range and presses the E key, start the dialogue
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogueForInfo();
        }
    }

    private void StartDialogueForInfo()
    {

        // Starts the dialogue with the specified dialogueId if there isn't any active dialogue
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            if (playerInventory.HasInformation(infoData))
            {
                dialogueSystem.StartDialogue(alternativeDialogueId);
                Debug.Log("Info collected. Using alternative dialogue");
            }
            else 
            {
                dialogueSystem.StartDialogue(dialogueId);
                dialogueSystem.HandleNotification("InfoReceived", HandleInfoNotification);
            }
            
        }
    }

    private void HandleInfoNotification(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationId == "InfoReceived")
        {
            Debug.Log("Notification received!");
            AddInfoToInventory();
        }
    }

    public void AddInfoToInventory()
    {
        if (playerInventory != null && infoData != null)
        {
            playerInventory.CollectInformation(infoData); // Calls the CollectInformation function to add the information to the player's inventory
            Debug.Log($"Info '{infoData.infoName}' sent to PlayerInventoryManager.");
        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager or InformationData is null.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}

