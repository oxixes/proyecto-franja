using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombatMinigame : Minigame
{
    [System.Serializable]
    public class CombatData
    {
        [System.Serializable]
        public class CombatLine
        {
            [System.Serializable]
            public class CombatOption
            {
                public string text;
                public string response;
                public int score;
            }

            public CombatOption[] regularOptions = { };
            public int specialOptionScore = 0;
            public string specialOptionResponse;
            public string enemyText;
        }

        public CombatLine[] lines = { };
    }

    public UnityEvent<int, int, bool> finishEvent = new UnityEvent<int, int, bool>();
    public UnityEvent skipEvent = new UnityEvent();

    public GameObject background;
    public GameObject turnsRemainingContainer;
    public GameObject juan;
    public GameObject juanDialogueBox;
    public GameObject quique;
    public GameObject quiqueDialogueBox;
    public GameObject kasHelp;
    public GameObject continueText;
    public GameObject optionsContainer;
    public GameObject[] options;
    public GameObject dialogueContainer;
    public GameObject dialoguesPanel;
    public GameObject effectAudioSourceGO;

    public AudioClip hurtSound;
    public AudioClip typingSound;

    public string dialogueDataPath = "FinalCombatData";
    public InformationData[] specialOptionsData;

    public float animationDuration = 1f;
    public float timeBetweenCharacters = 0.03f;
    [TextArea] public string startingText = "Te encuentras con Quique, ¡véncele en combate de\ninsultos!";
    [TextArea] public string badOptionText = "A Quique parece que le ha importado poco\ntu respuesta...";
    [TextArea] public string mediumOptionText = "Quique parece haberse sentido un poco\nofendido...";
    [TextArea] public string goodOptionText = "¡Quique ha quedado muy ofendido!";

    private bool isInAnimation = true;
    private bool startingAnimation = true;
    private bool gameSkipped = false;
    private float animationTimer = 0.0f;

    private int maxScore = 0;
    private int score = 0;
    private bool gameStarted = false;
    private bool lineIsFullyDisplayed = false;

    private bool inOptionsPart = true;
    private int currentCombatLine = 0;
    private int currentSelectedOption = 0;

    private RectTransform backgroundTransform;
    private TextMeshProUGUI juanDialogueText;
    private TextMeshProUGUI quiqueDialogueText;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI turnsRemainingText;
    private TextMeshProUGUI[] optionsText;

    private AudioSource effectAudioSource;

    private CombatData combatData;

    private PlayerInventoryManager inventory;

    private Animator quiqueAnimator;

    // Start is called before the first frame update
    void Start()
    {
        isInMinigame = true;

        background.SetActive(true);
        turnsRemainingContainer.SetActive(false);
        juan.SetActive(false);
        juanDialogueBox.SetActive(false);
        quique.SetActive(false);
        quiqueDialogueBox.SetActive(false);
        continueText.SetActive(false);
        optionsContainer.SetActive(false);
        dialogueContainer.SetActive(true);
        dialoguesPanel.SetActive(false);

        backgroundTransform = background.GetComponent<RectTransform>();
        backgroundTransform.localScale = new Vector3(0, 0, 0);

        juanDialogueText = juanDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        quiqueDialogueText = quiqueDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        dialogueText = dialogueContainer.GetComponentInChildren<TextMeshProUGUI>();
        turnsRemainingText = turnsRemainingContainer.GetComponentInChildren<TextMeshProUGUI>();

        inventory = GameObject.Find("Player").GetComponent<PlayerInventoryManager>();

        quiqueAnimator = quique.GetComponent<Animator>();

        optionsText = new TextMeshProUGUI[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            optionsText[i] = options[i].GetComponent<TextMeshProUGUI>();
        }

        effectAudioSource = effectAudioSourceGO.GetComponent<AudioSource>();

        combatData = JsonUtility.FromJson<CombatData>(Resources.Load<TextAsset>(dialogueDataPath).text);

        foreach (CombatData.CombatLine line in combatData.lines)
        {
            maxScore += line.specialOptionScore;
        }

        quique.GetComponent<AnimationEndNotifier>().onAnimationEnd.AddListener(DisplayQuiqueText);

        kasHelp.GetComponent<Button>().onClick.AddListener(OnKasHelpButtonPress);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInAnimation)
        {
            Mathf.Clamp(animationTimer, 0.0f, animationDuration);
            Vector3 scale = (startingAnimation) ? Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1.05f, 1.05f, 1.05f), animationTimer / animationDuration)
                : Vector3.Lerp(new Vector3(1.05f, 1.05f, 1.05f), new Vector3(0, 0, 0), animationTimer / animationDuration);
            backgroundTransform.localScale = scale;

            animationTimer += Time.deltaTime;

            if (animationTimer >= animationDuration)
            {
                backgroundTransform.localScale = (startingAnimation) ? new Vector3(1.05f, 1.05f, 1.05f) : new Vector3(0, 0, 0);
                isInAnimation = false;

                if (!startingAnimation)
                {
                    isInMinigame = false;
                    finishEvent.Invoke(maxScore, score, gameSkipped);
                    Destroy(gameObject);
                } else {
                    StartCoroutine(DisplayText(startingText, dialogueText));
                }
            }
            else
            {
                return;
            }
        }

        if (!isInMinigame) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            OnKasHelpButtonPress();
            return;
        }

        if (!gameStarted) {
            string turnsRemainingPaddedNum = (combatData.lines.Length - currentCombatLine - 1).ToString().PadLeft(2, '0');
            string turnsRemainingText = "<color=#71ed1f>PRÓXIMO TREN</color>\n<color=#f24478>" + turnsRemainingPaddedNum + " min</color>";
            this.turnsRemainingText.text = turnsRemainingText;

            turnsRemainingContainer.SetActive(true);
            dialoguesPanel.SetActive(true);
            quique.SetActive(true);
            juan.SetActive(true);

            if (lineIsFullyDisplayed && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                gameStarted = true;
                continueText.SetActive(false);
                Debug.Log("Game started");
            }

            return;
        }

        // By this point, the game has started
        CombatData.CombatLine currentLine = combatData.lines[currentCombatLine];
        if (inOptionsPart)
        {
            dialogueContainer.SetActive(false);
            optionsContainer.SetActive(true);

            if (combatData.lines.Length - currentCombatLine == 1) {
                turnsRemainingText.text = "<color=#f56b02>TREN A PUNTO DE LLEGAR</color>";
            } else {
                string turnsRemainingPaddedNum = (combatData.lines.Length - currentCombatLine - 1).ToString().PadLeft(2, '0');
                string turnsRemainingText = "<color=#71ed1f>PRÓXIMO TREN</color>\n<color=#f24478>" + turnsRemainingPaddedNum + " min</color>";
                this.turnsRemainingText.text = turnsRemainingText;
            }

            int i = 0;
            foreach (CombatData.CombatLine.CombatOption option in currentLine.regularOptions)
            {
                optionsText[i].text = (currentSelectedOption == i) ? "<color=yellow>> " + option.text + "</color>" : "  " + option.text;
                i++;
            }

            bool specialOptionAvailable = inventory.HasInformation(specialOptionsData[currentCombatLine]);
            optionsText[i].text = specialOptionAvailable ? (currentSelectedOption == i) ? "<color=yellow>> " + specialOptionsData[currentCombatLine].infoName + "</color>" : "  " + specialOptionsData[currentCombatLine].infoName : "<color=#adadad>  ???</color>";

            foreach (GameObject option in options)
            {
                option.SetActive(true);
            }

            continueText.SetActive(true);

            quiqueDialogueText.text = currentLine.enemyText;
            quiqueDialogueBox.SetActive(true);
            juanDialogueBox.SetActive(false);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentSelectedOption = (currentSelectedOption - 1 + (options.Length - (specialOptionAvailable ? 0 : 1))) % (options.Length - (specialOptionAvailable ? 0 : 1));
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentSelectedOption = (currentSelectedOption + 1) % (options.Length - (specialOptionAvailable ? 0 : 1));
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                inOptionsPart = false;
                optionsContainer.SetActive(false);
                dialogueContainer.SetActive(true);
                quiqueDialogueBox.SetActive(false);

                int scoreToAdd;
                if (currentSelectedOption < currentLine.regularOptions.Length)
                {
                    scoreToAdd = currentLine.regularOptions[currentSelectedOption].score;
                    quiqueDialogueText.text = currentLine.regularOptions[currentSelectedOption].response;
                }
                else
                {
                    scoreToAdd = currentLine.specialOptionScore;
                    quiqueDialogueText.text = currentLine.specialOptionResponse;
                }

                score += scoreToAdd;

                if (scoreToAdd <= 0) {
                    StartCoroutine(DisplayText(badOptionText, dialogueText));
                } else if (scoreToAdd < currentLine.specialOptionScore) {
                    StartCoroutine(DisplayText(mediumOptionText, dialogueText));
                } else {
                    StartCoroutine(DisplayText(goodOptionText, dialogueText));
                }

                effectAudioSource.PlayOneShot(hurtSound);
                quiqueAnimator.SetTrigger("Shake");
            }
        } else {
            if (lineIsFullyDisplayed && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                currentCombatLine++;
                inOptionsPart = true;
                continueText.SetActive(false);

                if (currentCombatLine >= combatData.lines.Length)
                {
                    animationTimer = 0.0f;
                    isInAnimation = true;
                    startingAnimation = false;

                    turnsRemainingContainer.SetActive(false);
                    juan.SetActive(false);
                    quique.SetActive(false);
                    dialoguesPanel.SetActive(false);
                }
            }
        }
    }

    IEnumerator DisplayText(string text, TextMeshProUGUI textComponent)
    {
        bool startedWithPressedKey = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);

        continueText.SetActive(false);
        lineIsFullyDisplayed = false;
        textComponent.text = "";
        foreach (char c in text)
        {
            if (!startedWithPressedKey && Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
            {
                textComponent.text = text;
                lineIsFullyDisplayed = true;
                continueText.SetActive(true);
                yield break;
            } else if (startedWithPressedKey) {
                if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Return))
                {
                    startedWithPressedKey = false;
                }
            }

            textComponent.text += c;
            effectAudioSource.PlayOneShot(typingSound);
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        lineIsFullyDisplayed = true;
        continueText.SetActive(true);
    }

    void DisplayQuiqueText()
    {
        quiqueDialogueBox.SetActive(true);
    }

    void OnKasHelpButtonPress()
    {
        gameSkipped = true;
        skipEvent.Invoke();

        animationTimer = 0.0f;
        isInAnimation = true;
        startingAnimation = false;

        turnsRemainingContainer.SetActive(false);
        juan.SetActive(false);
        quique.SetActive(false);
        dialoguesPanel.SetActive(false);
    }
}