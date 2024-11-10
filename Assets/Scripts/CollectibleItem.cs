using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData; // Asigna el ItemData en el Inspector
    public string dialogueId; // El ID del di�logo que se usar� en el DialogueSystem
    private int notificationHandlerId = -1; // ID de la subscripci�n a la notificaci�n
    PlayerInventoryManager playerInventory;

    void Start()
    {
        // Obt�n una referencia al DialogueSystem al inicio
        dialogueSystem = DialogueSystem.GetInstance();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerInventory = player.GetComponent<PlayerInventoryManager>();
        
        if (playerInventory ==null)
            Debug.LogError("No existe el Inventario");

        //tengo que comprobar si existe el objeto en el inventario, si est�, destruyo el objeto 
        CheckIfItemAlreadyInInventory();
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

    //Revisa si el objeto est� en el inventario. En caso de estarlo, lo destruye. 
    private void CheckIfItemAlreadyInInventory()
    {
        Debug.Log("he entrado");
        if (playerInventory != null && itemData != null)
        {
            if (playerInventory.playerInventory.HasItem(itemData))
            {
                Debug.Log($"El �tem '{itemData.itemName}' ya est� en el inventario. Destruyendo el objeto.");
                Destroy(gameObject); // Elimina el objeto de la escena
            }
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
        
        Debug.Log(playerInventory);
        Debug.Log(itemData);
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData);
            Debug.Log($"Item {itemData.itemName} a�adido al inventario.");
            Destroy(gameObject); // Destruye el objeto de la escena

        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager o itemData es nulo.");
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
