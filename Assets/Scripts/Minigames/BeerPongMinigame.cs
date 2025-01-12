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
    public GameObject resultPanel;
    public TMP_Text winText;
    public GameObject winPanel;
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
    private float greenZoneReductionFactor = 0.7f; //CAMBIAR A 0.6 O 0.7 SI MUY DIFÍCIL
    private float horizontalSpeed = 200f;
    private float verticalSpeed = 200f;
    private float speedIncreaseFactor = 1.4f;

    private bool kasHelpUsed = false;
    private bool won;

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

        introText.text = "Presiona ESPACIO para comenzar.\n\nInstrucciones:\n- Presiona ESPACIO para detener los sliders.\n- Alinea dirección y potencia para acertar 3 veces consecutivas y ganar.\n- Presiona K para recibir ayuda de Kas y saltarte el minijuego.";
        introText.alignment = TextAlignmentOptions.Center;
        introText.fontSize = 40;
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
        horizontalSlider.maxValue = 400;
        horizontalSlider.value = 400 / 2;
        horizontalSlider.gameObject.transform.SetSiblingIndex(1);

        var horizontalSliderRect = horizontalSlider.GetComponent<RectTransform>();
        horizontalSliderRect.anchorMin = new Vector2(0.5f, 0.1f);
        horizontalSliderRect.anchorMax = new Vector2(0.5f, 0.1f);
        horizontalSliderRect.pivot = new Vector2(0.5f, 0.5f);
        horizontalSliderRect.anchoredPosition = new Vector2(0, 0);
        horizontalSliderRect.sizeDelta = new Vector2(400, 20);

        verticalSlider.minValue = 0;
        verticalSlider.maxValue = 400;
        verticalSlider.value = 400 / 2;
        verticalSlider.gameObject.SetActive(false);
        verticalSlider.gameObject.transform.SetSiblingIndex(1);

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
                Minigame.isInMinigame = false;
                Debug.Log("Win: " + won + ", Kas Help: " + kasHelpUsed);
                finishEvent.Invoke(won, kasHelpUsed);
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnKasHelpButtonPress();
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
        rectTransform.localScale = new Vector3(1, 1, 1);

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
        float xForce = (horizontalPower / 1000) * 100f;
        float yForce = (verticalPower / 562.5585f) * 100f;

        var ballRigidbody = ball.AddComponent<Rigidbody2D>();
        ballRigidbody.gravityScale = 0;
        ballRigidbody.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
    }

    private IEnumerator DisplayResultAndContinue()
    {
        Debug.Log("Horizontal: " + horizontalPower + ", Vertical: " + verticalPower);
        Debug.Log("Horizontal Green Zone: " + horizontalGreenZone.localPosition.x + ", " + horizontalGreenZone.rect.width);
        bool hitHorizontal = Mathf.Abs(horizontalPower - 200) <= horizontalGreenZone.rect.width / 2;
        bool hitVertical = Mathf.Abs(verticalPower - 200) <= verticalGreenZone.rect.height / 2;

        yield return new WaitForSeconds(1f); // Espera para asegurar que el resultado se vea

        if (hitHorizontal && hitVertical)
        {
            consecutiveHits++;
            resultText.text = "¡Acertaste!";
            resultPanel.SetActive(true);

            // Posición del texto "Acertaste"
            resultText.rectTransform.anchoredPosition = new Vector2(0, 0);

            // Reducir el tamaño de las zonas verdes
            initialHorizontalZoneSize *= greenZoneReductionFactor; // Reducir tamaño
            initialVerticalZoneSize *= greenZoneReductionFactor;
            UpdateGreenZoneSize(horizontalGreenZone, initialHorizontalZoneSize); // Actualizar visualmente
            UpdateGreenZoneSize(verticalGreenZone, initialVerticalZoneSize);

            // Aumentar la velocidad de los sliders
            horizontalSpeed *= speedIncreaseFactor; // Incrementar velocidad
            verticalSpeed *= speedIncreaseFactor;

            yield return new WaitForSeconds(2f);
            resultPanel.SetActive(false);

            if (consecutiveHits >= 3)
            {
                winText.text = "¡GANASTE!";
                winPanel.SetActive(true);
                won = true;
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
            resultText.text = "¡Fallaste!";
            resultPanel.SetActive(true);

            // Posición del texto "Fallaste"
            resultText.rectTransform.anchoredPosition = new Vector2(0, 0);

            yield return new WaitForSeconds(2f);
            resultPanel.SetActive(false);
            won = false;
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
        ball.transform.localPosition = new Vector3(0, 0, 0);

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
        text.fontSize = 70;
        text.alignment = TextAlignmentOptions.Center;
        text.rectTransform.sizeDelta = new Vector2(400, 100);
        text.rectTransform.position = position;

        var shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        /* var background = new GameObject("Background");
        background.transform.SetParent(text.transform);
        var backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.5f);
        background.GetComponent<RectTransform>().sizeDelta = text.rectTransform.sizeDelta;
        background.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        background.transform.SetAsFirstSibling(); */

        // Get parent
        var parent = text.transform.parent;
        parent.gameObject.SetActive(false);
    }

    private void OnKasHelpButtonPress()
    {
        kasHelpUsed = true;
        consecutiveHits = 3;
        winText.text = "¡GANASTE!";
        winPanel.SetActive(true);
        won = true;
        StartClosingAnimation();
    }
}
