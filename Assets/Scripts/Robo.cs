using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robo : MonoBehaviour
{
    public GameObject mama;
    public GameObject trigger;
    private GameObject player;
    private GameObject ladron;
    public int speed = 5;

    public int huida = 50;
    public float distancia= 1f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ladron = GameObject.Find("Thief"); 
        mama = GameObject.Find("Mama");
       
        DialogueSystem.GetInstance().StartDialogue("Escena1/MomDialogue1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Robo incoming");
        }
        StartCoroutine(MoveLadronTowards(player.transform.position, speed));
        
        DialogueSystem.GetInstance().HandleNotification("robado", ACorrer);
        DialogueSystem.GetInstance().StartDialogue("Escena1/ThiefDialogue1");
        mama.GetComponent<InteractableObject>().dialogueID = "Escena1/MomDialogue2";

    }

    void OnTriggerExit2D(Collider2D other)
    {
               trigger.SetActive(false);
    }
    private IEnumerator MoveLadronTowards(Vector2 position, int velocidad)
    {
        bool alcanzado= false;
        while (Vector2.Distance(ladron.transform.position, position) > distancia&&!alcanzado)
        {
        ladron.transform.position = Vector2.MoveTowards(ladron.transform.position, position, velocidad * Time.deltaTime);
        yield return null;
        }
        alcanzado=true;
    }
    void ACorrer(string dialogueId, string notificationId, string notificationData) {
        Debug.Log("Corre ladron corre");
        
        StartCoroutine(MoveLadronTowards(new Vector2(200, ladron.transform.position.y), huida));
    }

 }

