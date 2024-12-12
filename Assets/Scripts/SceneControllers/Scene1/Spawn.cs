using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject trigger;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (SaveManager.GetInstance().Get<int>("Scene1MomDialogue1Finished") == 1)
        {
            OnTriggerExit2D(null);
            return;
        }
        DialogueSystem.GetInstance().StartDialogue("NPCs/MomDialogue1");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        SaveManager.GetInstance().Set("Scene1MomDialogue1Finished", 1);
        trigger.SetActive(false);
    }
 }

