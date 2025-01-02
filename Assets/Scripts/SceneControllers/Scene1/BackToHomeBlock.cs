using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToHomeBlock : MonoBehaviour
{
    public GameObject triggerUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && SaveManager.GetInstance().Get<int>("Scene1RobberyFinished") == 1)
        {
            triggerUp.SetActive(false);
            DialogueSystem.GetInstance().StartDialogue("NPCs/BackToHomeBlockDialogue");
        }
    }
}
