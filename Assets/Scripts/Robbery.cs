using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robbery : MonoBehaviour
{
    public GameObject mom;
    public GameObject player;
    public GameObject thief;
    public int thiefSpeed = 5;
    public int thiefRunningSpeed = 50;
    public float trackingMinDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {  
        DialogueSystem.GetInstance().StartDialogue("Scene1/MomDialogue1");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Starting robbery cutscene.");
            StartCoroutine(MoveLadronTowards(player.transform.position, thiefSpeed));

            DialogueSystem.GetInstance().HandleNotification("ThiefRun", HandleThiefRunNotification);
            DialogueSystem.GetInstance().StartDialogue("Scene1/ThiefDialogue1");
            mom.GetComponent<InteractableObject>().dialogueID = "Scene1/MomDialogue2";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        gameObject.SetActive(false);
    }

    private IEnumerator MoveLadronTowards(Vector2 position, int velocidad)
    {
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    void HandleThiefRunNotification(string dialogueId, string notificationId, string notificationData) {
        StartCoroutine(MoveLadronTowards(new Vector2(200, thief.transform.position.y), thiefRunningSpeed));
    }

 }

