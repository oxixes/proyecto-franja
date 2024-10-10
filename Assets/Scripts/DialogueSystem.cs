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

    private static DialogueSystem instance = null;
    private bool isDialogueActive = false;
    private DialogueData currentDialogueData = null;
    private DialogueLine currentDialogueLine = null;
    private int currentDialogueLineIndex = -1;
    private int currentLineCharacterCount = 0;
    private float timeSinceLastCharacter = 0;
    private float timeSinceLineFinished = 0;
    private int currentCharacterIndex = 0;
    private bool currentLineAccelerated = false;

    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI characterNameText;
    private RawImage characterImage;
    private GameObject pressToContinueText;

    public DialogueSystem()
    {
        if (instance != null)
        {
            Debug.LogWarning("Replacing instance of DialogueSystem");
        }

        instance = this;
    }

    public static DialogueSystem GetInstance()
    {
        if (instance == null) {
            Debug.LogError("You called GetInstance of DialogueSystem before it was instantiated in the scene!");
        }

        return instance;
    }

    public void StartDialogue(string dialogueId)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("A dialogue is already active, cannot start a new one.");
            return;
        }

        // Find the dialogue data with the given id
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/" + dialogueId);
        if (dialogueJSON == null)
        {
            Debug.LogError("Dialogue with id: " + dialogueId + " not found.");
            return;
        }

        currentDialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        if (currentDialogueData == null) {
            Debug.LogError("Dialogue with id: " + dialogueId + " could not be parsed.");
            return;
        }

        // Start the dialogue with the given id
        Debug.Log("Starting dialogue with id: " + dialogueId);

        isDialogueActive = true;
        gameObject.SetActive(true);

        PlayNextDialogueLine();
    }

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

        pressToContinueText.SetActive(false);
        gameObject.SetActive(false);

        Debug.Log("Dialogue system is ready!");

        StartDialogue("TestDialogue");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDialogueActive)
        {
            return;
        }

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
                if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetKeyDown(KeyCode.Space))
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

    private void PlayNextDialogueLine()
    {
        currentDialogueLineIndex++;

        if (currentDialogueLineIndex >= currentDialogueData.lines.Length)
        {
            // End of dialogue
            gameObject.SetActive(false);
            isDialogueActive = false;
            currentDialogueLineIndex = -1;
            characterImage.texture = null;
            return;
        }

        currentDialogueLine = currentDialogueData.lines[currentDialogueLineIndex];
        currentLineCharacterCount = currentDialogueLine.GetTextCharLength();

        dialogueText.text = "";
        characterNameText.text = currentDialogueLine.characterName;
        if (currentDialogueLine.imageAssetId != "")
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
        } else
        {
            characterImage.gameObject.SetActive(false);
        }
    }
}
