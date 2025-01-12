using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public GameObject barTableMan1;
    public GameObject barTableMan2;
    public GameObject bartender;
    public GameObject jessi;
    public GameObject enamorado;
    public GameObject canvas;
    public GameObject minigamePrefab;
    public AudioController audioController;
    public AudioClip minigameMusic;
    public AudioClip barMusic;
    public Vector2 enamoradoPos = new Vector2(-2.01f, 1.85f);
    public Vector2 enamoradoNextToJessiPos = new Vector2(7.36f, -1.07f);
    public GameObject fadeOutPanel;

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

        if (SaveManager.GetInstance().Get<int>("Scene4JessiEnamoradoDialogueFinished") == 1)
        {
            jessiIO.dialogueID = "Scene4/JessiRepeat";
        }

        if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDecisionChosen") >= 1)
        {
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            jessiIO.dialogueID = "Scene4/JessiRepeat";
        }

        DialogueSystem.GetInstance().HandleNotification("BarTableDialogueFinished", HandleTableConversationFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("BeerPongMinigame", HandleMinigameStartNotification);
        DialogueSystem.GetInstance().HandleNotification("BartenderDialogueFinished", HandleBartenderFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("JessiDialogueFinished", HandleJessiFinishNotification);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDialogueFinished", HandleEnamoradoDialogueFinish);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoDecision", HandleEnamoradoDecision);
        DialogueSystem.GetInstance().HandleNotification("EnamoradoCollarNotification", HandleEnamoradoCollarDecision);
        DialogueSystem.GetInstance().HandleNotification("BartenderCopaDialogueFinished", HandleCopaDialogueFinished);
        DialogueSystem.GetInstance().HandleNotification("JessiEnamoradoDialogueFinished", HandleJessiEnamoradoFinish);
        DialogueSystem.GetInstance().HandleNotification("CutseneFinished", HandleOnCutsceneFinished);
    }

    void HandleTableConversationFinishNotification(string dialogueId, string notificationId, string notificationData)
    {
        barTableMan1IO.dialogueID = "NPCs/BarTableRepeat";
        barTableMan2IO.dialogueID = "NPCs/BarTableRepeat";

        SaveManager.GetInstance().Set("Scene4BarTableDialogueFinished", 1);
        player.GetComponent<PlayerInventoryManager>().CollectInformation(rayoHasLostInfo);

        if (SaveManager.GetInstance().Get<int>("Scene4EnamoradoDialogueFinished") == 1)
        {
            string dialogueIdToSet = SaveManager.GetInstance().Get<string>("Scene4BarTableDialogueEndResult");
            if (string.IsNullOrEmpty(dialogueIdToSet))
            {
                dialogueIdToSet = "Scene4/BarTableMinijuego";
            }

            barTableMan1IO.dialogueID = dialogueIdToSet;
            barTableMan2IO.dialogueID = dialogueIdToSet;
            if (dialogueId != null) StartCoroutine(StartMinijuegoDialogue());
        }
    }

    void HandleMinigameStartNotification(string dialogueId, string notificationId, string notificationData)
    {
        audioController.ForcePlayWithTransition(minigameMusic, true);

        // Instantiate minigame in canvas
        GameObject minigame = Instantiate(minigamePrefab, canvas.transform);
        minigame.GetComponent<BeerPongGame>().finishEvent.AddListener(HandleMinigameFinish);
    }

    void HandleMinigameFinish(bool won, bool skipped)
    {
        audioController.ForcePlayWithTransition(barMusic, true);

        if (skipped) {
            barTableMan1IO.dialogueID = "Scene4/BarTablePostMiniRepeat";
            barTableMan2IO.dialogueID = "Scene4/BarTablePostMiniRepeat";
            inventory.CollectItem(copaItem);
            SaveManager.GetInstance().Set("Scene4BarTableDialogueEndResult", "Scene4/BarTablePostMiniRepeat");
            DialogueSystem.GetInstance().StartDialogue("Scene4/BarTableMiniSkipped");
        } else {
            if (won) {
                barTableMan1IO.dialogueID = "Scene4/BarTablePostMiniRepeat";
                barTableMan2IO.dialogueID = "Scene4/BarTablePostMiniRepeat";
                inventory.CollectItem(copaItem);
                SaveManager.GetInstance().Set("Scene4BarTableDialogueEndResult", "Scene4/BarTablePostMiniRepeat");
                DialogueSystem.GetInstance().StartDialogue("Scene4/BarTablePostMini");
            } else {
                barTableMan1IO.dialogueID = "Scene4/BarTablePostLostMini";
                barTableMan2IO.dialogueID = "Scene4/BarTablePostLostMini";
                SaveManager.GetInstance().Set("Scene4BarTableDialogueEndResult", "Scene4/BarTablePostLostMini");
                DialogueSystem.GetInstance().StartDialogue("Scene4/BarTablePostLostMini");
            }
        }

        SaveManager.GetInstance().Set("Scene4MinigameFinished", 1);
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
        if (barTableMan1IO.dialogueID == "NPCs/BarTableRepeat")
        {
            barTableMan1IO.dialogueID = "Scene4/BarTableMinijuego";
            barTableMan2IO.dialogueID = "Scene4/BarTableMinijuego";
        }
        SaveManager.GetInstance().Set("Scene4EnamoradoDialogueFinished", 1);
    }

    void HandleEnamoradoDecision(string dialogueId, string notificationId, string notificationData)
    {
        if (notificationData == "1")
        {
            StartCoroutine(StartEnamoradoPiquitoDialogue());
            enamoradoIO.dialogueID = "Scene4/EnamoradoFinish";
            jessiIO.dialogueID = "Scene4/JessiRepeat";
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
            jessiIO.dialogueID = "Scene4/JessiRepeat";
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
            jessiIO.dialogueID = "Scene4/JessiRepeat";
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
            jessiIO.dialogueID = "Scene4/JessiRepeat";
            SaveManager.GetInstance().Set("Scene4EnamoradoDecisionChosen", 4);
        }
    }

    void HandleCopaDialogueFinished(string dialogueId, string notificationId, string notificationData)
    {
        bartenderIO.dialogueID = "Scene4/BartenderRepeat";
    }

    void HandleJessiEnamoradoFinish(string dialogueId, string notificationId, string notificationData)
    {
        jessiIO.dialogueID = "Scene4/JessiRepeat";

        if (notificationData == "3")
        {
            inventory.CollectInformation(lePusoLosCuernosInfo);
            inventory.RemoveItem(copaItem);
        } else if (notificationData == "4")
        {
            inventory.CollectInformation(pichaCortaInfo);
            inventory.CollectInformation(lePusoLosCuernosInfo);
            inventory.RemoveItem(cleanCollarItem);
        }

        SaveManager.GetInstance().Set("Scene4JessiEnamoradoDialogueFinished", 1);
    }

    private IEnumerator StartEnamoradoPiquitoDialogue()
    {
        yield return new WaitForSeconds(.1f);
        StartCoroutine(FadeOutCutscene("Scene4/CutscenePiquito"));
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
        StartCoroutine(FadeOutCutscene("Scene4/CutsceneCopa"));
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
        StartCoroutine(FadeOutCutscene("Scene4/CutsceneFlores"));
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
        StartCoroutine(FadeOutCutscene("Scene4/CutsceneCollar"));
        yield return null;
    }

    private IEnumerator StartMinijuegoDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Scene4/BarTableMinijuego");
        yield return null;
    }

    private void HandleOnCutsceneFinished(string dialogueID, string notificationID, string notificationData)
    {
        StartCoroutine(OnCutseneFinished());
    }

    private IEnumerator FadeOutCutscene(string nextDialogueID)
    {
        Minigame.isInMinigame = true;
        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        fadeOutPanel.SetActive(true);

        // Animate fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        // TP Enamorado next to Jessi
        enamorado.transform.localPosition = enamoradoNextToJessiPos;

        // Animate fade out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }

        fadeOutPanel.SetActive(false);
        Minigame.isInMinigame = false;

        DialogueSystem.GetInstance().StartDialogue(nextDialogueID);
    }

    private IEnumerator OnCutseneFinished()
    {
        Minigame.isInMinigame = true;
        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        fadeOutPanel.SetActive(true);

        // Animate fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        // TP Enamorado back to original position
        enamorado.transform.localPosition = enamoradoPos;

        // Animate fade out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }

        fadeOutPanel.SetActive(false);
        Minigame.isInMinigame = false;
    }
}
