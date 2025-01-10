using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashTruckController : MonoBehaviour
{
    private int counter = 0;
    public GameObject container1;
    public GameObject container2;
    public GameObject container3;
    public GameObject manolo;
    public GameObject fadeOutPanel;
    public GameObject trashTruck;
    public GameObject collarItem;

    private InteractableObject manoloIO;
    // Start is called before the first frame update
    void Start()
    {
        manoloIO = manolo.GetComponent<InteractableObject>();
        container1.SetActive(false);
        container2.SetActive(false);
        container3.SetActive(false);

        if (SaveManager.GetInstance().Get<int>("Scene2RiotFinished") == 1)
        {
            manoloIO.dialogueID = "NPCs/ManoloHappy";
        }

        if (SaveManager.GetInstance().Get<int>("Scene2ManoloDialogueFinished") == 1)
        {
            HandleManoloNotification(SaveManager.GetInstance().Get<string>("Scene2ManoloDialogueID"), null,
                SaveManager.GetInstance().Get<string>("Scene2ManoloDialogueData"));
        }

        counter = SaveManager.GetInstance().Get<int>("Scene2ContainersThrown");

        if (SaveManager.GetInstance().Get<int>("Scene2Container1Thrown") == 1)
        {
            container1.SetActive(false);
        }

        if (SaveManager.GetInstance().Get<int>("Scene2Container2Thrown") == 1)
        {
            container2.SetActive(false);
        }

        if (SaveManager.GetInstance().Get<int>("Scene2Container3Thrown") == 1)
        {
            container3.SetActive(false);
            if (SaveManager.GetInstance().Get<int>("Scene2CollarDialogueFinished") == 0)
            {
                collarItem.SetActive(true);
            }
        }

        DialogueSystem.GetInstance().HandleNotification("ContainerThrown", HandleNotification);
        DialogueSystem.GetInstance().HandleNotification("AllContainersThrown", HandleRiotFinishedNotification);
        DialogueSystem.GetInstance().HandleNotification("ManoloDialogueFinished", HandleManoloNotification);
    }

    void HandleNotification(string dialogueID, string notificationID, string notificationData)
    {
        counter += 1;
        SaveManager.GetInstance().Set("Scene2ContainersThrown", counter);
        StartCoroutine(StartContainerDialogue());
        if (notificationData == "1")
        {
            container1.SetActive(false);
            SaveManager.GetInstance().Set("Scene2Container1Thrown", 1);
        }
        else if (notificationData == "2")
        {
            container2.SetActive(false);
            SaveManager.GetInstance().Set("Scene2Container2Thrown", 1);
        }
        else if (notificationData == "3")
        {
            container3.SetActive(false);
            collarItem.SetActive(true);
            SaveManager.GetInstance().Set("Scene2Container3Thrown", 1);
        }
    }

    void HandleRiotFinishedNotification(string dialogueID, string notificationID, string notificationData)
    {
        SaveManager.GetInstance().Set("Scene2RiotFinished", 1);
        manoloIO.dialogueID = "NPCs/ManoloHappy";
        StartCoroutine(StartRiotFinishedDialogue());
    }

    void HandleManoloNotification(string dialogueID, string notificationID, string notificationData)
    {
        SaveManager.GetInstance().Set("Scene2ManoloDialogueFinished", 1);
        manoloIO.dialogueID = notificationData;
        SaveManager.GetInstance().Set("Scene2ManoloDialogueID", dialogueID);
        SaveManager.GetInstance().Set("Scene2ManoloDialogueData", notificationData);
        if (dialogueID == "NPCs/ManoloHappy")
        {
            if (notificationID == null)
            {
                trashTruck.SetActive(false);
            }
            else
            {
                StartCoroutine(HideTrashTruck());
            }
        }
        else
        {
            container1.SetActive(true);
            container2.SetActive(true);
            container3.SetActive(true);
        }
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
        yield return null;
    }

    IEnumerator HideTrashTruck()
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

        // Hide trash truck
        trashTruck.SetActive(false);
        yield return new WaitForSeconds(1f);

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
