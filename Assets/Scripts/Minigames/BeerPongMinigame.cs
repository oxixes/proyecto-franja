using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerPongMinigame : Minigame
{
    public GameObject canvas;
    public TMP_Text instructionText;
    public Image backgroundImage;
    public Sprite specificBackground;

    public Slider directionSlider;
    public Slider powerSlider;

    public GameObject ballPrefab;
    public GameObject cupPrefab;

    private GameObject ballInstance;
    private Rigidbody2D ballRb;

    private bool directionSelected = false;
    private bool powerSelected = false;
    private float selectedDirection;
    private float selectedPower;
    private int shotsLeft = 6;
    private int points = 0;

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
        instructionText.text = "¡Presiona ESPACIO para detener la dirección y la potencia! Acierta en los vasos para ganar.";
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

        // Crear y posicionar vasos
        float startX = -200;
        float spacing = 150;
        for (int i = 0; i < 3; i++)
        {
            GameObject cup = Instantiate(cupPrefab, canvas.transform, false);
            RectTransform cupRect = cup.GetComponent<RectTransform>();
            cupRect.anchorMin = new Vector2(0.5f, 0.5f);
            cupRect.anchorMax = new Vector2(0.5f, 0.5f);
            cupRect.pivot = new Vector2(0.5f, 0.5f);
            cupRect.anchoredPosition = new Vector2(startX + i * spacing, 350);
            cupRect.sizeDelta = new Vector2(80, 80); // Vasos más grandes

            cup.transform.SetSiblingIndex(canvas.transform.childCount - 1); // Asegurar vasos al frente

            // Asegurar que los vasos tengan el tag correcto
            if (!cup.CompareTag("Cup"))
            {
                cup.tag = "Cup";
            }
        }

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
                selectedDirection = directionSlider.value;
                directionSelected = true;
                powerSlider.gameObject.SetActive(true);
                directionSlider.gameObject.SetActive(false);
            }
        }
        // Mover y detener la barra de potencia
        else if (!powerSelected)
        {
            powerSlider.value = Mathf.PingPong(Time.time, 1.0f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectedPower = powerSlider.value;
                powerSelected = true;
                LaunchBall();
            }
        }
    }

    void LaunchBall()
    {
        if (shotsLeft <= 0) return;

        // Calcular dirección y potencia
        float angle = Mathf.Lerp(-45, 45, selectedDirection);
        float power = Mathf.Lerp(5, 15, selectedPower);

        Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * power;
        ballRb.isKinematic = false;
        ballRb.velocity = Vector2.zero; // Reiniciar velocidad
        ballRb.AddForce(force, ForceMode2D.Impulse);

        shotsLeft--;
        directionSelected = false;
        powerSelected = false;

        directionSlider.gameObject.SetActive(true);
        powerSlider.gameObject.SetActive(false);

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        // Comprobar si la bola entra en algún vaso
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(ballInstance.transform.position, 0.5f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Cup"))
            {
                points++;
                Destroy(hit.gameObject);
                break;
            }
        }

        if (points >= 3)
        {
            Debug.Log("¡Ganaste el Beer Pong!");
            gameActive = false;
            canvas.SetActive(false);
        }
        else if (shotsLeft <= 0)
        {
            Debug.Log("Perdiste. ¡Intenta de nuevo!");
            gameActive = false;
            canvas.SetActive(false);
        }
    }
}
