using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robbery : MonoBehaviour
{
    public GameObject mom;
    public GameObject player;
    public GameObject thief;
    public Sprite thiefLookingRight;
    public Sprite regualrThief;
    public ItemData stolenItem;
    public int thiefSpeed = 10;
    public int thiefRunningSpeed = 50;
    public float trackingMinDistance = 1f;
    public AudioClip robberyMusic;
    public AudioClip overworldMusic;
    public GameObject audioController;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveManager.GetInstance().Get<int>("Scene1RobberyFinished") == 1)
        {
            OnTriggerExit2D(null);
        }
        else
        {
            if (SaveManager.GetInstance().Get<int>("TurraDicha") == 0)
            {
                player.GetComponent<PlayerInventoryManager>().CollectItem(stolenItem);
                DialogueSystem.GetInstance().StartDialogue("NPCs/MomDialogue1");
                SaveManager.GetInstance().Set("TurraDicha", 1);
            }
            else
            {
                mom.GetComponent<InteractableObject>().dialogueID = "NPCs/MomDialogue0";
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Starting robbery cutscene.");
            player.GetComponent<PlayerInventoryManager>().RemoveItem(stolenItem);
            thief.GetComponent<SpriteRenderer>().sprite = thiefLookingRight;
            thief.GetComponent<SpriteRenderer>().flipX = true;
            StartCoroutine(MoveLadronTowards(player.transform.position, thiefSpeed));

            DialogueSystem.GetInstance().HandleNotification("ThiefRun", HandleThiefRunNotification);
            DialogueSystem.GetInstance().HandleNotification("ThiefSteal", HandleThiefStealNotification);
            DialogueSystem.GetInstance().HandleNotification("BackToOverworldMusic", HandleBackToOverworldMusic);
            DialogueSystem.GetInstance().StartDialogue("NPCs/ThiefDialogue1");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        SaveManager.GetInstance().Set("Scene1RobberyFinished", 1);
        gameObject.SetActive(false);
        thief.SetActive(false);
        mom.GetComponent<InteractableObject>().dialogueID = "NPCs/MomDialogue2";
    }

    private IEnumerator MoveLadronTowards(Vector2 position, int velocidad)
    {
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }

        thief.GetComponent<SpriteRenderer>().sprite = regualrThief;
        thief.GetComponent<SpriteRenderer>().flipX = false;
    }

    void HandleThiefRunNotification(string dialogueId, string notificationId, string notificationData) {
        thief.GetComponent<SpriteRenderer>().sprite = thiefLookingRight;
        StartCoroutine(MoveLadronTowards(new Vector2(200, thief.transform.position.y), thiefRunningSpeed));
    }

    void HandleThiefStealNotification(string dialogueId, string notificationId, string notificationData) {
        audioController.GetComponent<AudioController>().ForcePlayWithTransition(robberyMusic, true);
    }

    void HandleBackToOverworldMusic(string dialogueId, string notificationId, string notificationData) {
        audioController.GetComponent<AudioController>().ForcePlayWithTransition(overworldMusic, true);
    }
 }

