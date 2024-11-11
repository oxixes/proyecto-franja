using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData;
    public string dialogueId;
    private int notificationHandlerId = -1;
    PlayerInventoryManager playerInventory;

    void Start()
    {
        // Obtains a reference to the DialogueSystem and PlayerInventoryManager
        dialogueSystem = DialogueSystem.GetInstance();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerInventory = player.GetComponent<PlayerInventoryManager>();
        
        if (playerInventory == null)
        {
            Debug.LogError("Inventory not found!");
            return;
        }

        // Check if the item is already in inventory 
        CheckIfItemAlreadyInInventory();

        // Check if the DialogueSystem is null
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem not found!");
            return;
        }

        // Subscribes to the "CollectItemNotification" notification
        notificationHandlerId = dialogueSystem.HandleNotification("CollectItemNotification", HandleCollectItemNotification);
    }

    void Update()
    {
        // Check if the player is in range and presses the E key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCollectDialogue();
        }
    }

    // Checks if the item is already in the player's inventory, and destroys it if it is
    private void CheckIfItemAlreadyInInventory()
    {
        if (playerInventory != null && itemData != null)
        {
            if (playerInventory.playerInventory.HasItem(itemData))
            {
                Debug.Log($"Item '{itemData.itemName}' already in inventory. Destroying...");
                Destroy(gameObject); // Elimina el objeto de la escena
            }
        }
    }

    private void ShowCollectDialogue()
    {
        // Starts the dialogue with the specified dialogueId if there isn't any active dialogue
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            dialogueSystem.StartDialogue(dialogueId);
        }
    }

    // Item collection notification handler
    private void HandleCollectItemNotification(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationId == "CollectItemNotification")
        {
            CollectItem();
        }
    }

    // Collect the item and add it to the inventory
    public void CollectItem()
    {
        
        Debug.Log(playerInventory);
        Debug.Log(itemData);
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData);
            Debug.Log($"Item {itemData.itemName} added to the inventory.");
            Destroy(gameObject); // Destroy the object in the scene

        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager or itemData is null.");
        }
    }

    public void OnDestroy()
    {
        // Elimina la suscripci�n a la notificaci�n
        if (notificationHandlerId != -1)
            dialogueSystem.RemoveNotificationHandler(notificationHandlerId);
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
