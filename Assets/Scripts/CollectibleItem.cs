using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData; // Asigna el ItemData específico en el Inspector

    void Start()
    {
        dialogueSystem = DialogueSystem.GetInstance();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCollectDialogue();
        }
    }

    private void ShowCollectDialogue()
    {
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            // Muestra un diálogo preguntando si desea recoger el ítem
            dialogueSystem.StartDialogue($"¿Quieres recoger {itemData.itemName}?");
        }
    }

    // Método que se llama si el jugador elige "Sí" en el diálogo
    public void CollectItem()
    {
        PlayerInventoryManager playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData); // Añade el ítem al inventario
            Destroy(gameObject); // Elimina el objeto de la escena
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Jugador en rango del ítem");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Jugador salió del rango del ítem");
        }
    }
}
