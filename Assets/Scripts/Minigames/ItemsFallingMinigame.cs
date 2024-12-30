using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemsFallingMinigame : Minigame
{
    public UnityEvent<int, int> finishEvent = new UnityEvent<int, int>();

    public enum FallingMinigameModes
    {
        CatchItems,
        AvoidItems
    }

    public GameObject fallingItemPrefab;
    public GameObject player;
    public GameObject explanation;
    public GameObject timer;
    public GameObject scoreText;
    public float spawnYPos = 320.0f;
    public float spawnRate = 1.0f; // Items per second
    public float spawnRateIncrease = 0.1f; // Items per second
    public float playerMoveSpeed = 30.0f;
    public int gameDuration = 30;
    public int poolSize = 10;
    public float animationDuration = 1f;
    public FallingMinigameModes minigameMode = FallingMinigameModes.CatchItems;

    private bool isInAnimation = true;
    private bool startingAnimation = true;
    private float animationTimer = 0.0f;
    private List<GameObject> fallingItemsPool = new List<GameObject>();
    private int nextItemToSpawn = 0;
    private float lastItemSpawnedTime = 0;
    private float timeLeft;

    private Animator playerAnimator;

    private int totalSpawnedItems = 0;
    private int score = 0;

    private bool gameStarted = false;
    private bool gameFinished = false;

    private RectTransform backgroundTransform;

    // Start is called before the first frame update
    void Start()
    {
        isInMinigame = true;

        GameObject background = GameObject.Find("ItemsFallingMinigame");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject fallingItem = Instantiate(fallingItemPrefab, new Vector3(0, spawnYPos, 0), Quaternion.identity);
            fallingItem.GetComponent<FallingItem>().minigame = this;
            fallingItem.transform.SetParent(background.transform);
            fallingItem.SetActive(false);
            fallingItemsPool.Add(fallingItem);
        }

        // Get child object with the background image
        backgroundTransform = background.GetComponent<RectTransform>();
        backgroundTransform.localScale = new Vector3(0, 0, 0);

        playerAnimator = player.GetComponent<Animator>();

        player.GetComponent<Image>().enabled = false; /* UNITY BUG: Animator stops playing if the object is disabled */
        explanation.SetActive(false);
        timer.SetActive(false);
        scoreText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInAnimation)
        {
            Mathf.Clamp(animationTimer, 0.0f, animationDuration);
            Vector3 scale = (startingAnimation) ? Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1.05f, 1.05f, 1.05f), animationTimer / animationDuration)
                : Vector3.Lerp(new Vector3(1.05f, 1.05f, 1.05f), new Vector3(0, 0, 0), animationTimer / animationDuration);
            backgroundTransform.localScale = scale;

            animationTimer += Time.deltaTime;

            if (animationTimer >= animationDuration)
            {
                backgroundTransform.localScale = (startingAnimation) ? new Vector3(1.05f, 1.05f, 1.05f) : new Vector3(0, 0, 0);
                isInAnimation = false;

                if (!startingAnimation)
                {
                    isInMinigame = false;
                    finishEvent.Invoke(totalSpawnedItems, score);
                    Destroy(gameObject);
                }
            }
            else
            {
                return;
            }
        }

        if (!gameStarted && !gameFinished)
        {
            player.GetComponent<Image>().enabled = true; /* UNITY BUG: Animator stops playing if the object is disabled */
            explanation.SetActive(true);
        }

        if (!gameStarted && !gameFinished && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            explanation.SetActive(false);
            timer.SetActive(true);
            if (minigameMode == FallingMinigameModes.CatchItems)
                scoreText.SetActive(true);
            gameStarted = true;
            lastItemSpawnedTime = Time.time;
            timeLeft = gameDuration;
        }

        if (gameStarted)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal") * playerMoveSpeed * Time.deltaTime, 0, 0);
            player.transform.localPosition = new Vector3(Mathf.Clamp(player.transform.localPosition.x + moveDirection.x * 10, -360, 360),
                player.transform.localPosition.y, player.transform.localPosition.z);

            if ((Time.time - lastItemSpawnedTime) >= (1 / spawnRate))
            {
                fallingItemsPool[nextItemToSpawn].SetActive(false);
                lastItemSpawnedTime = Time.time;
                fallingItemsPool[nextItemToSpawn].transform.localPosition = new Vector3(Random.Range(-360, 360), spawnYPos, 0);
                fallingItemsPool[nextItemToSpawn].SetActive(true);
                nextItemToSpawn = (nextItemToSpawn + 1) % poolSize;

                // Don't count last second items
                if (timeLeft > 2)
                {
                    totalSpawnedItems += 1;
                    if (minigameMode == FallingMinigameModes.AvoidItems)
                        score += 1;
                }
            }

            // Detect if the player has caught an item
            Vector2 playerPos = new Vector2(player.transform.localPosition.x, player.transform.localPosition.y);
            Vector2 playerSize = new Vector2(player.GetComponent<RectTransform>().rect.width, player.GetComponent<RectTransform>().rect.height);
            Rect playerRect = new Rect(playerPos - playerSize / 2, playerSize);

            for (int i = 0; i < poolSize; i++)
            {
                if (fallingItemsPool[i].activeSelf)
                {
                    Vector2 itemPos = new Vector2(fallingItemsPool[i].transform.localPosition.x, fallingItemsPool[i].transform.localPosition.y);
                    Vector2 itemSize = new Vector2(fallingItemsPool[i].GetComponent<RectTransform>().rect.width, fallingItemsPool[i].GetComponent<RectTransform>().rect.height);
                    Rect itemRect = new Rect(itemPos - itemSize / 2, itemSize);
                    if (playerRect.Overlaps(itemRect))
                    {
                        fallingItemsPool[i].SetActive(false);
                        score += (minigameMode == FallingMinigameModes.CatchItems) ? 1 : -1;
                        scoreText.GetComponent<TextMeshProUGUI>().text = "Puntos: " + score.ToString();
                        if (minigameMode == FallingMinigameModes.AvoidItems)
                            playerAnimator.SetTrigger("Hit");
                    }
                }
            }

            spawnRate += spawnRateIncrease * Time.deltaTime;
            timeLeft -= Time.deltaTime;

            int secondsLeft = (int)timeLeft;
            timer.GetComponent<TextMeshProUGUI>().text = secondsLeft.ToString();

            if (timeLeft <= 0)
            {
                gameStarted = false;
                timer.SetActive(false);
                player.GetComponent<Image>().enabled = false; /* UNITY BUG: Animator stops playing if the object is disabled */
                scoreText.SetActive(false);
                gameFinished = true;

                for (int i = 0; i < poolSize; i++)
                {
                    fallingItemsPool[i].SetActive(false);
                    Destroy(fallingItemsPool[i]);
                }

                if (score > totalSpawnedItems)
                    score = totalSpawnedItems;

                if (score < 0)
                    score = 0;

                startingAnimation = false;

                explanation.GetComponent<TextMeshProUGUI>().text = (minigameMode == FallingMinigameModes.CatchItems) ?
                    "¡Has atrapado " + score + " de " + totalSpawnedItems + " objetos!\n\nPresiona espacio para continuar" :
                    "¡Has evitado " + score + " de " + totalSpawnedItems + " objetos!\n\nPresiona espacio para continuar";
                explanation.SetActive(true);
            }
        }

        if (gameFinished)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                explanation.SetActive(false);
                animationTimer = 0.0f;
                isInAnimation = true;
            }
        }
    }

    public void ItemCaught()
    {
        score += 1;
    }
}
