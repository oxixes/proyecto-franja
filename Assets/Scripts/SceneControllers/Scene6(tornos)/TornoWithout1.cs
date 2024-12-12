using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornoWithout1 : MonoBehaviour
{
    public GameObject player;
    public GameObject thief;

    public GameObject recharger;
    public int thiefSpeed = 5;
    public int playerSpeed = 5;

    public float distancia = 2;

    public GameObject huida;
    public int bono = 1;
    //public float trackingMinDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {   
        player = GameObject.Find("Player");
        thief = GameObject.Find("Thief");
        DialogueSystem.GetInstance().HandleNotification("Return", Vuelve);
            DialogueSystem.GetInstance().HandleNotification("Bono2", HandleBono2Notification);
            DialogueSystem.GetInstance().HandleNotification("Bono3", HandleBono3Notification);
            DialogueSystem.GetInstance().HandleNotification("Eliminar", Eliminar);
            DialogueSystem.GetInstance().HandleNotification("Irse", Irse);
            bono = SaveManager.GetInstance().Get<int>("Bono");
            if(bono == 3|| bono == 4){
                recharger.GetComponent<InteractableObject>().dialogueID = "Scene6/RechargerDialogue2";
            }
            if (bono == 4){
                thief.SetActive(false);
            }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Starting tornos cutscene.");

            
            if (bono == 0)
            {
                DialogueSystem.GetInstance().StartDialogue("Scene6/TornoDialogue1");
            }
            else if (bono == 2)
            {
                DialogueSystem.GetInstance().StartDialogue("Scene6/TornoDialogue2");
            }
            else if (bono == 3)
            {
                DialogueSystem.GetInstance().StartDialogue("Scene6/TornoDialogue3");
            }
        }
    }


    void HandleBono2Notification(string dialogueId, string notificationId, string notificationData) {
        bono = 2;
        SaveManager.GetInstance().Set("Bono", 2);
    }
    void HandleBono3Notification(string dialogueId, string notificationId, string notificationData) {
        bono = 3;
        recharger.GetComponent<InteractableObject>().dialogueID = "Scene6/RechargerDialogue2";
        SaveManager.GetInstance().Set("Bono", 3);
    }

    void Vuelve(string dialogueId, string notificationId, string notificationData) {
        StartCoroutine(MovePlayerDown(distancia));
    }

    void Eliminar(string dialogueId, string notificationId, string notificationData) {
        gameObject.SetActive(false);
    }

    void Irse(string dialogueId, string notificationId, string notificationData) {
        StartCoroutine(MoveThiefAway());
        SaveManager.GetInstance().Set("Bono", 4);
    }

    private IEnumerator MovePlayerDown(float stepDistance)
{
    // Calcula la posición objetivo desplazada "stepDistance" hacia abajo
    Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y - stepDistance);
    
    // Mueve al jugador hacia la posición objetivo
    while (Vector2.Distance(player.transform.position, targetPosition) > 0.1f) // Menor distancia para precisión
    {
        player.transform.position = Vector2.MoveTowards(player.transform.position, targetPosition, playerSpeed * Time.deltaTime);
        yield return null;
    }

    yield break;
}

private IEnumerator MoveThiefAway(){
    Vector2 targetPosition = new Vector2(huida.transform.position.x, huida.transform.position.y);
    while (Vector2.Distance(thief.transform.position, targetPosition) > 0.1f) // Menor distancia para precisión
    {
        thief.transform.position = Vector2.MoveTowards(thief.transform.position, targetPosition, thiefSpeed * Time.deltaTime);
        yield return null;
    }
    thief.SetActive(false);
    yield break;

 }

}