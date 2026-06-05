using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapHallBootstrap : MonoBehaviour
{
    private const string MapHallSceneName = "MapHall";
    private const string CanvasName = "Map Hall UI";

    private static readonly Color BackgroundColor = new Color(0.11f, 0.09f, 0.07f, 1f);
    private static readonly Color PanelColor = new Color(0.18f, 0.13f, 0.09f, 0.92f);
    private static readonly Color RouteColor = new Color(0.55f, 0.34f, 0.15f, 1f);
    private static readonly Color NodeColor = new Color(0.73f, 0.50f, 0.25f, 1f);
    private static readonly Color MarkerColor = new Color(1f, 0.82f, 0.22f, 1f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureMapHall();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureMapHall();
    }

    private static void EnsureMapHall()
    {
        if (SceneManager.GetActiveScene().name != MapHallSceneName)
        {
            return;
        }

        MapHallController existingController = FindFirstObjectByType<MapHallController>();
        if (existingController != null && existingController.HasConfiguredView)
        {
            return;
        }

        EnsureEventSystem();

        GameObject canvasObject = new GameObject(CanvasName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject background = CreatePanel("Background", canvas.transform, BackgroundColor);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        TextMeshProUGUI title = CreateText("Title", canvas.transform, "Mine Map Hall", 44f, TextAlignmentOptions.Center, new Color(0.95f, 0.78f, 0.45f, 1f));
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(520f, 70f));

        TextMeshProUGUI goldText = CreateText("Gold Display", canvas.transform, "Gold: 0", 26f, TextAlignmentOptions.TopLeft, Color.white);
        SetRect(goldText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(220f, 42f));
        goldText.gameObject.AddComponent<GoldDisplay>();

        GameObject mapPanel = CreatePanel("Route Panel", canvas.transform, PanelColor);
        RectTransform mapRect = mapPanel.GetComponent<RectTransform>();
        SetRect(mapRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 8f), new Vector2(740f, 360f));

        Vector2 startPosition = new Vector2(-300f, -130f);
        Vector2 levelOnePosition = new Vector2(-135f, 120f);
        Vector2 levelTwoPosition = new Vector2(125f, -95f);
        Vector2 levelThreePosition = new Vector2(300f, 115f);

        Vector2[] routePoints =
        {
            startPosition,
            new Vector2(-300f, 65f),
            levelOnePosition,
            new Vector2(70f, 100f),
            levelTwoPosition,
            new Vector2(265f, -92f),
            levelThreePosition
        };

        for (int i = 0; i < routePoints.Length - 1; i++)
        {
            CreateRouteSegment(mapPanel.transform, routePoints[i], routePoints[i + 1]);
        }

        List<MapHallLevelOption> options = new List<MapHallLevelOption>
        {
            CreateNode(mapPanel.transform, "Start", "Start", "Choose a mine", string.Empty, 0, startPosition),
            CreateNode(mapPanel.transform, "Level 1", "Level 1", "Ticket: Free", "FirstFlour", 0, levelOnePosition),
            CreateNode(mapPanel.transform, "Level 2", "Level 2", "Ticket: 25 Gold", "SecondFlour", 25, levelTwoPosition),
            CreateNode(mapPanel.transform, "Level 3", "Level 3", "Ticket: 60 Gold", "ThirdFlour", 60, levelThreePosition)
        };

        GameObject marker = CreatePanel("Cart Marker", mapPanel.transform, MarkerColor);
        RectTransform markerRect = marker.GetComponent<RectTransform>();
        SetRect(markerRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), startPosition + new Vector2(0f, 64f), new Vector2(88f, 28f));
        TextMeshProUGUI markerLabel = CreateText("Cart Label", marker.transform, "CART", 17f, TextAlignmentOptions.Center, Color.black);
        Stretch(markerLabel.rectTransform);

        TextMeshProUGUI selectedText = CreateText("Selected Level", canvas.transform, string.Empty, 24f, TextAlignmentOptions.Center, Color.white);
        SetRect(selectedText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 92f), new Vector2(620f, 36f));

        TextMeshProUGUI messageText = CreateText("Entry Message", canvas.transform, string.Empty, 20f, TextAlignmentOptions.Center, new Color(0.95f, 0.78f, 0.45f, 1f));
        SetRect(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(700f, 32f));

        Button enterButton = CreateButton(canvas.transform, "Enter Level Button", "Enter Selected Level", new Vector2(0f, 24f), new Vector2(260f, 44f), new Color(0.66f, 0.42f, 0.18f, 1f));
        SetRect(enterButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(260f, 44f));

        MapHallController controller = canvasObject.AddComponent<MapHallController>();
        controller.Initialize(options, markerRect, enterButton, selectedText, messageText);
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private static MapHallLevelOption CreateNode(Transform parent, string objectName, string label, string costLabel, string sceneName, int ticketCost, Vector2 position)
    {
        Button button = CreateButton(parent, objectName, label, position, new Vector2(132f, 70f), NodeColor);

        TextMeshProUGUI costText = CreateText("Cost", button.transform, costLabel, 16f, TextAlignmentOptions.Center, Color.white);
        SetRect(costText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 13f), new Vector2(0f, 26f));

        RectTransform labelRect = button.GetComponentInChildren<TextMeshProUGUI>().rectTransform;
        SetRect(labelRect, new Vector2(0f, 0.35f), new Vector2(1f, 1f), new Vector2(0f, -2f), new Vector2(0f, 0f));

        return new MapHallLevelOption(label, sceneName, ticketCost, button.GetComponent<RectTransform>(), button);
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject buttonObject = CreatePanel(name, parent, color);
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        SetRect(rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, size);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        TextMeshProUGUI text = CreateText("Label", buttonObject.transform, label, 22f, TextAlignmentOptions.Center, new Color(0.12f, 0.08f, 0.04f, 1f));
        Stretch(text.rectTransform);

        return button;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = color;
        textComponent.enableWordWrapping = true;
        textComponent.raycastTarget = false;
        return textComponent;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static void CreateRouteSegment(Transform parent, Vector2 start, Vector2 end)
    {
        GameObject segment = CreatePanel("Route Segment", parent, RouteColor);
        RectTransform rectTransform = segment.GetComponent<RectTransform>();

        Vector2 delta = end - start;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = start + delta * 0.5f;
        rectTransform.sizeDelta = new Vector2(delta.magnitude, 14f);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
    }
}
