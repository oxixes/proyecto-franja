using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KasThinking : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        DialogueSystem.GetInstance().StartDialogue("Scene1/ThinkingAboutKas");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        gameObject.SetActive(false);
    }
}

