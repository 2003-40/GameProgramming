using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunCardChoiceUI : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject drawPromptPanel;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button skipButton;

    [Header("Choice")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] cardButtons;
    [SerializeField] private Image[] cardBackgroundImages;
    [SerializeField] private TMP_Text[] cardTitles;
    [SerializeField] private TMP_Text[] cardDescriptions;

    [Header("Display")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Sprite goodCardSprite;
    [SerializeField] private Sprite badCardSprite;

    private bool restoreTimeScaleAfterPrompt;
    private bool restoreTimeScaleAfterChoice;

    private void Awake()
    {
        BindSceneHierarchy();
        ConfigureButtons();
        HideDrawPrompt(false);
        HideChoicePanel(false);
    }

    private void OnEnable()
    {
        BindSceneHierarchy();
        ConfigureButtons();
        Subscribe();
        RefreshStatus();
    }

    private void OnDisable()
    {
        if (RunCardManager.HasInstance)
        {
            RunCardManager.Instance.DrawOfferBecameAvailable -= ShowDrawPrompt;
            RunCardManager.Instance.CardChoiceBecameAvailable -= ShowChoicePanel;
            RunCardManager.Instance.CardSelected -= HandleCardSelected;
            RunCardManager.Instance.RunStateChanged -= HandleRunStateChanged;
        }
    }

    private void Update()
    {
        RefreshStatus();
    }

    public void Initialize(
        GameObject promptPanel,
        GameObject panel,
        Button drawCardButton,
        Button skipCardButton,
        Button[] buttons,
        Image[] backgroundImages,
        TMP_Text[] titles,
        TMP_Text[] descriptions,
        TMP_Text selectedStatusText)
    {
        drawPromptPanel = promptPanel;
        choicePanel = panel;
        drawButton = drawCardButton;
        skipButton = skipCardButton;
        cardButtons = buttons;
        cardBackgroundImages = backgroundImages;
        cardTitles = titles;
        cardDescriptions = descriptions;
        statusText = selectedStatusText;

        ConfigureButtons();
        Subscribe();
        HideDrawPrompt(false);
        HideChoicePanel(false);
        RefreshStatus();
    }

    public void BindSceneHierarchy()
    {
        if (statusText == null)
        {
            statusText = FindChildComponent<TMP_Text>("RuntimeStatusText");
        }

        if (statusText == null)
        {
            statusText = FindChildComponent<TMP_Text>("RunCardStatusText");
        }

        if (drawPromptPanel == null)
        {
            drawPromptPanel = FindChildGameObject("DrawPromptPanel");
        }

        if (choicePanel == null)
        {
            choicePanel = FindChildGameObject("RunCardChoicePanel");
        }

        if (drawButton == null && drawPromptPanel != null)
        {
            drawButton = FindChildComponent<Button>(drawPromptPanel.transform, "DrawButton");
        }

        if (skipButton == null && drawPromptPanel != null)
        {
            skipButton = FindChildComponent<Button>(drawPromptPanel.transform, "SkipButton");
        }

        EnsureCardArrays();
        for (int i = 0; i < cardButtons.Length; i++)
        {
            string cardName = $"CardButton{i + 1}";
            if (cardButtons[i] == null)
            {
                cardButtons[i] = FindChildComponent<Button>(cardName);
            }

            Transform cardTransform = cardButtons[i] != null ? cardButtons[i].transform : FindChildTransform(cardName);
            if (cardTransform == null)
            {
                continue;
            }

            if (cardBackgroundImages[i] == null)
            {
                cardBackgroundImages[i] = FindChildComponent<Image>(cardTransform, "CardBackgroundImage");
            }

            if (cardTitles[i] == null)
            {
                cardTitles[i] = FindChildComponent<TMP_Text>(cardTransform, "CardTitle");
            }

            if (cardDescriptions[i] == null)
            {
                cardDescriptions[i] = FindChildComponent<TMP_Text>(cardTransform, "CardDescription");
            }

            if (cardBackgroundImages[i] != null)
            {
                cardBackgroundImages[i].raycastTarget = false;
            }
        }
    }

    private void Subscribe()
    {
        RunCardManager manager = RunCardManager.Instance;
        manager.DrawOfferBecameAvailable -= ShowDrawPrompt;
        manager.DrawOfferBecameAvailable += ShowDrawPrompt;
        manager.CardChoiceBecameAvailable -= ShowChoicePanel;
        manager.CardChoiceBecameAvailable += ShowChoicePanel;
        manager.CardSelected -= HandleCardSelected;
        manager.CardSelected += HandleCardSelected;
        manager.RunStateChanged -= HandleRunStateChanged;
        manager.RunStateChanged += HandleRunStateChanged;
    }

    private void ConfigureButtons()
    {
        if (drawButton != null)
        {
            drawButton.onClick.RemoveListener(HandleDrawClicked);
            drawButton.onClick.AddListener(HandleDrawClicked);
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(HandleSkipClicked);
            skipButton.onClick.AddListener(HandleSkipClicked);
        }

        if (cardButtons == null)
        {
            return;
        }

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int cardIndex = i;
            Button button = cardButtons[i];
            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RunCardManager.Instance.SelectCard(cardIndex));
        }
    }

    private void ConfigureCards()
    {
        EnsureCardArrays();

        RunCardManager manager = RunCardManager.Instance;
        RunCardData[] cards = manager.CurrentChoiceCards;
        Sprite cardSprite = manager.IsCurrentChoiceBad ? badCardSprite : goodCardSprite;

        for (int i = 0; i < cardButtons.Length; i++)
        {
            bool hasCard = cards != null && i < cards.Length && cards[i] != null;

            if (cardButtons[i] != null)
            {
                cardButtons[i].interactable = hasCard;
            }

            if (!hasCard)
            {
                SetCardText(i, string.Empty, string.Empty);
                continue;
            }

            SetCardText(i, cards[i].DisplayName, cards[i].Description);

            if (cardBackgroundImages[i] != null && cardSprite != null)
            {
                cardBackgroundImages[i].sprite = cardSprite;
            }
        }
    }

    private void ShowDrawPrompt()
    {
        if (drawPromptPanel != null)
        {
            drawPromptPanel.SetActive(true);
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        restoreTimeScaleAfterPrompt = Time.timeScale > 0f;
        restoreTimeScaleAfterChoice = restoreTimeScaleAfterPrompt;
        Time.timeScale = 0f;
        RefreshStatus();
    }

    private void ShowChoicePanel()
    {
        ConfigureCards();
        HideDrawPrompt(false);
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }

        if (!restoreTimeScaleAfterChoice)
        {
            restoreTimeScaleAfterChoice = Time.timeScale > 0f;
        }

        Time.timeScale = 0f;
        RefreshStatus();
    }

    private void HandleCardSelected(RunCardData card)
    {
        HideChoicePanel(restoreTimeScaleAfterChoice);
        restoreTimeScaleAfterChoice = false;
        RefreshStatus();
    }

    private void HandleRunStateChanged()
    {
        RunCardManager manager = RunCardManager.Instance;
        if (!manager.IsDrawOfferAvailable && drawPromptPanel != null)
        {
            drawPromptPanel.SetActive(false);
        }

        if (!manager.IsChoiceAvailable && choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        RefreshStatus();
    }

    private void HandleDrawClicked()
    {
        RunCardManager.Instance.AcceptDrawOffer();
    }

    private void HandleSkipClicked()
    {
        RunCardManager.Instance.SkipDrawOffer();
        HideDrawPrompt(restoreTimeScaleAfterPrompt);
        restoreTimeScaleAfterPrompt = false;
        restoreTimeScaleAfterChoice = false;
        RefreshStatus();
    }

    private void HideDrawPrompt(bool restoreTimeScale)
    {
        if (drawPromptPanel != null)
        {
            drawPromptPanel.SetActive(false);
        }

        if (restoreTimeScale)
        {
            Time.timeScale = 1f;
        }
    }

    private void HideChoicePanel(bool restoreTimeScale)
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        if (restoreTimeScale)
        {
            Time.timeScale = 1f;
        }
    }

    private void RefreshStatus()
    {
        if (statusText == null || !RunCardManager.HasInstance)
        {
            return;
        }

        RunCardManager manager = RunCardManager.Instance;
        if (!manager.IsMiningRun)
        {
            statusText.text = string.Empty;
            return;
        }

        if (manager.HasSelectedCard)
        {
            statusText.text = $"Run Card: {manager.SelectedCardName}";
            return;
        }

        if (manager.IsCardOpportunityResolved)
        {
            statusText.text = "Run Card: skipped";
            return;
        }

        if (manager.IsChoiceAvailable)
        {
            statusText.text = manager.IsCurrentChoiceBad ? "Choose a curse card" : "Choose a run card";
            return;
        }

        if (manager.IsDrawOfferAvailable)
        {
            statusText.text = "Card draw available";
            return;
        }

        if (manager.HasStartedCardTimer)
        {
            statusText.text = $"Card Timer: {manager.ActiveMiningSeconds:0.0}/{manager.RequiredSeconds:0}s";
            return;
        }

        statusText.text = "Card Timer: swing or collect ore to start";
    }

    private void EnsureCardArrays()
    {
        if (cardButtons == null || cardButtons.Length != 3)
        {
            cardButtons = ResizeArray(cardButtons, 3);
        }

        if (cardBackgroundImages == null || cardBackgroundImages.Length != 3)
        {
            cardBackgroundImages = ResizeArray(cardBackgroundImages, 3);
        }

        if (cardTitles == null || cardTitles.Length != 3)
        {
            cardTitles = ResizeArray(cardTitles, 3);
        }

        if (cardDescriptions == null || cardDescriptions.Length != 3)
        {
            cardDescriptions = ResizeArray(cardDescriptions, 3);
        }
    }

    private void SetCardText(int index, string title, string description)
    {
        if (cardTitles != null && index < cardTitles.Length && cardTitles[index] != null)
        {
            cardTitles[index].text = title;
        }

        if (cardDescriptions != null && index < cardDescriptions.Length && cardDescriptions[index] != null)
        {
            cardDescriptions[index].text = description;
        }
    }

    private GameObject FindChildGameObject(string objectName)
    {
        Transform child = FindChildTransform(objectName);
        return child != null ? child.gameObject : null;
    }

    private T FindChildComponent<T>(string objectName) where T : Component
    {
        Transform child = FindChildTransform(objectName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private static T FindChildComponent<T>(Transform root, string objectName) where T : Component
    {
        Transform child = FindChildTransform(root, objectName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindChildTransform(string objectName)
    {
        return FindChildTransform(transform, objectName);
    }

    private static Transform FindChildTransform(Transform root, string objectName)
    {
        if (root == null)
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
            {
                return children[i];
            }
        }

        return null;
    }

    private static T[] ResizeArray<T>(T[] source, int size)
    {
        T[] resized = new T[size];
        if (source == null)
        {
            return resized;
        }

        int copyLength = Mathf.Min(source.Length, size);
        for (int i = 0; i < copyLength; i++)
        {
            resized[i] = source[i];
        }

        return resized;
    }
}
