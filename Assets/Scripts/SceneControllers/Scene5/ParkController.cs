using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkController : MonoBehaviour
{

    public GameObject chavala;
    public GameObject chaval1;
    public GameObject chaval2;
    public GameObject mantero;
    public GameObject quiosquero;

    public Vector2 targetPoint; 
    public float speed = 0.1f; 
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

        DialogueSystem.GetInstance().HandleNotification("ChavalaDialogueFinished", HandleChavalaDialogueNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroDialogueFinished", HandleManteroDialogueNotification);
        DialogueSystem.GetInstance().HandleNotification("CapBought", HandleCapNotification);
        DialogueSystem.GetInstance().HandleNotification("ComicBought", HandleComicNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroAngry", HandleManteroAngryNotification);
        DialogueSystem.GetInstance().HandleNotification("ChavalaAngry", HandleChavalaAngryNotification);
        DialogueSystem.GetInstance().HandleNotification("ManteroLeaves", HandleManteroLeavesNotification);
        DialogueSystem.GetInstance().HandleNotification("ChavalaHelped", HandleChavalaHelpedNotification);
    }

    void HandleChavalaDialogueNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = notificationData;
        chaval1IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        chaval2IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";

    }

    void HandleManteroDialogueNotification(string dialogueID, string notificationID, string notificationData)
    {
        manteroIO.dialogueID = notificationData;
    }

    void HandleCapNotification(string dialogueID, string notificationID, string notificationData)
    {
        manteroIO.dialogueID = "EscenaParque/NPCs/ManteroDialogueAfterCap";
        quiosqueroIO.dialogueID = "EscenaParque/NPCs/QuiosqueroDialogueAfterPurchase";
        chavalaIO.dialogueID = "EscenaParque/NPCs/ChavalaDialogueAfterCap";
    }

    void HandleComicNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = "EscenaParque/NPCs/ChavalaDialogueAfterComic";
        quiosqueroIO.dialogueID = "EscenaParque/NPCs/QuiosqueroDialogueAfterPurchase";
    }

    void HandleManteroAngryNotification(string dialogueID, string notificationID, string notificationData)
    {
        manteroIO.dialogueID = notificationData;
    }

    void HandleChavalaAngryNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = notificationData;
        chaval1IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
        chaval2IO.dialogueID = "EscenaParque/NPCs/ChavalDialogueRepeat";
    }

    void HandleChavalaHelpedNotification(string dialogueID, string notificationID, string notificationData)
    {
        chavalaIO.dialogueID = notificationData;
        manteroIO.dialogueID = "EscenaParque/NPCs/ManteroDialogueAfterComic";
    }

    void HandleManteroLeavesNotification(string dialogueID, string notificationID, string notification)
    {
        runaway = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica que exista un punto de destino
        if (runaway)
        {
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
