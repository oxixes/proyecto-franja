using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueSystem : MonoBehaviour
{
    public int textSpeed = 20; // Characters per second
    public float timeBetweenLines = 1f; // Seconds
    public float accelerationOnInput = 1.5f;

    // Allow tests to simulate pressing the space, up and down keys
    [HideInInspector] public bool spacePressed = false;
    [HideInInspector] public bool upPressed = false;
    [HideInInspector] public bool downPressed = false;

    private static DialogueSystem instance = null;
    private bool isDialogueActive = false;
    private string currentDialogueId = null;
    private DialogueData currentDialogueData = null;
    private DialogueLine currentDialogueLine = null;
    private int currentDialogueLineIndex = -1;
    private int currentLineCharacterCount = 0;
    private float timeSinceLastCharacter = 0;
    private float timeSinceLineFinished = 0;
    private int currentCharacterIndex = 0;
    private bool currentLineAccelerated = false;
    private int currentOptionIndex = 0;

    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI characterNameText;
    private RawImage characterImage;
    private GameObject pressToContinueText;

    private GameObject pressToContinueLayout;
    private GameObject characterNameLayout;
    private GameObject[] optionLayouts = new GameObject[3];
    private TextMeshProUGUI[] optionTexts = new TextMeshProUGUI[3];

    private Dictionary<string, List<Tuple<int, Action<string, string, string>>>> notificationHandlers;
    private Dictionary<int, Tuple<string, int>> notificationIdToPosition;
    private int currentNotificationId = 0;

    public DialogueSystem()
    {
        if (instance != null)
        {
            Debug.LogWarning("Replacing instance of DialogueSystem");
        }

        instance = this;
        notificationHandlers = new Dictionary<string, List<Tuple<int, Action<string, string, string>>>>();
        notificationIdToPosition = new Dictionary<int, Tuple<string, int>>();
    }

    /// <summary>
    /// Get the instance of the DialogueSystem in the scene.
    /// 
    /// This is a singleton. You mustn't instantiate a DialogueSystem yourself.
    /// </summary>
    /// <returns>The instance of the current Dialogue System.</returns>
    public static DialogueSystem GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("You called GetInstance of DialogueSystem before it was instantiated in the scene!");
        }

        return instance;
    }

    /// <summary>
    /// Register a notification handler for a given notification id.
    /// 
    /// The handler will be called when the notification with the given id is triggered
    /// by a dialogue.
    /// 
    /// The handler has three parameters:
    /// - The dialogue id
    /// - The notification id
    /// - The notification data
    /// </summary>
    /// <param name="notificationId">Notification identifier you will be subscribed to.</param>
    /// <param name="handler">The handler that will be called to be notified.</param>
    /// <returns>The handler identifier. You can use this to remove it later.</returns>
    public int HandleNotification(string notificationId, Action<string, string, string> handler)
    {
        if (!notificationHandlers.ContainsKey(notificationId))
        {
            notificationHandlers[notificationId] = new List<Tuple<int, Action<string, string, string>>> ();
        }

        int position = notificationHandlers[notificationId].Count;
        int id = this.currentNotificationId++;
        notificationHandlers[notificationId].Add(new Tuple<int, Action<string, string, string>>(position, handler));
        notificationIdToPosition[id] = new Tuple<string, int>(notificationId, position);
        return id;
    }

    /// <summary>
    /// Remove a notification handler with the given id.
    /// </summary>
    /// <param name="id">The identifier of the handler.</param>
    /// <returns>A boolean that indicates whether the handler has been removed successfully or not.</returns>
    public bool RemoveNotificationHandler(int id)
    {
        if (!notificationIdToPosition.ContainsKey(id))
        {
            Debug.LogError("Trying to remove a notification handler that does not exist");
            return false;
        }

        Tuple<string, int> notificationIdAndPosition = notificationIdToPosition[id];
        int index = -1;
        for (int i = 0; i < notificationHandlers[notificationIdAndPosition.Item1].Count; i++)
        {
            if (notificationHandlers[notificationIdAndPosition.Item1][i].Item1 == notificationIdAndPosition.Item2)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Debug.LogError("Trying to remove a notification handler that does not exist");
            return false;
        }

        notificationHandlers[notificationIdAndPosition.Item1].RemoveAt(index);
        notificationIdToPosition.Remove(id);
        
        return true;
    }

    /// <summary>
    /// Start a dialogue with the given id.
    /// 
    /// If a dialogue is already active, or if the id or file linked to it is invalid, this method will do nothing.
    /// </summary>
    /// <param name="dialogueId">The identifier of the dialogue to start.</param>
    public void StartDialogue(string dialogueId)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("A dialogue is already active, cannot start a new one.");
            return;
        }

        StartDialogueInternal(dialogueId);
    }

    private bool StartDialogueInternal(string dialogueId)
    {
        // Find the dialogue data with the given id
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/" + dialogueId);
        if (dialogueJSON == null)
        {
            Debug.LogError("Dialogue with id: " + dialogueId + " not found.");
            return false;
        }

        try
        {
            currentDialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
            currentDialogueId = dialogueId;
        } catch (System.Exception e)
        {
            Debug.LogError("Failed to parse dialogue with id: " + dialogueId + " - " + e.Message);
            return false;
        }


        // Start the dialogue with the given id
        Debug.Log("Starting dialogue with id: " + dialogueId);

        isDialogueActive = true;
        gameObject.SetActive(true);

        PlayNextDialogueLine();
        return true;
    }

    /// <summary>
    /// Check if a dialogue is currently active.
    /// </summary>
    /// <returns>A boolean that is true if a dialogue is active, or false otherwise.</returns>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        characterNameText = GameObject.Find("TalkerName").GetComponent<TextMeshProUGUI>();
        characterImage = GameObject.Find("TalkerImage").GetComponent<RawImage>();
        pressToContinueText = GameObject.Find("PressText");

        pressToContinueLayout = GameObject.Find("PressLayout");
        characterNameLayout = GameObject.Find("TalkerPanel");
        Debug.Log(characterNameLayout);
        for (int i = 1; i <= optionLayouts.Length; i++)
        {
            optionLayouts[i - 1] = GameObject.Find("Option" + i);
            optionTexts[i - 1] = GameObject.Find("Option" + i + "Text").GetComponent<TextMeshProUGUI>();

            optionLayouts[i - 1].SetActive(false);
        }

        pressToContinueText.SetActive(false);
        gameObject.SetActive(false);

        Debug.Log("Dialogue system is ready!");

        // StartDialogue("TestDialogue");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDialogueActive)
        {
            return;
        }

        if (currentDialogueLine.type.Equals("text"))
        {
            dialogueText.gameObject.SetActive(true);
            pressToContinueLayout.SetActive(true);

            bool isDialogueLineFinished = currentCharacterIndex >= currentLineCharacterCount;
            if (isDialogueLineFinished)
            {
                if (currentDialogueLine.pause)
                {
                    if (pressToContinueText.activeSelf == false)
                    {
                        pressToContinueText.SetActive(true);
                    }

                    // Wait for player input
                    if (Input.GetKeyDown(KeyCode.Space) || spacePressed)
                    {
                        timeSinceLastCharacter = 0;
                        currentCharacterIndex = 0;
                        timeSinceLineFinished = 0;
                        currentLineAccelerated = false;
                        pressToContinueText.SetActive(false);
                        PlayNextDialogueLine();
                    }
                }
                else if (timeSinceLineFinished >= timeBetweenLines)
                {
                    timeSinceLastCharacter = 0;
                    currentCharacterIndex = 0;
                    timeSinceLineFinished = 0;
                    currentLineAccelerated = false;
                    PlayNextDialogueLine();
                }

                timeSinceLineFinished += Time.deltaTime;
            }
            else
            {
                float acceleration = 1.0f;
                if (Input.GetKeyDown(KeyCode.Space) || spacePressed)
                {
                    currentLineAccelerated = true;
                }

                if (currentLineAccelerated)
                {
                    acceleration = accelerationOnInput;
                }

                timeSinceLastCharacter += acceleration * Time.deltaTime;
                if (timeSinceLastCharacter >= 1.0f / textSpeed)
                {
                    if (currentCharacterIndex < currentLineCharacterCount)
                    {
                        dialogueText.text = currentDialogueLine.GetTextUpUntil(currentCharacterIndex + 1);
                    }

                    timeSinceLastCharacter = 0;
                    currentCharacterIndex++;
                }
            }
        } 
        else if (currentDialogueLine.type.Equals("options"))
        {
            dialogueText.gameObject.SetActive(false);
            pressToContinueLayout.SetActive(false);

            for (int i = 0; i < currentDialogueLine.options.Length && i < optionLayouts.Length; i++)
            {
                if (i == currentOptionIndex)
                {
                    optionTexts[i].text = "<color=yellow>> " + currentDialogueLine.options[i].text + "</color>";
                } else
                {
                    optionTexts[i].text = "   " + currentDialogueLine.options[i].text;
                }
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || upPressed)
            {
                currentOptionIndex = (currentOptionIndex - 1 + currentDialogueLine.options.Length) % currentDialogueLine.options.Length;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || downPressed)
            {
                currentOptionIndex = (currentOptionIndex + 1) % currentDialogueLine.options.Length;
            }
            else if (Input.GetKeyDown(KeyCode.Space) || spacePressed)
            {
                currentDialogueLineIndex = -1;
                characterImage.texture = null;
                StartDialogueInternal(currentDialogueLine.options[currentOptionIndex].diversion);
            }
        }
    }

    private void PlayNextDialogueLine()
    {
        currentDialogueLineIndex++;

        if (currentDialogueLineIndex >= currentDialogueData.lines.Length ||
            !currentDialogueData.lines[currentDialogueLineIndex].CheckFormatOk())
        {
            // End of dialogue
            gameObject.SetActive(false);
            isDialogueActive = false;
            currentDialogueLineIndex = -1;
            characterImage.texture = null;
            Debug.Log("Dialogue finished");
            return;
        }

        currentOptionIndex = 0;
        currentDialogueLine = currentDialogueData.lines[currentDialogueLineIndex];

        if (currentDialogueLine.type.Equals("text") || currentDialogueLine.type.Equals("options"))
        {
            if (!string.IsNullOrEmpty(currentDialogueLine.imageAssetId))
            {
                Texture2D texture = Resources.Load<Texture2D>(currentDialogueLine.imageAssetId);
                if (texture == null)
                {
                    Debug.LogError("Image with id: " + currentDialogueLine.imageAssetId + " not found.");
                }
                else
                {
                    characterImage.texture = texture;
                }

                characterImage.gameObject.SetActive(true);
            }
            else
            {
                characterImage.gameObject.SetActive(false);
            }

            if (string.IsNullOrEmpty(currentDialogueLine.characterName))
            {
                characterNameLayout.SetActive(false);
            }
            else
            {
                characterNameText.text = currentDialogueLine.characterName;
                characterNameLayout.SetActive(true);
            }
        }

        if (currentDialogueLine.type.Equals("text"))
        {
            for (int i = 0; i < optionLayouts.Length; i++)
            {
                optionLayouts[i].SetActive(false);
            }

            currentLineCharacterCount = currentDialogueLine.GetTextCharLength();

            dialogueText.text = "";
        }
        else if (currentDialogueLine.type.Equals("options"))
        {
            for (int i = 0; i < currentDialogueLine.options.Length && i < optionLayouts.Length; i++)
            {
                optionLayouts[i].SetActive(true);
            }
        }
        else if (currentDialogueLine.type.Equals("diversion"))
        {
            currentDialogueLineIndex = -1;
            characterImage.texture = null;
            if (!StartDialogueInternal(currentDialogueLine.diversion))
            {
                Debug.LogError("Failed to divert to dialogue with id: " + currentDialogueLine.diversion);
                // End of dialogue
                gameObject.SetActive(false);
                isDialogueActive = false;
            }
        }
        else if (currentDialogueLine.type.Equals("notification"))
        {
            // Trigger the notification
            if (notificationHandlers.ContainsKey(currentDialogueLine.notificationId))
            {
                foreach (Tuple<int, Action<string, string, string>> handler in notificationHandlers[currentDialogueLine.notificationId])
                {
                    handler.Item2(currentDialogueId, currentDialogueLine.notificationId, currentDialogueLine.text);
                }
            }

            // Go to the next line
            PlayNextDialogueLine();
        }
    }
}