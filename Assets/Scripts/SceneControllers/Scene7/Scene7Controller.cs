using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scene7Controller : MonoBehaviour
{
    public GameObject metro;
    public GameObject player;
    public GameObject thief;
    public GameObject canvas;
    public GameObject combatPrefab;
    public GameObject kasHelper;

    public GameObject fadeOutPanel;
    public GameObject endText;
    public GameObject creditsText;
    public GameObject thanksText;

    public GameObject audioController;
    public AudioClip combatMusic1;
    public AudioClip combatMusic2;

    public Sprite thiefLookingUp;
    public Sprite thiefLookingRight;
    public Sprite thiefLookingDown;

    public int thiefSpeed = 5;
    public int metroSpeed = 50;
    public float trackingMinDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        DialogueSystem.GetInstance().HandleNotification("SubirAlMetro", HandleSubirAlMetroNotification);
        DialogueSystem.GetInstance().HandleNotification("Hostiazo", HandleHostiazoNotification);
        DialogueSystem.GetInstance().HandleNotification("Combate", HandleCombateNotification);
        DialogueSystem.GetInstance().HandleNotification("LlegaMetro", HandleLlegaMetroNotification);
        DialogueSystem.GetInstance().HandleNotification("SpawnKasInCombat", HandleSpawnKas);
        metro.SetActive(false);
    }

    private IEnumerator MoveLadronTowards(Vector2 position, int velocidad)
    {
        thief.GetComponent<SpriteRenderer>().sprite = thiefLookingUp;

        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }

        thief.SetActive(false);

        metro.GetComponent<AnimationEndNotifier>().onAnimationEnd2.AddListener(HandleMetroDoorsClose);
        metro.GetComponent<Animator>().SetTrigger("CloseDoors");
    }

    private IEnumerator MoveMetro(Vector2 position, float velocidad)
    {
        while (Vector2.Distance(metro.transform.position, position) > trackingMinDistance)
        {
            metro.transform.position = Vector3.MoveTowards(metro.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    void HandleCombateNotification(string dialogueId, string notificationId, string notificationData)
    {
        audioController.GetComponent<AudioController>().ForcePlayWithTransition(combatMusic1, false);
        audioController.GetComponent<AudioController>().EnqueueAudio(combatMusic2, true);

        // Instantiate the prefab in the canvas
        GameObject combat = Instantiate(combatPrefab, canvas.transform);
        CombatMinigame combatMinigame = combat.GetComponent<CombatMinigame>();

        combatMinigame.finishEvent.AddListener(HandleCombatEnd);
    }

    void HandleCombatEnd(int maxScore, int score, bool skipped)
    {
        audioController.GetComponent<AudioController>().StopWithTransition();

        if (skipped)
        {
            DialogueSystem.GetInstance().StartDialogue("Scene7/CombatSkip");
        }
        else
        {
            if (score >= maxScore)
            {
                DialogueSystem.GetInstance().StartDialogue("Scene7/TrueEnding");
            }
            else if (score >= maxScore / 2)
            {
                DialogueSystem.GetInstance().StartDialogue("Scene7/Victory");
            }
            else
            {
                DialogueSystem.GetInstance().StartDialogue("Scene7/Defeat");
            }
        }
    }

    void HandleSpawnKas(string dialogueId, string notificationId, string notificationData)
    {
        kasHelper.SetActive(true);
        StartCoroutine(MoveKasToFinalPosition());
    }

    void HandleLlegaMetroNotification(string dialogueId, string notificationId, string notificationData)
    {
        metro.SetActive(true);
        StartCoroutine(MoveMetroStart());
    }

    void HandleSubirAlMetroNotification(string dialogueId, string notificationId, string notificationData)
    {
        metro.GetComponent<AnimationEndNotifier>().onAnimationEnd.AddListener(HandleMetroDoorsOpen);
        metro.GetComponent<Animator>().SetTrigger("OpenDoors");
    }

    void HandleMetroDoorsOpen()
    {
        StartCoroutine(MoveLadronTowards(new Vector2(thief.transform.position.x, thief.transform.position.y + 5), thiefSpeed));
    }

    void HandleMetroDoorsClose()
    {
        StartCoroutine(MoveMetroAndPlayEnd());
    }

    IEnumerator MoveMetroAndPlayEnd()
    {
        yield return MoveMetro(new Vector3(metro.transform.position.x - 50, metro.transform.position.y, metro.transform.position.z), metroSpeed);

        int contextualHints = PlayerPrefs.GetInt("ShowContextualHints", 0);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("ShowContextualHints", contextualHints);
        PlayerPrefs.Save();

        Inventory playerInventory = new Inventory();
        playerInventory.DeleteInventory();

        StartCoroutine(EndSequence());
    }

    void HandleHostiazoNotification(string dialogueId, string notificationId, string notificationData)
    {
        StartCoroutine(Hostiazo());
    }

    private IEnumerator MoveMetroStart()
    {
        float velocidad = 40f;
        Vector2 position = new Vector2(metro.transform.position.x - 50, metro.transform.position.y);
        while (Vector2.Distance(metro.transform.position, position) > trackingMinDistance)
        {
            metro.transform.position = Vector2.MoveTowards(metro.transform.position, position, velocidad * Time.deltaTime);
            velocidad *= 0.9995f;
            yield return null;
        }
    }

    private IEnumerator Hostiazo()
    {
        thief.GetComponent<SpriteRenderer>().sprite = thiefLookingRight;
        // Flip the sprite
        thief.GetComponent<SpriteRenderer>().flipX = true;

        float velocidad = 1.5f;
        Vector2 position = new Vector2(thief.transform.position.x + 3, thief.transform.position.y);
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }

        yield return Hostiazo2();
    }

    private IEnumerator Hostiazo2()
    {
        int velocidad = 20;
        Vector2 position = new Vector2(thief.transform.position.x - 4, thief.transform.position.y);
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }

        position = new Vector2(thief.transform.position.x + 3, thief.transform.position.y);
        while (Vector2.Distance(thief.transform.position, position) > trackingMinDistance)
        {
            thief.transform.position = Vector2.MoveTowards(thief.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }

        thief.GetComponent<SpriteRenderer>().sprite = thiefLookingDown;
        thief.GetComponent<SpriteRenderer>().flipX = false;
    }

    private IEnumerator MoveKasToFinalPosition()
    {
        int velocidad = 5;
        Vector2 position = new Vector2(kasHelper.transform.position.x + 3, kasHelper.transform.position.y);
        while (Vector2.Distance(kasHelper.transform.position, position) > 1)
        {
            kasHelper.transform.position = Vector2.MoveTowards(kasHelper.transform.position, position, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator EndSequence()
    {
        Minigame.isInMinigame = true;

        yield return new WaitForSeconds(2f);

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        fadeOutPanel.SetActive(true);

        // Animate fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(3f);
        endText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
        endText.SetActive(true);

        yield return new WaitForSeconds(3f);
        // Animate endText fade out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            endText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        endText.SetActive(false);

        creditsText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        creditsText.SetActive(true);

        yield return new WaitForSeconds(2f);

        // Animate credits fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            creditsText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(7f);

        // Animate credits fade out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            creditsText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        creditsText.SetActive(false);

        thanksText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        thanksText.SetActive(true);

        yield return new WaitForSeconds(2f);

        // Animate thanks fade in
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            thanksText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        fadeOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(5f);

        // Animate thanks fade out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            thanksText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, i);
            yield return null;
        }

        thanksText.SetActive(false);

        yield return new WaitForSeconds(2f);

        Minigame.isInMinigame = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

