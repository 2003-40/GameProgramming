using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles map-hall node selection, cart marker movement, ticket checks, and level loading.
/// </summary>
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
    private static readonly Vector2 MarkerOffset = new Vector2(0f, 64f);
    private static readonly string[] StartNodeNames = { "StartNode", "Start Node" };

    public bool HasConfiguredView => marker != null
        && enterButton != null
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
        DisableMarkerRaycasts();
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
        // Bind buttons once so repeated bootstrap calls do not duplicate listeners.
        if (initialized)
        {
            return;
        }

        if (enterButton == null || levelOptions == null || levelOptions.Count == 0)
        {
            return;
        }

        enterButtonText = enterButton.GetComponentInChildren<TMP_Text>();
        DisableMarkerRaycasts();

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

        MapHallLevelOption startOption = FindStartOption();
        if (startOption != null)
        {
            SelectOption(startOption);
            return;
        }

        ClearSelection();
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
            marker.anchoredPosition = option.NodeTransform.anchoredPosition + MarkerOffset;
        }

        UpdateSelectionState();
    }

    private void ClearSelection()
    {
        selectedOption = null;
        MoveMarkerToStartFallback();

        if (selectedLevelText != null)
        {
            selectedLevelText.text = "Selected: Start  |  Choose Level 1, 2, or 3";
        }

        if (enterButton != null)
        {
            enterButton.interactable = false;
        }

        if (enterButtonText != null)
        {
            enterButtonText.text = "Select a Mine Level";
        }

        SetMessage("Pick a level node on the S-route.", new Color(0.95f, 0.78f, 0.45f, 1f));
    }

    private void MoveMarkerToStartFallback()
    {
        if (marker == null)
        {
            return;
        }

        RectTransform startTransform = FindSceneRectTransform(StartNodeNames);
        if (startTransform == null)
        {
            return;
        }

        if (startTransform.parent == marker.parent)
        {
            marker.anchoredPosition = startTransform.anchoredPosition + MarkerOffset;
            return;
        }

        Vector3 startWorldPosition = startTransform.TransformPoint(startTransform.rect.center);
        Vector3 markerParentPosition = marker.parent.InverseTransformPoint(startWorldPosition);
        marker.anchoredPosition = new Vector2(markerParentPosition.x, markerParentPosition.y) + MarkerOffset;
    }

    private void EnterSelectedLevel()
    {
        // Level tickets spend saved gold before loading the selected mining scene.
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

    private MapHallLevelOption FindStartOption()
    {
        for (int i = 0; i < levelOptions.Count; i++)
        {
            MapHallLevelOption option = levelOptions[i];
            if (option != null && !option.CanEnter)
            {
                return option;
            }
        }

        return null;
    }

    private void DisableMarkerRaycasts()
    {
        // The cart marker is decorative and must not block clicks on level nodes.
        if (marker == null)
        {
            return;
        }

        Graphic[] graphics = marker.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].raycastTarget = false;
        }
    }

    private static RectTransform FindSceneRectTransform(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            GameObject target = GameObject.Find(names[i]);
            if (target == null)
            {
                continue;
            }

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform;
            }
        }

        return null;
    }
}
