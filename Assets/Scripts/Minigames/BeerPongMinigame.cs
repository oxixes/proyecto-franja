using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerPongGame : MonoBehaviour
{
    public GameObject canvas;
    public TMP_Text resultText;
    public TMP_Text winText;
    public Slider horizontalSlider;
    public Slider verticalSlider;
    public GameObject introPanel; // Panel for the intro screen
    public TMP_Text introText; // Text for the intro panel

    private GameObject ball;
    private RectTransform horizontalGreenZone;
    private RectTransform verticalGreenZone;

    private bool isPlaying = false;
    private bool isHorizontalStopped = false;
    private bool isVerticalStopped = false;
    private bool isVerticalVisible = false;

    private float horizontalPower;
    private float verticalPower;

    private int consecutiveHits = 0;
    private Vector2 initialHorizontalZoneSize = new Vector2(100, 20);
    private Vector2 initialVerticalZoneSize = new Vector2(20, 100);
    private float greenZoneReductionFactor = 0.8f;
    private float horizontalSpeed = 500f;
    private float verticalSpeed = 300f;
    private float speedIncreaseFactor = 1.2f;

    void Start()
    {
        if (introPanel == null || introText == null)
        {
            Debug.LogError("IntroPanel or IntroText is not assigned in the Inspector!");
            return;
        }

        // Configure the intro panel appearance
        var panelImage = introPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = new Color(0, 0, 0, 0.95f); // Black with 95% opacity
        }

        // Set the intro text
        introText.text = "Press SPACE to start!\n\nInstructions:\n- Press SPACE to stop the sliders.\n- Align direction and power to hit 3 times consecutively to win.";
        introText.alignment = TextAlignmentOptions.Center;
        introText.fontSize = 30; // Smaller font size
        introText.rectTransform.sizeDelta = new Vector2(800, 200); // Wider text area

        introPanel.SetActive(true); // Activate the intro panel at the start

        canvas.SetActive(true);

        ball = new GameObject("Ball");
        var ballTransform = ball.AddComponent<RectTransform>();
        ballTransform.sizeDelta = new Vector2(100, 100);
        ball.transform.SetParent(canvas.transform, false); // Ensure it stays under the canvas
        ball.SetActive(false); // Ball starts hidden

        var ballRenderer = ball.AddComponent<Image>();
        ballRenderer.sprite = GenerateCircleSprite();
        ballRenderer.color = Color.yellow;

        horizontalSlider.minValue = 0;
        horizontalSlider.maxValue = Screen.width;
        horizontalSlider.value = Screen.width / 2;
        horizontalSlider.gameObject.transform.SetSiblingIndex(0); // Ensure sliders are behind the intro panel

        var horizontalSliderRect = horizontalSlider.GetComponent<RectTransform>();
        horizontalSliderRect.anchorMin = new Vector2(0.5f, 0.1f);
        horizontalSliderRect.anchorMax = new Vector2(0.5f, 0.1f);
        horizontalSliderRect.pivot = new Vector2(0.5f, 0.5f);
        horizontalSliderRect.anchoredPosition = new Vector2(0, 0);
        horizontalSliderRect.sizeDelta = new Vector2(400, 20);

        verticalSlider.minValue = 0;
        verticalSlider.maxValue = Screen.height;
        verticalSlider.value = Screen.height / 2;
        verticalSlider.gameObject.SetActive(false);
        verticalSlider.gameObject.transform.SetSiblingIndex(0); // Ensure sliders are behind the intro panel

        var verticalSliderRect = verticalSlider.GetComponent<RectTransform>();
        verticalSliderRect.anchorMin = new Vector2(0.1f, 0.5f);
        verticalSliderRect.anchorMax = new Vector2(0.1f, 0.5f);
        verticalSliderRect.pivot = new Vector2(0.5f, 0.5f);
        verticalSliderRect.anchoredPosition = new Vector2(0, 0);
        verticalSliderRect.sizeDelta = new Vector2(400, 20);
        verticalSliderRect.localEulerAngles = new Vector3(0, 0, 90);

        horizontalGreenZone = CreateGreenZone(horizontalSlider, initialHorizontalZoneSize);
        verticalGreenZone = CreateGreenZone(verticalSlider, initialVerticalZoneSize);

        ConfigureText(resultText, "", new Vector3(Screen.width / 2 - 200, Screen.height / 2 - 100, 0)); // Offset position for result text
        ConfigureText(winText, "", new Vector3(Screen.width / 2 - 200, Screen.height / 2 - 100, 0)); // Offset position for win text
    }

    void Update()
    {
        if (!isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
            return;
        }

        if (isPlaying)
        {
            if (!isHorizontalStopped && !isVerticalVisible)
            {
                horizontalSlider.value = Mathf.PingPong(Time.time * horizontalSpeed, horizontalSlider.maxValue);
            }
            else if (!isVerticalStopped && isVerticalVisible)
            {
                verticalSlider.value = Mathf.PingPong(Time.time * verticalSpeed, verticalSlider.maxValue);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isHorizontalStopped && !isVerticalVisible)
                {
                    isHorizontalStopped = true;
                    horizontalPower = horizontalSlider.value;
                    verticalSlider.gameObject.SetActive(true);
                    isVerticalVisible = true;
                }
                else if (!isVerticalStopped && isVerticalVisible)
                {
                    isVerticalStopped = true;
                    verticalPower = verticalSlider.value;
                    LaunchBall();
                }
            }
        }
    }

    private void StartGame()
    {
        introPanel.SetActive(false); // Hide the intro panel
        ball.SetActive(true); // Show the ball
        ResetBallPosition();
        isPlaying = true;
    }

    private RectTransform CreateGreenZone(Slider slider, Vector2 size)
    {
        var greenZone = new GameObject("GreenZone");
        var rectTransform = greenZone.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.SetParent(slider.transform);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        var renderer = greenZone.AddComponent<Image>();
        renderer.color = Color.green;

        return rectTransform;
    }

    private Sprite GenerateCircleSprite()
    {
        Texture2D texture = new Texture2D(100, 100);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float dx = x - texture.width / 2;
                float dy = y - texture.height / 2;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                texture.SetPixel(x, y, distance <= texture.width / 2 ? Color.white : Color.clear);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void LaunchBall()
    {
        float xForce = (horizontalPower / Screen.width) * 900f;
        float yForce = (verticalPower / Screen.height) * 900f;

        var ballRigidbody = ball.AddComponent<Rigidbody2D>();
        ballRigidbody.gravityScale = 0;
        ballRigidbody.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);

        StartCoroutine(DisplayResultAndReset());
    }

    private IEnumerator DisplayResultAndReset()
    {
        yield return new WaitUntil(() => Mathf.Abs(ball.transform.position.x) > Screen.width || Mathf.Abs(ball.transform.position.y) > Screen.height);

        bool hitHorizontal = Mathf.Abs(horizontalPower - horizontalGreenZone.position.x) <= horizontalGreenZone.rect.width / 2;
        bool hitVertical = Mathf.Abs(verticalPower - verticalGreenZone.position.y) <= verticalGreenZone.rect.height / 2;

        if (hitHorizontal && hitVertical)
        {
            consecutiveHits++;
            resultText.text = "Acertaste!";
            resultText.alignment = TextAlignmentOptions.Center;
            resultText.rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - 500, Screen.height / 2 - 300); // Offset position for result text

            if (consecutiveHits >= 3)
            {
                winText.text = "GANASTE";
                winText.alignment = TextAlignmentOptions.Center;
                winText.rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - 500, Screen.height / 2 - 300); // Offset position for win text
                winText.gameObject.SetActive(true);
                isPlaying = false;
                yield break;
            }

            horizontalGreenZone.sizeDelta *= greenZoneReductionFactor;
            verticalGreenZone.sizeDelta *= greenZoneReductionFactor;

            horizontalSpeed *= speedIncreaseFactor;
            verticalSpeed *= speedIncreaseFactor;
        }
        else
        {
            consecutiveHits = 0;
            resultText.text = "Fallaste!";
            resultText.alignment = TextAlignmentOptions.Center;
            resultText.rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - 200, Screen.height / 2 - 100); // Offset position for result text

            horizontalGreenZone.sizeDelta = initialHorizontalZoneSize;
            verticalGreenZone.sizeDelta = initialVerticalZoneSize;

            horizontalSpeed = 500f;
            verticalSpeed = 300f;
        }

        resultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        resultText.gameObject.SetActive(false);
        ResetGame();
    }

    private void ResetGame()
    {
        Destroy(ball.GetComponent<Rigidbody2D>());
        ResetBallPosition();

        horizontalSlider.value = Screen.width / 2;
        verticalSlider.value = Screen.height / 2;
        verticalSlider.gameObject.SetActive(false);

        isHorizontalStopped = false;
        isVerticalStopped = false;
        isVerticalVisible = false;

        resultText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
    }

    private void ResetBallPosition()
    {
        ball.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    private void ConfigureText(TMP_Text text, string content, Vector3 position)
    {
        text.text = content;
        text.fontSize = 50;
        text.alignment = TextAlignmentOptions.Center;
        text.rectTransform.sizeDelta = new Vector2(400, 100);
        text.rectTransform.position = position;

        var shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        var background = new GameObject("Background");
        background.transform.SetParent(text.transform);
        var backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.5f);
        background.GetComponent<RectTransform>().sizeDelta = text.rectTransform.sizeDelta;
        background.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        background.transform.SetAsFirstSibling();

        text.gameObject.SetActive(false);
    }
}
