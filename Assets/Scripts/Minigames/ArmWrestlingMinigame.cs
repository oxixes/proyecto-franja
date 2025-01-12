using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ArmWrestlingMinigame : MonoBehaviour
{
    public Slider progressBar;
    public GameObject canvas;
    public TMP_Text instructionText;
    public Image backgroundImage;
    public Sprite specificBackground;
    public GameObject introPanel;
    public TMP_Text introText;
    public Button kasHelpButton;
    public GameObject blackPanel;

    public float decrementRate = 0.2f;
    public float incrementAmount = 0.05f;
    public float winThreshold = 1.0f;
    public float loseThreshold = 0.0f;
    public float expandDuration = 2.0f;

    private bool gameActive = false;
    private RectTransform blackPanelTransform;
    private bool isExpanding = true;
    private bool isClosing = false; // Nueva variable para gestionar la animación de cierre
    private float expandTimer = 0.0f;
    private bool kasHelpUsed = false;

    public UnityEvent<bool, bool> finishEvent = new UnityEvent<bool, bool>();

    void Start()
    {
        Minigame.isInMinigame = true;

        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        if (blackPanel != null)
        {
            blackPanelTransform = blackPanel.GetComponent<RectTransform>();

            blackPanelTransform.anchorMin = new Vector2(0, 0);
            blackPanelTransform.anchorMax = new Vector2(1, 1);
            blackPanelTransform.offsetMin = Vector2.zero;
            blackPanelTransform.offsetMax = Vector2.zero;
            blackPanelTransform.pivot = new Vector2(0.5f, 0.5f);
            blackPanelTransform.localScale = new Vector3(0, 0, 1);
            blackPanel.SetActive(true);
        }

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

        if (introPanel != null && introText != null)
        {
            var panelImage = introPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0, 0, 0, 0.95f);
            }

            introText.text = "Presiona ESPACIO para comenzar.\n\nInstrucciones:\n- Presiona ESPACIO rápidamente para ganar el pulso de brazos.\n- Mantén la barra llena para ganar.\n- Presiona K para pedir ayuda a KAS y saltarte el minijuego.";
            introText.alignment = TextAlignmentOptions.Center;
            introText.fontSize = 30;
            introText.rectTransform.sizeDelta = new Vector2(800, 200);

            introPanel.SetActive(false);
        }

        if (instructionText != null)
        {
            instructionText.text = "¡Presiona ESPACIO para ganar el pulso de brazos!";
            instructionText.alignment = TextAlignmentOptions.Center;
            instructionText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.anchoredPosition = new Vector2(0, -120);
            instructionText.rectTransform.sizeDelta = new Vector2(600, 100);
            instructionText.fontSize = 24;
            instructionText.color = Color.white;
            instructionText.fontStyle = FontStyles.Bold;
            instructionText.enableWordWrapping = true;

            Shadow shadow = instructionText.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(2, -2);

            instructionText.gameObject.SetActive(false);
        }

        if (progressBar != null)
        {
            progressBar.value = 0.5f;
            RectTransform sliderTransform = progressBar.GetComponent<RectTransform>();
            sliderTransform.anchorMin = new Vector2(0.5f, 0.5f);
            sliderTransform.anchorMax = new Vector2(0.5f, 0.5f);
            sliderTransform.pivot = new Vector2(0.5f, 0.5f);
            sliderTransform.anchoredPosition = new Vector2(0, -200);
            sliderTransform.sizeDelta = new Vector2(800, 60);

            Image fillImage = progressBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.yellow;
            }

            progressBar.gameObject.SetActive(false);
        }

        if (kasHelpButton != null)
        {
            RectTransform buttonTransform = kasHelpButton.GetComponent<RectTransform>();
            buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
            buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
            buttonTransform.pivot = new Vector2(0.5f, 0.5f);
            buttonTransform.anchoredPosition = new Vector2(358, 150);
            kasHelpButton.gameObject.SetActive(false);
            
        }

        gameActive = false;
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

        if (isClosing && blackPanel != null)
        {
            expandTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(expandTimer / expandDuration);
            blackPanelTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0, 0, 1), progress);

            if (progress >= 1.0f)
            {
                Destroy(this.gameObject);
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

        if (progressBar != null && gameActive)
        {
            progressBar.value -= decrementRate * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                progressBar.value += incrementAmount;
            }

            progressBar.value = Mathf.Clamp(progressBar.value, 0.0f, 1.0f);

            if (progressBar.value >= winThreshold)
            {
                EndGame(true);
            }
            else if (progressBar.value <= loseThreshold)
            {
                EndGame(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            OnKasHelpButtonPress();
        }
    }

    private void StartGame()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
        }
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
        }
        gameActive = true;
        if (kasHelpButton != null)
        {
            kasHelpButton.gameObject.SetActive(true);
        }
    }

  void EndGame(bool won)
{
    gameActive = false;

    if (progressBar != null)
    {
        progressBar.gameObject.SetActive(false); // Ocultar el slider
    }

    if (kasHelpButton != null)
    {
        kasHelpButton.gameObject.SetActive(false); // Ocultar el botón KAS
    }

    // Mostrar el texto de resultado con fondo blanco
    if (instructionText != null)
    {
        instructionText.gameObject.SetActive(true); // Asegurarse de que está activo
        instructionText.text = won ? "¡Ganaste!" : "¡Perdiste!";
        instructionText.color = Color.black; // Texto negro
        instructionText.fontSize = 50; // Tamaño grande
        instructionText.alignment = TextAlignmentOptions.Center;

        // Crear un fondo blanco dinámicamente
        var parent = instructionText.transform.parent; // Obtener el padre del texto
        var background = parent.Find("Background"); // Buscar si ya existe un fondo llamado "Background"

        if (background == null)
        {
            // Crear un nuevo objeto de fondo
            GameObject bgObject = new GameObject("Background");
            bgObject.transform.SetParent(parent, false); // Añadirlo al mismo padre del texto

            // Añadir componente de imagen al fondo
            var bgImage = bgObject.AddComponent<Image>();
            bgImage.color = Color.white; // Fondo blanco

            // Configurar el RectTransform del fondo
            RectTransform bgTransform = bgObject.GetComponent<RectTransform>();
            RectTransform textTransform = instructionText.rectTransform;
            bgTransform.anchorMin = textTransform.anchorMin; // Copiar anclas del texto
            bgTransform.anchorMax = textTransform.anchorMax;
            bgTransform.pivot = textTransform.pivot;
            bgTransform.anchoredPosition = textTransform.anchoredPosition;
            bgTransform.sizeDelta = new Vector2(textTransform.sizeDelta.x + 50, textTransform.sizeDelta.y + 30); // Ajustar tamaño del fondo
            bgObject.transform.SetSiblingIndex(instructionText.transform.GetSiblingIndex()); // Asegurar que el fondo está detrás del texto
        }
    }

    finishEvent.Invoke(won, kasHelpUsed);

    // Iniciar la animación de cierre con un retraso
    StartCoroutine(CloseAfterDelay());
}

private IEnumerator CloseAfterDelay()
{
    yield return new WaitForSeconds(2.0f); // Mostrar el texto de resultado por 2 segundos
    StartClosingAnimation(); // Iniciar la animación de cierre
}



    private void StartClosingAnimation()
    {
        blackPanel.SetActive(true); // Activar el panel negro
        if (canvas != null)
        {
            canvas.SetActive(false); // Desactivar el canvas del minijuego
        }
        isClosing = true;
        expandTimer = 0.0f; // Reiniciar el temporizador para la animación
    }

    public void OnKasHelpButtonPress()
    {
        Debug.Log("¡Ayuda de KAS activada!");
        kasHelpUsed = true;
        EndGame(true);
    }
}
