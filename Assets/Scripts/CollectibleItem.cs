using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData; // Asigna el ItemData en el Inspector
    public string dialogueId; // El ID del di�logo que se usar� en el DialogueSystem
    private int notificationHandlerId; // ID de la subscripci�n a la notificaci�n

    void Start()
    {
        // Obt�n una referencia al DialogueSystem al inicio
        dialogueSystem = DialogueSystem.GetInstance();

        // Verifica si se ha encontrado el DialogueSystem
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem no se ha encontrado en la escena.");
            return;
        }

        // Suscr�bete a la notificaci�n "CollectItemNotification"
        notificationHandlerId = dialogueSystem.HandleNotification("CollectItemNotification", HandleCollectItemNotification);
    }

    void Update()
    {
        // Si el jugador est� en rango y presiona la tecla E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCollectDialogue();
        }
    }

    private void ShowCollectDialogue()
    {
        // Inicia el di�logo usando el DialogueSystem con el ID proporcionado
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            dialogueSystem.StartDialogue(dialogueId);
        }
    }

    // Funci�n que maneja la notificaci�n y recoge el �tem
    private void HandleCollectItemNotification(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationId == "CollectItemNotification")
        {
            CollectItem();
        }
    }

    // M�todo que se ejecutar� para recoger el �tem y guardarlo en el inventario
    public void CollectItem()
    {
        PlayerInventoryManager playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData);
            Debug.Log($"Item {itemData.itemName} a�adido al inventario.");
            Destroy(gameObject); // Destruye el objeto de la escena

            // Elimina la suscripci�n a la notificaci�n
            dialogueSystem.RemoveNotificationHandler(notificationHandlerId);
        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager o itemData es nulo.");
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
