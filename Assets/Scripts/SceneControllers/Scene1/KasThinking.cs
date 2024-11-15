using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KasThinking : MonoBehaviour
{
    public GameObject kas;

    void OnTriggerEnter2D(Collider2D other)
    {
        DialogueSystem.GetInstance().HandleNotification("KasDialogueFinished", OnKasDialogueFinished);
        DialogueSystem.GetInstance().StartDialogue("NPCs/ThinkingAboutKas");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        gameObject.SetActive(false);
    }

    void OnKasDialogueFinished(string dialogueId, string notificationId, string notificationData)
    {
        kas.GetComponent<InteractableObject>().dialogueID = "NPCs/KasDialogueRepeat";
    }
}

