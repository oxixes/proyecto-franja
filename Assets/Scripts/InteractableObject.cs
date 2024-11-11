using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private DialogueSystem dialogueSystem;
    public string dialogueID;

    void Start() { 
        dialogueSystem = DialogueSystem.GetInstance();
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {

        if (dialogueSystem != null && !dialogueSystem.IsDialogueActive())
        {
            dialogueSystem.StartDialogue(dialogueID);
        }
        else
        {
            Debug.LogWarning("Couldn't start dialogue or there is already an active dialogue");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Entered an object's interaction area");
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Left an object's interaction area");
            isPlayerInRange = false;
        }
    }
}
