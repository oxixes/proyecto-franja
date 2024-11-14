using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTruckController : MonoBehaviour
{
    private int counter = 0;
    public GameObject container1;
    public GameObject container2;
    public GameObject container3;
    public GameObject manolo;

    private InteractableObject manoloIO;
    // Start is called before the first frame update
    void Start()
    {
        manoloIO = manolo.GetComponent<InteractableObject>();
        DialogueSystem.GetInstance().HandleNotification("ContainerThrown", HandleNotification);
        DialogueSystem.GetInstance().HandleNotification("AllContainersThrown", HandleRiotFinishedNotification);
        DialogueSystem.GetInstance().HandleNotification("ManoloEncountered", HandleManoloNotification);
    }

    void HandleNotification(string dialogueID, string notificationID, string notificationData)
    {
        counter += 1;
        StartCoroutine(StartContainerDialogue());
        if (notificationData == "1")
        {
            container1.SetActive(false);
        }
        else if (notificationData == "2")
        {
            container2.SetActive(false);
        }
        else if (notificationData == "3")
        {
            container3.SetActive(false);
        }
    }

    void HandleRiotFinishedNotification(string dialogueID, string notificationID, string notificationData)
    {
        manoloIO.dialogueID = "NPCs/ManoloHappy";
        StartCoroutine(StartRiotFinishedDialogue());
    }

    void HandleManoloNotification(string dialogueID, string notificationID, string notificationData)
    {
        manoloIO.dialogueID = notificationData;
    }

    IEnumerator StartContainerDialogue()
    {
        yield return new WaitForSeconds(.1f);
        if (counter == 1)
        {
            DialogueSystem.GetInstance().StartDialogue("Objetos/Thrown1");
        }
        else if (counter == 2)
        {
            DialogueSystem.GetInstance().StartDialogue("Objetos/Thrown2");
        }
        else if (counter == 3)
        {
            DialogueSystem.GetInstance().StartDialogue("Objetos/Thrown3");
        }
    }

    IEnumerator StartRiotFinishedDialogue()
    {
        yield return new WaitForSeconds(.1f);
        DialogueSystem.GetInstance().StartDialogue("Other/RiotFinished");
        gameObject.SetActive(false);
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
