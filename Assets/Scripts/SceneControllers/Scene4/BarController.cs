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

    public GameObject player;

    public InformationData rayoHasLostInfo;
    public InformationData pichaCortaInfo;
    public InformationData lePusoLosCuernosInfo;
    public ItemData copaItem;
    public ItemData flowersItem;
    public ItemData dirtyCollarItem;
    public ItemData cleanCollarItem;

    private InteractableObject barTableMan1IO;
    private InteractableObject barTableMan2IO;
    private InteractableObject bartenderIO;
    private InteractableObject jessiIO;
    private InteractableObject enamoradoIO;

    private PlayerInventoryManager inventory;

    // Start is called before the first frame update
    void Start()
    {
        barTableMan1IO = barTableMan1.GetComponent<InteractableObject>();
        barTableMan2IO = barTableMan2.GetComponent<InteractableObject>();
        bartenderIO = bartender.GetComponent<InteractableObject>();
        jessiIO = jessi.GetComponent<InteractableObject>();
        enamoradoIO = enamorado.GetComponent<InteractableObject>();
        inventory = player.GetComponent<PlayerInventoryManager>();

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

        if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") >= 1)
        {
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") == 1)
            {
                jessiIO.dialogueID = "Scene4/JessiPiquito";
            }
            else if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") == 2)
            {
                jessiIO.dialogueID = "Scene4/JessiRosas";
            }
            else if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") == 3)
            {
                jessiIO.dialogueID = "Scene4/JessiCopa";
            }
            else if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") == 4)
            {
                jessiIO.dialogueID = "Scene4/JessiCollar";
            }
        }

        if (SaveManager.GetInstance().Get<int>("Scene4JessiEnamoradoDialogueFinished") == 1)
        {
            jessiIO.dialogueID = "Scene4/JessiRepeat";
        }

        DialogueSystem.GetInstance().HandleNotification("BarTableDialogueFinished", HandleTableConversationFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("BartenderDialogueFinished", HandleBartenderFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("JessiDialogueFinished", HandleJessiFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDialogueFinished", HandleEnamoradoDialogueFinish);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDecision", HandleEnamoradoDecision);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoCollarNotification", HandleEnamoradoCollarDecision);
        DialogueSystem.GetInstance().HandleNotification("BartenderCopaDialogueFinished", HandleCopaDialogueFinished);
        DialogueSystem.GetInstance().HandleNotification("JessiEnamoradoDialogueFinished", HandleJessiEnamoradoFinish);
    }

    void HandleTableConversationFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        barTableMan1IO.dialogueID = "NPCs/BarTableRepeat";
        barTableMan2IO.dialogueID = "NPCs/BarTableRepeat";

        SaveManager.GetInstance().Set("Scene4BarTableDialogueFinished", 1);
        player.GetComponent<PlayerInventoryManager>().CollectInformation(rayoHasLostInfo);
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
        bartenderIO.dialogueID = "Scene4/BartenderCopa";
        SaveManager.GetInstance().Set("Scene4EnamoradoDialogueFinished", 1);
    }

    void HandleEnamoradoDecision(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationData == "1")
        {
            StartCoroutine(StartEnamoradoPiquitoDialogue());
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            jessiIO.dialogueID = "Scene4/JessiPiquito";
            SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 1);
        }
        else if (notificationData == "2" && !inventory.HasItem(flowersItem))
        {
            StartCoroutine(StartDidntBringFlowersDialogue());
        }
        else if (notificationData == "2" && inventory.HasItem(flowersItem))
        {
            StartCoroutine(StartEnamoradoFlowersDialogue());
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            inventory.RemoveItem(flowersItem);
            jessiIO.dialogueID = "Scene4/JessiRosas";
            SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 2);
        }
        else if (notificationData == "3" && !inventory.HasItem(copaItem))
        {
            StartCoroutine(StartDidntBringCopaDialogue());
        }
        else if (notificationData == "3" && inventory.HasItem(copaItem))
        {
            StartCoroutine(StartEnamoradoCopaDialogue());
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            jessiIO.dialogueID = "Scene4/JessiCopa";
            inventory.RemoveItem(copaItem);
            SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 3);
        }
        else if (notificationData == "4" && (inventory.HasItem(dirtyCollarItem) || inventory.HasItem(cleanCollarItem)))
        {
            StartCoroutine(StartDecision4Dialogue());
        }
    }

    void HandleEnamoradoCollarDecision(string dialogueId, string notificationId, string notificationData)
    {
        if (inventory.HasItem(dirtyCollarItem))
        {
            StartCoroutine(StartDirtyCollarDialogue());
        }
        else if (inventory.HasItem(cleanCollarItem))
        {
            StartCoroutine(StartCleanCollarDialogue());
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            inventory.RemoveItem(cleanCollarItem);
            jessiIO.dialogueID = "Scene4/JessiCollar";
            SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 4);
        }
    }

    void HandleCopaDialogueFinished(string dialogueId, string notificationId, string notificationData)
    {
        bartenderIO.dialogueID = "Scene4/BartenderRepeat";
        inventory.CollectItem(copaItem);
    }

    void HandleJessiEnamoradoFinish(string dialogueId, string notificationId, string notificationData)
    {
        jessiIO.dialogueID = "Scene4/JessiRepeat";

        if (notificationData == "3")
        {
            inventory.CollectInformation(lePusoLosCuernosInfo);
        } else if (notificationData == "4")
        {
            inventory.CollectInformation(pichaCortaInfo);
            inventory.CollectInformation(lePusoLosCuernosInfo);
        }

        SaveManager.GetInstance().Set("Scene4JessiEnamoradoDialogueFinished", 1);
    }

    private IEnumerator StartEnamoradoPiquitoDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoPiquito");
        yield return null;
    }

    private IEnumerator StartDidntBringCopaDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoDidntBringCopa");
        yield return null;
    }

    private IEnumerator StartEnamoradoCopaDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoCopa");
        yield return null;
    }

    private IEnumerator StartDidntBringFlowersDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoDidntBringRosas");
        yield return null;
    }

    private IEnumerator StartEnamoradoFlowersDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoRosas");
        yield return null;
    }

    private IEnumerator StartDecision4Dialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoDecision4Collar");
        yield return null;
    }

    private IEnumerator StartDirtyCollarDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoDirtyCollar");
        yield return null;
    }

    private IEnumerator StartCleanCollarDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/EnamoradoCollar");
        yield return null;
    }
}
