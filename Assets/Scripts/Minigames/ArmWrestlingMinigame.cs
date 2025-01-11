using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ArmWrestlingMinigame : MonoBehaviour
{
    public Slider progressBar; // Barra deslizante para representar el progreso.
    public GameObject canvas; // Canvas negro referenciado desde el prefab.
    public TMP_Text instructionText; // Texto para mostrar instrucciones al usuario.
    public Image backgroundImage; // Imagen de fondo para el minijuego.
    public Sprite specificBackground; // Fondo específico a usar.
    public GameObject introPanel; // Panel para la pantalla de introducción.
    public TMP_Text introText; // Texto para la pantalla de introducción.
    public Button kasHelpButton; // Botón KAS para finalizar automáticamente el juego.
    public GameObject blackPanel; // Panel negro para la animación inicial.

    public float decrementRate = 0.2f; // Tasa a la que disminuye la barra.
    public float incrementAmount = 0.05f; // Cantidad que aumenta la barra por cada pulsación de espacio.
    public float winThreshold = 1.0f; // Valor al que el jugador gana.
    public float loseThreshold = 0.0f; // Valor al que el jugador pierde.
    public float expandDuration = 2.0f; // Duración ajustada de la animación del panel negro.

    private bool gameActive = false;
    private RectTransform blackPanelTransform;
    private bool isExpanding = true;
    private float expandTimer = 0.0f;
    private bool kasHelpUsed = false; // Indica si se utilizó el botón KAS.

    public UnityEvent<bool, bool> finishEvent = new UnityEvent<bool, bool>(); // Evento para informar del resultado.


    void Start()
    {
        // Configurar el Canvas
        if (canvas != null)
        {
            canvas.SetActive(false); // Mantener el canvas desactivado inicialmente.
        }

        // Configurar el panel negro
        if (blackPanel != null)
        {
            blackPanelTransform = blackPanel.GetComponent<RectTransform>();

            // Igualar tamaño del panel negro al del canvas principal
            Canvas canvasComponent = canvas.GetComponent<Canvas>();
            Canvas blackPanelCanvas = blackPanel.GetComponentInParent<Canvas>();
            if (canvasComponent != null && blackPanelCanvas != null)
            {
                blackPanelCanvas.renderMode = canvasComponent.renderMode;
                blackPanelCanvas.sortingOrder = canvasComponent.sortingOrder - 1; // Asegurar que esté detrás del canvas principal
            }

            blackPanelTransform.anchorMin = new Vector2(0, 0);
            blackPanelTransform.anchorMax = new Vector2(1, 1);
            blackPanelTransform.offsetMin = Vector2.zero;
            blackPanelTransform.offsetMax = Vector2.zero;
            blackPanelTransform.pivot = new Vector2(0.5f, 0.5f); // Centrar el pivot para que escale desde el centro.
            blackPanelTransform.localScale = new Vector3(0, 0, 1); // Comienza desde un tamaño muy pequeño.
            blackPanel.SetActive(true); // Asegurar que el panel negro está activo.
        }

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

            introText.text = "Presiona ESPACIO para comenzar!\n\nInstrucciones:\n- Presiona ESPACIO rápidamente para ganar el pulso de brazos.\n- Mantén la barra llena para ganar.\n- Presiona K para pedir ayuda a KAS y saltarte el minijuego.";
            introText.alignment = TextAlignmentOptions.Center;
            introText.fontSize = 30;
            introText.rectTransform.sizeDelta = new Vector2(800, 200);

            introPanel.SetActive(false); // Mantener el panel de introducción oculto inicialmente.
        }

        // Configurar el texto del minijuego
        if (instructionText != null)
        {
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

            instructionText.gameObject.SetActive(false); // Ocultar texto del minijuego inicialmente.
        }

        // Configurar el slider
        if (progressBar != null)
        {
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

            progressBar.gameObject.SetActive(false); // Ocultar el slider inicialmente.
        }

        // Configurar la posición del botón KAS
        if (kasHelpButton != null)
        {
            RectTransform buttonTransform = kasHelpButton.GetComponent<RectTransform>();
            buttonTransform.anchorMin = new Vector2(0.5f, 0.5f); // Centrar según la referencia del script
            buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
            buttonTransform.pivot = new Vector2(0.5f, 0.5f);
            buttonTransform.anchoredPosition = new Vector2(358, 150); // Posición ajustada ligeramente más abajo
        }

        gameActive = false; // El juego empieza inactivo hasta que se pulse espacio
    }

    void Update()
    {
        if (isExpanding && blackPanel != null)
        {
            // Incrementa el temporizador.
            expandTimer += Time.deltaTime;

            // Calcula el progreso de la expansión.
            float progress = Mathf.Clamp01(expandTimer / expandDuration);

            // Escala el panel negro gradualmente hasta que cubra toda la pantalla.
            blackPanelTransform.localScale = Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(1, 1, 1), progress);

            // Comprueba si la animación ha terminado.
            if (progress >= 1.0f)
            {
                isExpanding = false; // Detiene la animación.

                // Desactiva el panel negro y activa el canvas del minijuego y el panel de introducción.
                blackPanel.SetActive(false);
                if (canvas != null)
                {
                    canvas.SetActive(true);
                }
                if (introPanel != null)
                {
                    introPanel.SetActive(true);
                }
            }
        }

        if (!gameActive && !isExpanding)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
            return;
        }

        // Disminuir el valor de la barra de progreso con el tiempo.
        if (progressBar != null)
        {
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

        // Activar el botón KAS con la tecla K
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnKasHelpButtonPress();
        }
    }

    private void StartGame()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false); // Ocultar el panel de introducción
        }
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true); // Mostrar el texto del minijuego
        }
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true); // Mostrar el slider del minijuego
        }
        gameActive = true;
        if (kasHelpButton != null)
        {
            kasHelpButton.gameObject.SetActive(true); // Mostrar el botón KAS cuando empieza el juego
        }
    }

    void EndGame(bool won)
    {
        gameActive = false;

        if (canvas != null)
        {
            canvas.SetActive(false); // Desactivar el canvas.
        }

        if (kasHelpButton != null)
        {
            kasHelpButton.gameObject.SetActive(false); // Ocultar el botón KAS al finalizar
        }

        // Invocar el evento con los resultados
        finishEvent.Invoke(won, kasHelpUsed);

        if (won)
        {
            Debug.Log("¡Ganaste el pulso de brazos!");
        }
        else
        {
            Debug.Log("Perdiste el pulso de brazos.");
        }

        Destroy(this.gameObject); // Destruir el prefab completo en ambos casos.
    }

    public void OnKasHelpButtonPress()
    {
        Debug.Log("¡Ayuda de KAS activada!");
        kasHelpUsed = true; // Marcar que se utilizó el botón KAS.
        EndGame(true); // Finalizar el juego automáticamente como victoria
    }
}
