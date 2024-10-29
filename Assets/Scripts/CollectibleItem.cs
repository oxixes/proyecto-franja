using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public ItemData itemData; // Asigna el ItemData espec�fico en el Inspector

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
            // Muestra un di�logo preguntando si desea recoger el �tem
            dialogueSystem.StartDialogue($"�Quieres recoger {itemData.itemName}?");
        }
    }

    // M�todo que se llama si el jugador elige "S�" en el di�logo
    public void CollectItem()
    {
        PlayerInventoryManager playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (playerInventory != null && itemData != null)
        {
            playerInventory.CollectItem(itemData); // A�ade el �tem al inventario
            Destroy(gameObject); // Elimina el objeto de la escena
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Jugador en rango del �tem");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Jugador sali� del rango del �tem");
        }
    }
}
