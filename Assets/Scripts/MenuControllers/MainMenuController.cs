using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject controlsButton;
    public GameObject exitButton;

    public GameObject controlsPanel;
    public GameObject controlsCloseButton;
    public GameObject contextualHintsButton;

    // Start is called before the first frame update
    void Start()
    {
        bool hasSavedGame = PlayerPrefs.HasKey("SavedGame");
        if (!hasSavedGame) {
            continueButton.GetComponentInChildren<MenuTextHighlightController>().isEnabled = false;
        } else {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            continueButton.GetComponent<Button>().onClick.AddListener(OnContinueButtonPress);
        }

        newGameButton.GetComponent<Button>().onClick.AddListener(OnNewGameButtonPress);
        controlsButton.GetComponent<Button>().onClick.AddListener(OnControlsButtonPress);
        controlsCloseButton.GetComponent<Button>().onClick.AddListener(OnControlsCloseButtonPress);
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitButtonPress);
        contextualHintsButton.GetComponent<Button>().onClick.AddListener(OnContextualHintsButtonPress);

        if (PlayerPrefs.GetInt("ShowContextualHints", 0) == 1)
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recordatorios contextuales: Activados";
        }
        else
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recordatorios contextuales: Desactivados";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controlsPanel.SetActive(false);
        }
    }

    void OnNewGameButtonPress()
    {
        int contextualHints = PlayerPrefs.GetInt("ShowContextualHints", 1);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("SavedGame", 0);
        PlayerPrefs.SetInt("ShowContextualHints", contextualHints);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    void OnContinueButtonPress()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    void OnExitButtonPress()
    {
        Application.Quit();
    }

    public void OnControlsButtonPress()
    {
        controlsPanel.SetActive(true);
    }

    public void OnControlsCloseButtonPress()
    {
        controlsPanel.SetActive(false);
    }

    public void OnContextualHintsButtonPress()
    {
        bool showContextualHints = PlayerPrefs.GetInt("ShowContextualHints", 0) == 1;

        if (showContextualHints)
        {
            PlayerPrefs.SetInt("ShowContextualHints", 0);
        }
        else
        {
            PlayerPrefs.SetInt("ShowContextualHints", 1);
        }

        PlayerPrefs.Save();

        if (!showContextualHints)
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "<color=yellow>Recordatorios contextuales: Activados</color>";
        }
        else
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "<color=yellow>Recordatorios contextuales: Desactivados</color>";
        }
    }
}
