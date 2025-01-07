using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmWrestlingMinigame : MonoBehaviour
{
    public Slider progressBar; // Barra deslizante para representar el progreso.
    public GameObject canvas; // Canvas negro para el minijuego.
    public TMP_Text instructionText; // Texto para mostrar instrucciones al usuario.
    public Image backgroundImage; // Imagen de fondo para el minijuego.
    public Sprite specificBackground; // Fondo específico a usar.
    public GameObject introPanel; // Panel para la pantalla de introducción
    public TMP_Text introText; // Texto para la pantalla de introducción

    public float decrementRate = 0.2f; // Tasa a la que disminuye la barra.
    public float incrementAmount = 0.05f; // Cantidad que aumenta la barra por cada pulsación de espacio.
    public float winThreshold = 1.0f; // Valor al que el jugador gana.
    public float loseThreshold = 0.0f; // Valor al que el jugador pierde.

    private bool gameActive = false;

    void Start()
    {
        // Configurar el Canvas
        canvas.SetActive(true); // Activar el canvas.

        // Configurar la imagen de fondo
        if (backgroundImage != null && specificBackground != null)
        {
            backgroundImage.sprite = specificBackground;
            backgroundImage.color = Color.white; // Asegurar que la imagen no esté oscurecida.

            // Configurar el RectTransform para que ocupe todo el canvas
            RectTransform bgTransform = backgroundImage.GetComponent<RectTransform>();
            bgTransform.anchorMin = new Vector2(0, 0);
            bgTransform.anchorMax = new Vector2(1, 1);
            bgTransform.offsetMin = Vector2.zero; // Left, Bottom
            bgTransform.offsetMax = Vector2.zero; // Right, Top
        }

        // Configurar el texto de introducción
        if (introPanel != null && introText != null)
        {
            var panelImage = introPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0, 0, 0, 0.95f); // Negro con 95% opacidad
            }

            introText.text = "Press SPACE to start!\n\nInstructions:\n- Press SPACE to win the arm wrestle.\n- Keep the bar full to win.";
            introText.alignment = TextAlignmentOptions.Center;
            introText.fontSize = 30;
            introText.rectTransform.sizeDelta = new Vector2(800, 200);

            introPanel.SetActive(true); // Mostrar el panel al inicio
        }

        // Configurar el texto del minijuego
        instructionText.text = "¡Presiona ESPACIO para ganar el pulso de brazos!";
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        instructionText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        instructionText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        instructionText.rectTransform.anchoredPosition = new Vector2(0, -120); // Posición justo encima del slider.
        instructionText.rectTransform.sizeDelta = new Vector2(600, 100); // Más ancho.
        instructionText.fontSize = 24; // Reducir tamaño de letra.
        instructionText.color = Color.white;
        instructionText.fontStyle = FontStyles.Bold;
        instructionText.enableWordWrapping = true;

        // Añadir sombra al texto
        Shadow shadow = instructionText.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        // Configurar el slider
        progressBar.value = 0.5f; // Iniciar la barra en el punto medio.
        RectTransform sliderTransform = progressBar.GetComponent<RectTransform>();
        sliderTransform.anchorMin = new Vector2(0.5f, 0.5f);
        sliderTransform.anchorMax = new Vector2(0.5f, 0.5f);
        sliderTransform.pivot = new Vector2(0.5f, 0.5f);
        sliderTransform.anchoredPosition = new Vector2(0, -200); // Más abajo.
        sliderTransform.sizeDelta = new Vector2(800, 60); // Más grande.

        // Cambiar color del slider para resaltar
        Image fillImage = progressBar.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = Color.yellow; // Cambiar a un color que resalte.
        }

        gameActive = false; // El juego empieza inactivo hasta que se pulse espacio
    }

    void Update()
    {
        if (!gameActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
            return;
        }

        // Disminuir el valor de la barra de progreso con el tiempo.
        progressBar.value -= decrementRate * Time.deltaTime;

        // Aumentar el valor de la barra de progreso cuando se presiona la tecla espacio.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            progressBar.value += incrementAmount;
        }

        // Asegurarse de que el valor se mantenga entre 0 y 1.
        progressBar.value = Mathf.Clamp(progressBar.value, 0.0f, 1.0f);

        // Comprobar las condiciones de victoria o derrota.
        if (progressBar.value >= winThreshold)
        {
            EndGame(true);
        }
        else if (progressBar.value <= loseThreshold)
        {
            EndGame(false);
        }
    }

    private void StartGame()
    {
        introPanel.SetActive(false); // Ocultar el panel de introducción
        gameActive = true;
    }

    void EndGame(bool won)
    {
        gameActive = false;
        canvas.SetActive(false); // Desactivar el canvas.

        if (won)
        {
            Debug.Log("¡Ganaste el pulso de brazos!");
        }
        else
        {
            Debug.Log("Perdiste el pulso de brazos.");
        }

        // Opcionalmente, dispara un evento o transita de vuelta al juego principal.
    }
}
