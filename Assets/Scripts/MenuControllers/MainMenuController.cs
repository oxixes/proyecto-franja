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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("SavedGame", 0);
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
}
