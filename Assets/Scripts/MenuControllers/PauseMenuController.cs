using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject objectsSelection;
    public GameObject infoSelection;
    public GameObject controlsSelection;
    public GameObject quitSelection;

    public GameObject objectsPanel;
    public GameObject objectsExitButton;
    public GameObject infoPanel;
    public GameObject infoExitButton;
    public GameObject controlsPanel;
    public GameObject controlsExitButton;

    public GameObject contextualHintsButton;

    [HideInInspector] public static bool isPaused = false;

    private int currentSelectionIndex = 0;

    private TextMeshProUGUI[] selectionTexts;
    private string[] originalSelectionTexts;

    private GameObject[] panels;
    private bool isPanelOpen = false;
    private int currentPanelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        selectionTexts = new TextMeshProUGUI[4];
        originalSelectionTexts = new string[4];

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                selectionTexts[i] = objectsSelection.GetComponent<TextMeshProUGUI>();
            }
            else if (i == 1)
            {
                selectionTexts[i] = infoSelection.GetComponent<TextMeshProUGUI>();
            }
            else if (i == 2)
            {
                selectionTexts[i] = controlsSelection.GetComponent<TextMeshProUGUI>();
            }
            else if (i == 3)
            {
                selectionTexts[i] = quitSelection.GetComponent<TextMeshProUGUI>();
            }

            originalSelectionTexts[i] = selectionTexts[i].text.Replace("\\n", "\n");
        }

        panels = new GameObject[3];
        panels[0] = objectsPanel;
        panels[1] = infoPanel;
        panels[2] = controlsPanel;

        pauseMenu.SetActive(false);
        objectsPanel.SetActive(false);
        infoPanel.SetActive(false);
        controlsPanel.SetActive(false);

        objectsExitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnExitButtonPressed);
        infoExitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnExitButtonPressed);
        controlsExitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnExitButtonPressed);
        contextualHintsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnContextualHintsButtonPress);

        if (SaveManager.GetInstance().Get<int>("ShowContextualHints") == 1)
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recordatorios contextuales: Activados";
        }
        else
        {
            contextualHintsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recordatorios contextuales: Desactivados";
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == currentSelectionIndex)
            {
                selectionTexts[i].text = "<color=yellow>> " + originalSelectionTexts[i].Substring(2) + "</color>";
            }
            else
            {
                selectionTexts[i].text = originalSelectionTexts[i];
            }
        }

        if (!isPaused && Input.GetKeyDown(KeyCode.Escape) && !DialogueSystem.GetInstance().IsDialogueActive())
        {
            isPaused = true;
            pauseMenu.SetActive(true);
        }
        else if (isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPanelOpen)
            {
                panels[currentPanelIndex].SetActive(false);
                isPanelOpen = false;
            } else
            {
                isPaused = false;
                pauseMenu.SetActive(false);
                currentSelectionIndex = 0;
            }
        }

        if (!isPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelectionIndex--;
            if (currentSelectionIndex < 0)
            {
                currentSelectionIndex = 3;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelectionIndex++;
            if (currentSelectionIndex > 3)
            {
                currentSelectionIndex = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            switch (currentSelectionIndex)
            {
                case 0:
                case 1:
                case 2:
                    if (!isPanelOpen)
                    {
                        panels[currentSelectionIndex].SetActive(true);
                        isPanelOpen = true;
                        currentPanelIndex = currentSelectionIndex;
                    }
                    else
                    {
                        panels[currentPanelIndex].SetActive(false);
                        isPanelOpen = false;
                    }
                    break;
                case 3:
                    isPaused = false;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Everything was already saved, so we can just quit the game
                    break;
            }
        }
    }

    private void OnExitButtonPressed()
    {
        panels[currentPanelIndex].SetActive(false);
        isPanelOpen = false;
    }

    public void OnContextualHintsButtonPress()
    {
        bool showContextualHints = SaveManager.GetInstance().Get<int>("ShowContextualHints") == 1;

        if (showContextualHints)
        {
            SaveManager.GetInstance().Set("ShowContextualHints", 0);
        }
        else
        {
            SaveManager.GetInstance().Set("ShowContextualHints", 1);
        }

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
