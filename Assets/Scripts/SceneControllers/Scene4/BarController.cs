using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public GameObject barTableMan1;
    public GameObject barTableMan2;
    public GameObject bartender;
    public GameObject jessi;
    public GameObject enamorado;

    private InteractableObject barTableMan1IO;
    private InteractableObject barTableMan2IO;
    private InteractableObject bartenderIO;
    private InteractableObject jessiIO;
    private InteractableObject enamoradoIO;

    // Start is called before the first frame update
    void Start()
    {
        barTableMan1IO = barTableMan1.GetComponent<InteractableObject>();
        barTableMan2IO = barTableMan2.GetComponent<InteractableObject>();
        bartenderIO = bartender.GetComponent<InteractableObject>();
        jessiIO = jessi.GetComponent<InteractableObject>();
        enamoradoIO = enamorado.GetComponent<InteractableObject>();

        if (SaveManager.GetInstance().Get<int>("Scene4BarTableDialogueFinished") == 1)
        {
            HandleTableConversationFinishNotification(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene4BartenderDialogueFinished") == 1)
        {
            HandleBartenderFinishNotification(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene4JessiDialogueFinished") == 1)
        {
            HandleJessiFinishNotification(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDialogueFinished") == 1)
        {
            HandleEnamoradoDialogueFinish(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") == 1)
        {
            HandleEnamoradoDecision(null, null, SaveManager.GetInstance().Get<string>("Scene4EnamoradoDecision"));
        }

        if (SaveManager.GetInstance().Get<int>("Scene4BartenderCopaDialogueFinished") == 1)
        {
            HandleBartenderCopaFinish(null, null, null);
        }

        DialogueSystem.GetInstance().HandleNotification("BarTableDialogueFinished", HandleTableConversationFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("BartenderDialogueFinished", HandleBartenderFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("JessiDialogueFinished", HandleJessiFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDialogueFinished", HandleEnamoradoDialogueFinish);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDecision", HandleEnamoradoDecision);
        DialogueSystem.GetInstance().HandleNotification("BartenderCopaDialogueFinished", HandleBartenderCopaFinish);
    }

    void HandleTableConversationFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        barTableMan1IO.dialogueID = "NPCs/BarTableRepeat";
        barTableMan2IO.dialogueID = "NPCs/BarTableRepeat";

        SaveManager.GetInstance().Set("Scene4BarTableDialogueFinished", 1);
    }

    void HandleBartenderFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        bartenderIO.dialogueID = "Scene4/BartenderRepeat";
        SaveManager.GetInstance().Set("Scene4BartenderDialogueFinished", 1);
    }

    void HandleJessiFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        jessiIO.dialogueID = "Scene4/JessiRepeat";
        SaveManager.GetInstance().Set("Scene4JessiDialogueFinished", 1);
    }

    void HandleEnamoradoDialogueFinish(string dialogueId, string notificationId, string notificationData)
    {
        enamoradoIO.dialogueID = "Scene4/EnamoradoDecision1";
        SaveManager.GetInstance().Set("Scene4EnamoradoDialogueFinished", 1);
    }

    void HandleEnamoradoDecision(string dialogueId, string notificationId, string notificationData)
    {
        enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
        SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 1);
        SaveManager.GetInstance().Set("Scene4EnamoradoDecision", notificationData);

        if (notificationData == "3")
        {
            enamoradoIO.dialogueID = "Scene4/EnamoradoDidntBringCopa";
            bartenderIO.dialogueID = "Scene4/BartenderCopa";
        }
    }

    void HandleBartenderCopaFinish(string dialogueId, string notificationId, string notificationData)
    {
        bartenderIO.dialogueID = "Scene4/BartenderRepeat";
        enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
        SaveManager.GetInstance().Set("Scene4BartenderCopaDialogueFinished", 1);
    }
}
