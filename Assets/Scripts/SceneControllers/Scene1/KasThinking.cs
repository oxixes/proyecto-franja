using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KasThinking : MonoBehaviour
{
    public GameObject kas;

    private void Start()
    {
        if (SaveManager.GetInstance().Get<int>("Scene1KasDialogueFinished") == 1)
        {
            OnKasDialogueFinished(null, null, null);
            OnTriggerExit2D(null);
        }

        if (SaveManager.GetInstance().Get<int>("KasThinkingFinished") == 1)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DialogueSystem.GetInstance().HandleNotification("KasDialogueFinished", OnKasDialogueFinished);
        DialogueSystem.GetInstance().StartDialogue("NPCs/ThinkingAboutKas");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        gameObject.SetActive(false);
        SaveManager.GetInstance().Set("KasThinkingFinished", 1);
    }

    void OnKasDialogueFinished(string dialogueId, string notificationId, string notificationData)
    {
        kas.GetComponent<InteractableObject>().dialogueID = "NPCs/KasDialogueRepeat";
        SaveManager.GetInstance().Set("Scene1KasDialogueFinished", 1);
    }
}

