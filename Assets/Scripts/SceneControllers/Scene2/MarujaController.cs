using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MarujaController : MonoBehaviour
{
    public GameObject minigamePrefab;
    public GameObject player;
    public InformationData marujaInformation;

    private InteractableObject marujaIO;
    private GameObject canvas;
    private PlayerInventoryManager playerInventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        marujaIO = gameObject.GetComponent<InteractableObject>();
        canvas = GameObject.Find("Canvas");

        if (SaveManager.GetInstance().Get<int>("Scene2MarujaDialogueFinished") == 1)
        {
            HandleMarujaDialogueFinishedNotification(SaveManager.GetInstance().Get<string>("Scene2MarujaDialogueID"), null,
                SaveManager.GetInstance().Get<string>("Scene2MarujaDialogueData"));
        }

        DialogueSystem.GetInstance().HandleNotification("MarujaDialogueFinished", HandleMarujaDialogueFinishedNotification);
        DialogueSystem.GetInstance().HandleNotification("MarujaMinigame", HandleMinigameStartNotification);

        playerInventoryManager = player.GetComponent<PlayerInventoryManager>();
    }

    void HandleMarujaDialogueFinishedNotification(string dialogueID, string notificationID, string notificationData)
    {
        SaveManager.GetInstance().Set("Scene2MarujaDialogueFinished", 1);
        marujaIO.dialogueID = notificationData;
        SaveManager.GetInstance().Set("Scene2MarujaDialogueID", dialogueID);
        SaveManager.GetInstance().Set("Scene2MarujaDialogueData", notificationData);

        if (dialogueID == "NPCs/MarujaMinigameWon" || dialogueID == "NPCs/MarujaMinigameSkipped") {
            Debug.Log("MarujaController: MarujaDialogueFinishedNotification: Adding Maruja information to player inventory");
            playerInventoryManager.CollectInformation(marujaInformation);
        }
    }

    void HandleMinigameStartNotification(string dialogueID, string notificationID, string notificationData)
    {
        // Instantiate the minigame in the canvas
        GameObject minigame = Instantiate(minigamePrefab, canvas.transform);

        // The minigame's controller has an event when it finishes, subscribe to it
        minigame.GetComponent<ItemsFallingMinigame>().finishEvent.AddListener(HandleOnMinigameFinish);
    }

    void HandleOnMinigameFinish(int total, int score, bool skipped)
    {
        if (skipped)
        {
            DialogueSystem.GetInstance().StartDialogue("NPCs/MarujaMinigameSkipped");
            return;
        }

        float percentage = (float)score / total;
        if (percentage >= 0.5)
        {
            DialogueSystem.GetInstance().StartDialogue("NPCs/MarujaMinigameWon");
        }
        else
        {
            DialogueSystem.GetInstance().StartDialogue("NPCs/MarujaMinigameLost");
        }
    }
}
