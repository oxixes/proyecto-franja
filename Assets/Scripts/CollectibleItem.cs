using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData; // Asigna el ItemData en el Inspector
    public string dialogueId; // El ID del diálogo que se usará en el DialogueSystem
    private int notificationHandlerId = -1; // ID de la subscripción a la notificación
    PlayerInventoryManager playerInventory;

    void Start()
    {
        // Obtén una referencia al DialogueSystem al inicio
        dialogueSystem = DialogueSystem.GetInstance();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerInventory = player.GetComponent<PlayerInventoryManager>();
        
        if (playerInventory ==null)
            Debug.LogError("No existe el Inventario");

        //tengo que comprobar si existe el objeto en el inventario, si está, destruyo el objeto 
        CheckIfItemAlreadyInInventory();
        // Verifica si se ha encontrado el DialogueSystem
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem no se ha encontrado en la escena.");
            return;
        }

        // Suscríbete a la notificación "CollectItemNotification"
        notificationHandlerId = dialogueSystem.HandleNotification("CollectItemNotification", HandleCollectItemNotification);
    }

    void Update()
    {
        // Si el jugador está en rango y presiona la tecla E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCollectDialogue();
        }
    }

    //Revisa si el objeto está en el inventario. En caso de estarlo, lo destruye. 
    private void CheckIfItemAlreadyInInventory()
    {
        Debug.Log("he entrado");
        if (playerInventory != null && itemData != null)
        {
            if (playerInventory.playerInventory.HasItem(itemData))
            {
                Debug.Log($"El ítem '{itemData.itemName}' ya está en el inventario. Destruyendo el objeto.");
                Destroy(gameObject); // Elimina el objeto de la escena
            }
        }
    }

    private void ShowCollectDialogue()
    {
        // Inicia el diálogo usando el DialogueSystem con el ID proporcionado
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            dialogueSystem.StartDialogue(dialogueId);
        }
    }

    // Función que maneja la notificación y recoge el ítem
    private void HandleCollectItemNotification(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationId == "CollectItemNotification")
        {
            CollectItem();
        }
    }

    // Método que se ejecutará para recoger el ítem y guardarlo en el inventario
    public void CollectItem()
    {
        
        Debug.Log(playerInventory);
        Debug.Log(itemData);
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData);
            Debug.Log($"Item {itemData.itemName} añadido al inventario.");
            Destroy(gameObject); // Destruye el objeto de la escena

        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager o itemData es nulo.");
        }
    }

    public void OnDestroy()
    {
        // Elimina la suscripción a la notificación
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
