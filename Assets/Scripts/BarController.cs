using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public GameObject barTableMan1;
    public GameObject barTableMan2;

    private InteractableObject barTableMan1IO;
    private InteractableObject barTableMan2IO;

    // Start is called before the first frame update
    void Start()
    {
        barTableMan1IO = barTableMan1.GetComponent<InteractableObject>();
        barTableMan2IO = barTableMan2.GetComponent<InteractableObject>();

        DialogueSystem.GetInstance().HandleNotification("BarTableDialogueFinished", HandleTableConversationFinishNotification);
    }

    void HandleTableConversationFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        barTableMan1IO.dialogueID = "NPCs/BarTableRepeat";
        barTableMan2IO.dialogueID = "NPCs/BarTableRepeat";
    }
}
