using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene7Controller : MonoBehaviour
{
    public GameObject metro;
    public GameObject player;
    public GameObject thief;
    public GameObject combate;
    public int thiefSpeed = 5;
    public int metroSpeed = 50;
    public float trackingMinDistance = 1f;
    public Vector3 scaleIncrease = new Vector3(2, 1, 0);

    // Start is called before the first frame update
    void Start(){
        DialogueSystem.GetInstance().HandleNotification("SubirAlMetro", HandleSubirAlMetroNotification);
        DialogueSystem.GetInstance().HandleNotification("Hostiazo", HandleHostiazoNotification);
        DialogueSystem.GetInstance().HandleNotification("Combate", HandleCombateNotification);
        DialogueSystem.GetInstance().HandleNotification("LlegaMetro", HandleLlegaMetroNotification);
        combate.transform.localScale = new Vector3(2, 1, 1);
        combate.transform.position = new Vector3(-12, 4, -1);
        combate.SetActive(false);
        metro.SetActive(false);
        
    }

    private IEnumerator MoveLadronTowards(Vector2 position, int velocidad)
    {
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
        thief.SetActive(false);
        yield return MoveMetro(new Vector3(metro.transform.position.x -50, metro.transform.position.y, metro.transform.position.z), metroSpeed);
    }

    private IEnumerator MoveMetro(Vector2 position, int velocidad)
    {
        while (Vector2.Distance(metro.transform.position, position) > trackingMinDistance)
        {
            metro.transform.position = Vector3.MoveTowards(metro.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    void HandleCombateNotification(string dialogueId, string notificationId, string notificationData) {
        combate.SetActive(true);
        StartCoroutine(ScaleCombat());
    }
    void HandleLlegaMetroNotification(string dialogueId, string notificationId, string notificationData) {
        metro.transform.localPosition = new Vector3(-12, 6, -1);
        metro.SetActive(true);
        combate.SetActive(false);
    }

    void HandleSubirAlMetroNotification(string dialogueId, string notificationId, string notificationData) {
        StartCoroutine(MoveLadronTowards(new Vector2(thief.transform.position.x, thief.transform.position.y+4), thiefSpeed));
        
    }
    void HandleHostiazoNotification(string dialogueId, string notificationId, string notificationData) {
        StartCoroutine(Hostiazo());
    }
    

    private IEnumerator ScaleCombat(){
        while (combate.transform.localScale.x < 34) {
            combate.transform.position = player.transform.position;
            combate.transform.localScale += scaleIncrease * Time.deltaTime * 15;
            yield return null;
        }
    }
    
    private IEnumerator Hostiazo(){
        int velocidad = 2;
        Vector2 position = new Vector2(thief.transform.position.x+2, thief.transform.position.y);
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
            yield return Hostiazo2();
    }
    private IEnumerator Hostiazo2 (){
        int velocidad = 20;
        Vector2 position = new Vector2(thief.transform.position.x-3, thief.transform.position.y);
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, player.transform.position, velocidad * Time.deltaTime);
            yield return null;
        }   
    }

 }

