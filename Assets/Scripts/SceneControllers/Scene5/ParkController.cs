using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ParkController : MonoBehaviour
{
    public GameObject chavala;
    public GameObject chaval1;
    public GameObject chaval2;
    public GameObject mantero;
    public Sprite manteroLookingRight;
    public GameObject quiosquero;
    public GameObject canvas;
    public GameObject juegoPulsoPrefab;
    public AudioController audioController;
    public AudioClip minigameMusic;
    public AudioClip overworldMusic;

    public GameObject player;
    public GameObject flowers;
    public ItemData flowersItem;
    public ItemData comicItem;
    public ItemData capItem;
    public InformationData quiqueTraidorInfo;

    public Vector2 targetPoint;
    public float speed = 0.5f;
    private bool runaway = false;

    private InteractableObject chavalaIO;
    private InteractableObject chaval1IO;
    private InteractableObject chaval2IO;
    private InteractableObject manteroIO;
    private InteractableObject quiosqueroIO;
    // Start is called before the first frame update
    void Start()
    {
        chavalaIO = chavala.GetComponent<InteractableObject>();
        chaval1IO = chaval1.GetComponent<InteractableObject>();
        chaval2IO = chaval2.GetComponent<InteractableObject>();
        manteroIO = mantero.GetComponent<InteractableObject>();
        quiosqueroIO = quiosquero.GetComponent<InteractableObject>();

        DialogueSystem.GetInstance().HandleNotification("ChavalaDialogueAfterPulsoFinished", HandleChavalaDialogueNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroDialogueFinished", HandleManteroDialogueNotification);
        DialogueSystem.GetInstance().HandleNotification("CapBought", HandleCapNotification);
        DialogueSystem.GetInstance().HandleNotification("ComicBought", HandleComicNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroAngry", HandleManteroAngryNotification);
        DialogueSystem.GetInstance().HandleNotification("ChavalaAngry", HandleChavalaAngryNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroLeaves", HandleManteroLeavesNotification);
        DialogueSystem.GetInstance().HandleNotification("ChavalaHelped", HandleChavalaHelpedNotification);
        DialogueSystem.GetInstance().HandleNotification("FlowersFound", HandleFlowersNotification);
        DialogueSystem.GetInstance().HandleNotification("JuegoPulso", HandleJuegoPulsoNotification);

        if (SaveManager.GetInstance().Get<int>("Scene5ChavalaDialogueFinished") == 1)
        {
            HandleChavalaDialogueNotification(null, null, "EscenaParque/NPCs/ChavalaDialogueRepeat");
        }
        if (SaveManager.GetInstance().Get<int>("Scene5ManteroDialogueFinished") == 1)
        {
            HandleManteroDialogueNotification(null, null, "EscenaParque/NPCs/ManteroDialogueRepeat");
        }
        if (SaveManager.GetInstance().Get<int>("CapBought") == 1)
        {
            HandleCapNotification(null, null, "hola");
        }
        if (SaveManager.GetInstance().Get<int>("ComicBought") == 1)
        {
            HandleComicNotification(null, null, "hola");
        }
        if (SaveManager.GetInstance().Get<int>("Scene5ManteroAngry") == 1)
        {
            HandleManteroAngryNotification(null, null, "EscenaParque/NPCs/ManteroIgnoresJuan");
        }
        if (SaveManager.GetInstance().Get<int>("Scene5ChavalaAngry") == 1)
        {
            HandleChavalaAngryNotification(null, null, "EscenaParque/NPCs/ChavalaIgnoresJuan");
        }
        if (SaveManager.GetInstance().Get<int>("Scene5ChavalaHelped") == 1)
        {
            HandleChavalaHelpedNotification(null, null, "EscenaParque/NPCs/ChavalaDialogueAfterHelp");
        }
        if (SaveManager.GetInstance().Get<int>("Scene5Flowers") == 1)
        {
            HandleFlowersNotification(null, null, null);
        }

        if (SaveManager.GetInstance().Get<int>("Scene5ManteroLeaves") == 1)
        {
            mantero.transform.position = targetPoint;
            chavala.transform.position = targetPoint;
            chaval1.transform.position = targetPoint;
            chaval2.transform.position = targetPoint;
        }
    }

    void HandleChavalaDialogueNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = notificationData;
        chaval1IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        chaval2IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        quiosqueroIO.dialogueID = "EscenaParque/NPCs/QuiosqueroDialogue";
        SaveManager.GetInstance().Set("Scene5ChavalaDialogueFinished", 1);

    }

    void HandleManteroDialogueNotification(string dialogueID, string notificationID, string notificationData)
    {
        manteroIO.dialogueID = notificationData;
        chaval1IO.dialogueID = "EscenaParque/NPCs/ChavalDialogue";
        chavalaIO.dialogueID = "EscenaParque/NPCs/ChavalaDialogueAfterMantero";
        SaveManager.GetInstance().Set("Scene5ManteroDialogueFinished", 1);
    }

    void HandleCapNotification(string dialogueID, string notificationID, string notificationData)
    {
        player.GetComponent<PlayerInventoryManager>().CollectItem(capItem);
        manteroIO.dialogueID = "EscenaParque/NPCs/ManteroDialogueAfterCap";
        quiosqueroIO.dialogueID = "EscenaParque/NPCs/QuiosqueroDialogueAfterPurchase";
        chavalaIO.dialogueID = "EscenaParque/NPCs/ChavalaDialogueAfterCap";
        SaveManager.GetInstance().Set("CapBought", 1);
    }

    void HandleComicNotification(string dialogueID, string notificationID, string notificationData)
    {
        player.GetComponent<PlayerInventoryManager>().CollectItem(comicItem);
        chavalaIO.dialogueID = "EscenaParque/NPCs/ChavalaDialogueAfterComic";
        quiosqueroIO.dialogueID = "EscenaParque/NPCs/QuiosqueroDialogueAfterPurchase";
        SaveManager.GetInstance().Set("ComicBought", 1);
    }

    void HandleManteroAngryNotification(string dialogueID, string notificationID, string notificationData)
    {
        manteroIO.dialogueID = notificationData;
        SaveManager.GetInstance().Set("Scene5ManteroAngry", 1);
    }

    void HandleChavalaAngryNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = notificationData;
        chaval1IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        chaval2IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        SaveManager.GetInstance().Set("Scene5ChavalaAngry", 1);
    }

    void HandleChavalaHelpedNotification(string dialogueID, string notificationID, string notificationData)
    {
        player.GetComponent<PlayerInventoryManager>().RemoveItem(comicItem);
        player.GetComponent<PlayerInventoryManager>().CollectItem(capItem);
        chavalaIO.dialogueID = notificationData;
        manteroIO.dialogueID = "EscenaParque/NPCs/ManteroDialogueAfterComic";
        SaveManager.GetInstance().Set("Scene5ChavalaHelped", 1);
    }

    void HandleManteroLeavesNotification(string dialogueID, string notificationID, string notification)
    {
        runaway = true;
        player.GetComponent<PlayerInventoryManager>().CollectInformation(quiqueTraidorInfo);
        mantero.GetComponent<SpriteRenderer>().sprite = manteroLookingRight;
        SaveManager.GetInstance().Set("Scene5ManteroLeaves", 1);
    }

    void HandleFlowersNotification(string dialogueID, string notificationID, string notificationData)
    {
        SaveManager.GetInstance().Set("Scene5Flowers", 1);
        flowers.SetActive(false);
        player.GetComponent<PlayerInventoryManager>().CollectItem(flowersItem);
    }

    void HandleJuegoPulsoNotification(string dialogueID, string notificationID, string notificationData)
    {
        audioController.ForcePlayWithTransition(minigameMusic, true);

        // Instantiate the minigame in the canvas
        GameObject juegoPulso = Instantiate(juegoPulsoPrefab, canvas.transform);
        ArmWrestlingMinigame armWrestlingMinigame = juegoPulso.GetComponent<ArmWrestlingMinigame>();
        armWrestlingMinigame.finishEvent.AddListener(HandlePulsoFinished);
    }

    void HandlePulsoFinished(bool won, bool skipped)
    {
        audioController.ForcePlayWithTransition(overworldMusic, true);

        if (skipped) {
            DialogueSystem.GetInstance().StartDialogue("EscenaParque/NPCs/ChavalaDialogueAfterSkippedPulso");
        } else if (won) {
            DialogueSystem.GetInstance().StartDialogue("EscenaParque/NPCs/ChavalaDialogueAfterPulso");
        } else {
            DialogueSystem.GetInstance().StartDialogue("EscenaParque/NPCs/ChavalaDialogueAfterLostPulso");
        }
    }

    void Update()
    {
        // Verifica que exista un punto de destino
        if (runaway)
        {
            mantero.GetComponent<BoxCollider2D>().enabled = false;

            // Mueve el objeto hacia el punto objetivo
            mantero.transform.position = Vector2.MoveTowards(mantero.transform.position, targetPoint, speed * Time.deltaTime);
            chavala.transform.position = Vector2.MoveTowards(chavala.transform.position, targetPoint, speed * Time.deltaTime);
            chaval1.transform.position = Vector2.MoveTowards(chaval1.transform.position, targetPoint, speed * Time.deltaTime);
            chaval2.transform.position = Vector2.MoveTowards(chaval1.transform.position, targetPoint, speed * Time.deltaTime);

            // Si el objeto llega al punto objetivo, detiene el movimiento
            if ((Vector2)mantero.transform.position == targetPoint)
            {
                runaway = false;
            }
        }
    }
}
