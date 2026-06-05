using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapHallController : MonoBehaviour
{
    [Header("Scene View")]
    [SerializeField] private List<MapHallLevelOption> levelOptions = new List<MapHallLevelOption>();
    [SerializeField] private RectTransform marker;
    [SerializeField] private Button enterButton;
    [SerializeField] private TMP_Text selectedLevelText;
    [SerializeField] private TMP_Text messageText;

    private TMP_Text enterButtonText;
    private MapHallLevelOption selectedOption;
    private bool initialized;

    public bool HasConfiguredView => marker != null
        && enterButton != null
        && selectedLevelText != null
        && messageText != null
        && levelOptions != null
        && levelOptions.Count > 0;

    private void Start()
    {
        if (!initialized && HasConfiguredView)
        {
            BindLevelOptions();
        }
    }

    public void Initialize(
        IEnumerable<MapHallLevelOption> options,
        RectTransform cartMarker,
        Button selectedLevelButton,
        TMP_Text selectedText,
        TMP_Text entryMessageText)
    {
        marker = cartMarker;
        enterButton = selectedLevelButton;
        enterButtonText = enterButton.GetComponentInChildren<TMP_Text>();
        selectedLevelText = selectedText;
        messageText = entryMessageText;

        levelOptions.Clear();
        foreach (MapHallLevelOption option in options)
        {
            levelOptions.Add(option);
        }

        BindLevelOptions();
    }

    private void BindLevelOptions()
    {
        if (initialized)
        {
            return;
        }

        if (enterButton == null || levelOptions == null || levelOptions.Count == 0)
        {
            return;
        }

        enterButtonText = enterButton.GetComponentInChildren<TMP_Text>();

        foreach (MapHallLevelOption option in levelOptions)
        {
            if (option == null || option.Button == null)
            {
                continue;
            }

            MapHallLevelOption capturedOption = option;
            capturedOption.Button.onClick.AddListener(() => SelectOption(capturedOption));
        }

        enterButton.onClick.AddListener(EnterSelectedLevel);
        GoldManager.Instance.GoldChanged += OnGoldChanged;
        initialized = true;

        MapHallLevelOption firstOption = FindFirstValidOption();
        if (firstOption != null)
        {
            SelectOption(firstOption);
        }
    }

    private void OnDestroy()
    {
        if (initialized && GoldManager.HasInstance)
        {
            GoldManager.Instance.GoldChanged -= OnGoldChanged;
        }
    }

    private void SelectOption(MapHallLevelOption option)
    {
        if (option == null)
        {
            return;
        }

        selectedOption = option;

        if (marker != null && option.NodeTransform != null)
        {
            marker.anchoredPosition = option.NodeTransform.anchoredPosition + new Vector2(0f, 64f);
        }

        UpdateSelectionState();
    }

    private void EnterSelectedLevel()
    {
        if (selectedOption == null || !selectedOption.CanEnter)
        {
            SetMessage("Select a mine level first.", new Color(0.95f, 0.78f, 0.45f, 1f));
            return;
        }

        if (!GoldManager.Instance.TrySpendGold(selectedOption.TicketCost))
        {
            int missingGold = selectedOption.TicketCost - GoldManager.Instance.Gold;
            SetMessage($"Need {missingGold} more gold for {selectedOption.DisplayName}.", new Color(1f, 0.45f, 0.32f, 1f));
            return;
        }

        SceneManager.LoadScene(selectedOption.SceneName);
    }

    private void OnGoldChanged(int currentGold)
    {
        UpdateSelectionState();
    }

    private void UpdateSelectionState()
    {
        if (selectedOption == null)
        {
            return;
        }

        if (selectedLevelText != null)
        {
            selectedLevelText.text = selectedOption.CanEnter
                ? $"Selected: {selectedOption.DisplayName}  |  Ticket: {FormatCost(selectedOption.TicketCost)}"
                : "Selected: Start  |  Choose Level 1, 2, or 3";
        }

        if (enterButton != null)
        {
            enterButton.interactable = selectedOption.CanEnter;
        }

        if (enterButtonText != null)
        {
            enterButtonText.text = selectedOption.CanEnter ? $"Enter {selectedOption.DisplayName}" : "Select a Mine Level";
        }

        if (!selectedOption.CanEnter)
        {
            SetMessage("Pick a level node on the S-route.", new Color(0.95f, 0.78f, 0.45f, 1f));
            return;
        }

        if (GoldManager.Instance.CanAfford(selectedOption.TicketCost))
        {
            SetMessage($"{selectedOption.DisplayName} is ready to enter.", new Color(0.62f, 0.95f, 0.50f, 1f));
            return;
        }

        int missingGold = selectedOption.TicketCost - GoldManager.Instance.Gold;
        SetMessage($"Not enough gold. Need {missingGold} more.", new Color(1f, 0.45f, 0.32f, 1f));
    }

    private void SetMessage(string message, Color color)
    {
        if (messageText == null)
        {
            return;
        }

        messageText.text = message;
        messageText.color = color;
    }

    private static string FormatCost(int ticketCost)
    {
        return ticketCost <= 0 ? "Free" : $"{ticketCost} Gold";
    }

    private MapHallLevelOption FindFirstValidOption()
    {
        for (int i = 0; i < levelOptions.Count; i++)
        {
            if (levelOptions[i] != null)
            {
                return levelOptions[i];
            }
        }

        return null;
    }
}
