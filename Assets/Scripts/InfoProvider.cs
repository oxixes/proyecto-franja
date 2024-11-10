using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoProvider : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public InformationData infoData; // Asigna el InformationData en el Inspector
    public string dialogueId; // ID del diálogo, variable pública para asignar según el personaje
    PlayerInventoryManager playerInventory;

    void Start()
    {
        // Obtén una referencia al sistema de diálogo
        dialogueSystem = DialogueSystem.GetInstance();
        playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem no se ha encontrado en la escena.");
        }
    }

    void Update()
    {
        // Si el jugador está en rango y presiona la tecla E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogueForInfo();
        }
    }

    private void StartDialogueForInfo()
    {
        
        // Inicia el diálogo si no hay otro diálogo activo
        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            if (playerInventory.HasInformation(infoData))
            {
                //se puede cambiar por variable pública para tener diferentes diálogos de info repetida
                dialogueSystem.StartDialogue("Info_Already_Adquired");
                Debug.Log("La información ya está en el inventario. Usando el diálogo alternativo.");
            }
            else {
                dialogueSystem.StartDialogue(dialogueId);
                dialogueSystem.HandleNotification("InfoReceived", HandleInfoNotification);
            }
            
        }
    }

    private void HandleInfoNotification(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationId == "InfoReceived")
        {
            Debug.Log("Ha llegado la noti de info recibida");
            AddInfoToInventory();
        }
    }

    private void AddInfoToInventory()
    {
        //PlayerInventoryManager playerInventory = FindObjectOfType<PlayerInventoryManager>();
        if (playerInventory != null && infoData != null)
        {
            playerInventory.CollectInformation(infoData); // Llama a la nueva función para añadir información
            Debug.Log($"Información '{infoData.infoName}' ha sido mandada a PlayerInventoryManager.");
        }
        else
        {
            Debug.LogWarning("PlayerInventoryManager o InformationData es nulo.");
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

