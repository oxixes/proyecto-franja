using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerPongGreenBands : Minigame
{
    public GameObject canvas;
    public TMP_Text instructionText;
    public Image backgroundImage;
    public Sprite specificBackground;

    public Slider directionSlider;
    public Slider powerSlider;
    public GameObject ballPrefab;

    private GameObject ballInstance;
    private Rigidbody2D ballRb;

    private bool directionSelected = false;
    private bool powerSelected = false;
    private float selectedDirection;
    private float selectedPower;

    private bool gameActive = false;

    void Start()
    {
        // Configurar el Canvas
        canvas.SetActive(true);

        // Configurar la imagen de fondo
        if (backgroundImage != null && specificBackground != null)
        {
            backgroundImage.sprite = specificBackground;
            backgroundImage.color = Color.white;
            RectTransform bgTransform = backgroundImage.GetComponent<RectTransform>();
            bgTransform.anchorMin = new Vector2(0, 0);
            bgTransform.anchorMax = new Vector2(1, 1);
            bgTransform.offsetMin = Vector2.zero;
            bgTransform.offsetMax = Vector2.zero;
        }

        // Crear un fondo para el texto de instrucciones
        GameObject textBackground = new GameObject("TextBackground");
        textBackground.transform.SetParent(canvas.transform, false);
        RectTransform bgRect = textBackground.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = new Vector2(50, -50);
        bgRect.sizeDelta = new Vector2(300, 100);
        Image bgImage = textBackground.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.6f); // Fondo negro semitransparente

        // Configurar el texto de instrucciones
        instructionText.text = "¡Presiona ESPACIO cuando la barra esté en la banda verde para acertar!";
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.fontSize = 24;
        instructionText.color = Color.white;
        instructionText.transform.SetParent(textBackground.transform, false);
        RectTransform textRect = instructionText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1, 0.5f);
        textRect.anchorMax = new Vector2(1, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(-150, 0);
        textRect.sizeDelta = new Vector2(280, 80);

        // Añadir sombra al texto
        Shadow textShadow = instructionText.gameObject.AddComponent<Shadow>();
        textShadow.effectColor = new Color(0, 0, 0, 0.5f);
        textShadow.effectDistance = new Vector2(2, -2);

        // Configurar sliders
        RectTransform directionSliderRect = directionSlider.GetComponent<RectTransform>();
        directionSliderRect.anchorMin = new Vector2(0.5f, 0);
        directionSliderRect.anchorMax = new Vector2(0.5f, 0);
        directionSliderRect.pivot = new Vector2(0.5f, 0);
        directionSliderRect.anchoredPosition = new Vector2(0, 20);
        directionSliderRect.sizeDelta = new Vector2(400, 30);

        RectTransform powerSliderRect = powerSlider.GetComponent<RectTransform>();
        powerSliderRect.anchorMin = new Vector2(0.5f, 0);
        powerSliderRect.anchorMax = new Vector2(0.5f, 0);
        powerSliderRect.pivot = new Vector2(0.5f, 1);
        powerSliderRect.anchoredPosition = new Vector2(0, 100);
        powerSliderRect.sizeDelta = new Vector2(400, 30);
        powerSliderRect.localRotation = Quaternion.Euler(0, 0, 90);
        powerSlider.gameObject.SetActive(false);

        // Agregar bandas verdes
        AddGreenBand(directionSliderRect);
        AddGreenBand(powerSliderRect);

        // Crear la bola
        ballInstance = Instantiate(ballPrefab, canvas.transform, false);
        RectTransform ballRect = ballInstance.GetComponent<RectTransform>();
        ballRect.anchorMin = new Vector2(0.5f, 0.3f);
        ballRect.anchorMax = new Vector2(0.5f, 0.3f);
        ballRect.pivot = new Vector2(0.5f, 0);
        ballRect.anchoredPosition = new Vector2(0, 150);
        ballRect.sizeDelta = new Vector2(50, 50);

        ballInstance.transform.SetSiblingIndex(canvas.transform.childCount - 1); // Asegurar la bola al frente
        ballRb = ballInstance.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true; // Mantener la bola estática al inicio

        gameActive = true;
    }

    void Update()
    {
        if (!gameActive) return;

        // Mover y detener la barra de dirección
        if (!directionSelected)
        {
            directionSlider.value = Mathf.PingPong(Time.time, 1.0f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsWithinGreenBand(directionSlider))
                {
                    selectedDirection = directionSlider.value;
                    directionSelected = true;
                    powerSlider.gameObject.SetActive(true);
                    directionSlider.gameObject.SetActive(false);
                }
            }
        }
        // Mover y detener la barra de potencia
        else if (!powerSelected)
        {
            powerSlider.value = Mathf.PingPong(Time.time, 1.0f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsWithinGreenBand(powerSlider))
                {
                    selectedPower = powerSlider.value;
                    powerSelected = true;
                    LaunchBall();
                }
            }
        }
    }

    void LaunchBall()
    {
        // Simular el lanzamiento de la bola
        ballRb.isKinematic = false;
        ballRb.velocity = Vector2.zero;
        ballRb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        Debug.Log("¡Lanzamiento completado!");

        // Reiniciar el minijuego
        directionSelected = false;
        powerSelected = false;

        directionSlider.gameObject.SetActive(true);
        powerSlider.gameObject.SetActive(false);
    }

    void AddGreenBand(RectTransform sliderRect)
    {
        GameObject greenBand = new GameObject("GreenBand");
        greenBand.transform.SetParent(sliderRect.transform, false);
        RectTransform greenBandRect = greenBand.AddComponent<RectTransform>();
        greenBandRect.anchorMin = new Vector2(0.4f, 0);
        greenBandRect.anchorMax = new Vector2(0.6f, 1);
        greenBandRect.offsetMin = Vector2.zero;
        greenBandRect.offsetMax = Vector2.zero;
        Image greenBandImage = greenBand.AddComponent<Image>();
        greenBandImage.color = Color.green;
    }

    bool IsWithinGreenBand(Slider slider)
    {
        return slider.value >= 0.4f && slider.value <= 0.6f;
    }
}
