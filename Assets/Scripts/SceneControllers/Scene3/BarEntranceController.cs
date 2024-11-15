using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarEntranceController : MonoBehaviour
{
    public GameObject barMan;

    // Start is called before the first frame update
    void Start()
    {
        DialogueSystem.GetInstance().HandleNotification("BarManDialogueFinished", HandleBarManDialogueFinishNotification);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player triggered bar entrance, running cutscene.");
            DialogueSystem.GetInstance().StartDialogue("NPCs/BarMan");
        }
    }

    void HandleBarManDialogueFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        barMan.SetActive(false);
        gameObject.SetActive(false);
    }
}
