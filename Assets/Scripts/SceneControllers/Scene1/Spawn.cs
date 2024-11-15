using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject trigger;

    void OnTriggerEnter2D(Collider2D other)
    {
        DialogueSystem.GetInstance().StartDialogue("NPCs/MomDialogue1");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        trigger.SetActive(false);
    }
 }

