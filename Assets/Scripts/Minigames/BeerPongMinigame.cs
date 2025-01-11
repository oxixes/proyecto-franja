using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class BeerPongGame : MonoBehaviour
{
    public GameObject blackOverlayCanvas;
    public GameObject canvas;
    public GameObject blackPanel;
    public TMP_Text resultText;
    public TMP_Text winText;
    public Slider horizontalSlider;
    public Slider verticalSlider;
    public GameObject introPanel;
    public TMP_Text introText;
    public Button kasHelpButton;

    private RectTransform blackPanelTransform;
    private bool isExpanding = true;
    private bool isClosing = false;
    private float expandTimer = 0.0f;
    public float expandDuration = 2.0f;

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
    private Vector2 initialHorizontalZoneSize = new Vector2(120, 30);
    private Vector2 initialVerticalZoneSize = new Vector2(30, 120);
    private float greenZoneReductionFactor = 0.5f;
    private float horizontalSpeed = 500f;
    private float verticalSpeed = 300f;
    private float speedIncreaseFactor = 1.7f;
    private float thirdRoundToleranceIncrease = 1.5f;

    private bool kasHelpUsed = false;

    public UnityEvent<bool, bool> finishEvent = new UnityEvent<bool, bool>();

    void Start()
    {
        Minigame.isInMinigame = true;

        if (blackPanel != null)
        {
            blackPanelTransform = blackPanel.GetComponent<RectTransform>();
            blackPanelTransform.anchorMin = new Vector2(0, 0);
            blackPanelTransform.anchorMax = new Vector2(1, 1);
            blackPanelTransform.offsetMin = Vector2.zero;
            blackPanelTransform.offsetMax = Vector2.zero;
            blackPanelTransform.pivot = new Vector2(0.5f, 0.5f);
            blackPanelTransform.localScale = new Vector3(0, 0, 1);

            blackOverlayCanvas.SetActive(true);
            canvas.SetActive(false);
        }

        if (canvas == null || introPanel == null || introText == null)
        {
            Debug.LogError("Canvas, IntroPanel o IntroText no están asignados en el Inspector!");
            return;
        }

        var panelImage = introPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = new Color(0, 0, 0, 0.95f);
        }

        introText.text = "Presiona ESPACIO para comenzar!\n\nInstrucciones:\n- Presiona ESPACIO para detener los sliders.\n- Alinea dirección y potencia para acertar 3 veces consecutivas y ganar.";
        introText.alignment = TextAlignmentOptions.Center;
        introText.fontSize = 30;
        introText.rectTransform.sizeDelta = new Vector2(800, 200);

        introPanel.SetActive(true);

        ball = new GameObject("Ball");
        var ballTransform = ball.AddComponent<RectTransform>();
        ballTransform.sizeDelta = new Vector2(100, 100);
        ball.transform.SetParent(canvas.transform, false);
        ball.SetActive(false);

        var ballRenderer = ball.AddComponent<Image>();
        ballRenderer.sprite = GenerateCircleSprite();
        ballRenderer.color = Color.yellow;

        horizontalSlider.minValue = 0;
        horizontalSlider.maxValue = Screen.width;
        horizontalSlider.value = Screen.width / 2;
        horizontalSlider.gameObject.transform.SetSiblingIndex(0);

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
        verticalSlider.gameObject.transform.SetSiblingIndex(0);

        var verticalSliderRect = verticalSlider.GetComponent<RectTransform>();
        verticalSliderRect.anchorMin = new Vector2(0.1f, 0.5f);
        verticalSliderRect.anchorMax = new Vector2(0.1f, 0.5f);
        verticalSliderRect.pivot = new Vector2(0.5f, 0.5f);
        verticalSliderRect.anchoredPosition = new Vector2(0, 0);
        verticalSliderRect.sizeDelta = new Vector2(400, 20);
        verticalSliderRect.localEulerAngles = new Vector3(0, 0, 90);

        horizontalGreenZone = CreateGreenZone(horizontalSlider, initialHorizontalZoneSize);
        verticalGreenZone = CreateGreenZone(verticalSlider, initialVerticalZoneSize);

        ConfigureText(resultText, "", new Vector3(Screen.width / 2 - 200, Screen.height / 2 - 100, 0));
        ConfigureText(winText, "", new Vector3(Screen.width / 2 - 200, Screen.height / 2 - 100, 0));

        if (kasHelpButton != null)
        {
            kasHelpButton.onClick.AddListener(OnKasHelpButtonPress);
        }
    }

    void Update()
    {
        if (isExpanding && blackPanel != null)
        {
            expandTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(expandTimer / expandDuration);
            blackPanelTransform.localScale = Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(1, 1, 1), progress);

            if (progress >= 1.0f)
            {
                isExpanding = false;
                blackOverlayCanvas.SetActive(false);
                canvas.SetActive(true);
            }
        }

        if (isClosing && blackPanel != null)
        {
            expandTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(expandTimer / expandDuration);
            blackPanelTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0, 0, 1), progress);

            if (progress >= 1.0f)
            {
                Destroy(gameObject);
            }
        }

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
                    StartCoroutine(DisplayResultAndContinue());
                }
            }
        }
    }

    private void StartGame()
    {
        introPanel.SetActive(false);
        ball.SetActive(true);
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
    }

    private IEnumerator DisplayResultAndContinue()
{
    bool hitHorizontal = Mathf.Abs(horizontalPower - horizontalGreenZone.position.x) <= horizontalGreenZone.rect.width / 2;
    bool hitVertical = Mathf.Abs(verticalPower - verticalGreenZone.position.y) <= verticalGreenZone.rect.height / 2;

    yield return new WaitForSeconds(1f); // Espera para asegurar que el resultado se vea

    if (hitHorizontal && hitVertical)
    {
        consecutiveHits++;
        resultText.text = "Acertaste!";
        resultText.gameObject.SetActive(true);

        // Posición del texto "Acertaste"
        resultText.rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - 500, Screen.height / 2 - 250);

        // Reducir el tamaño de las zonas verdes
        initialHorizontalZoneSize *= greenZoneReductionFactor; // Reducir tamaño
        initialVerticalZoneSize *= greenZoneReductionFactor;
        UpdateGreenZoneSize(horizontalGreenZone, initialHorizontalZoneSize); // Actualizar visualmente
        UpdateGreenZoneSize(verticalGreenZone, initialVerticalZoneSize);

        // Aumentar la velocidad de los sliders
        horizontalSpeed *= speedIncreaseFactor; // Incrementar velocidad
        verticalSpeed *= speedIncreaseFactor;

        yield return new WaitForSeconds(2f);
        resultText.gameObject.SetActive(false);

        if (consecutiveHits >= 3)
        {
            winText.text = "GANASTE!";
            winText.gameObject.SetActive(true);
            finishEvent.Invoke(true, kasHelpUsed);
            StartClosingAnimation(); // Iniciar animación de cierre
        }
        else
        {
            ResetBallPosition();
            isHorizontalStopped = false;
            isVerticalStopped = false;
            isVerticalVisible = false;
        }
    }
    else
    {
        resultText.text = "Fallaste!";
        resultText.gameObject.SetActive(true);

        // Posición del texto "Fallaste"
        resultText.rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - 500, Screen.height / 2 - 250);

        yield return new WaitForSeconds(2f);
        resultText.gameObject.SetActive(false);
        finishEvent.Invoke(false, kasHelpUsed);
        StartClosingAnimation(); // Iniciar animación de cierre
    }
}


private void UpdateGreenZoneSize(RectTransform greenZone, Vector2 newSize)
{
    if (greenZone != null)
    {
        greenZone.sizeDelta = newSize;
    }
}



    private void StartClosingAnimation()
    {
        blackOverlayCanvas.SetActive(true);
        blackPanel.SetActive(true);
        canvas.SetActive(false);

        isClosing = true;
        expandTimer = 0f;
    }

    private void ResetBallPosition()
    {
        ball.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        var existingRigidbody = ball.GetComponent<Rigidbody2D>();
        if (existingRigidbody != null)
        {
            Destroy(existingRigidbody);
        }

        ball.SetActive(false);
        ball.SetActive(true);
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

    private void OnKasHelpButtonPress()
    {
        kasHelpUsed = true;
        consecutiveHits = 3;
        winText.text = "GANASTE!";
        winText.gameObject.SetActive(true);
        finishEvent.Invoke(true, kasHelpUsed);
        StartClosingAnimation();
    }
}
