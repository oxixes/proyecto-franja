using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantGoLeft : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            DialogueSystem.GetInstance().StartDialogue("NPCs/KasCantGoLeft");
        }
    }
}
