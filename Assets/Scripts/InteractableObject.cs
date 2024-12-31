using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private bool inDialogue = false;
    private DialogueSystem dialogueSystem;
    public string dialogueID;

    // Allow tests to simulate key presses and player being in range
    [HideInInspector] public bool ePressed = false;
    [HideInInspector] public bool forcePlayerInRange = false;

    private GameObject eKeyHint;

    void Start() {
        dialogueSystem = DialogueSystem.GetInstance();

        // Find the E key hint in the children of this object
        if (transform.Find("EKeySprite") != null)
        {
            eKeyHint = transform.Find("EKeySprite").gameObject;
            eKeyHint.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        inDialogue = dialogueSystem.IsDialogueActive();
        if ((isPlayerInRange || forcePlayerInRange) && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || ePressed) && !inDialogue && !Minigame.isInMinigame && !PauseMenuController.isPaused)
        {
            Interact();
        }

        if (SaveManager.GetInstance().Get<int>("ShowContextualHints") == 1 && eKeyHint != null && (isPlayerInRange || forcePlayerInRange) && !inDialogue && !Minigame.isInMinigame) {
            eKeyHint.SetActive(true);
        } else if (eKeyHint != null) {
            eKeyHint.SetActive(false);
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
