using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds the run-card prompt and selection UI at runtime when the mining scene has no manual version.
/// </summary>
public class RunCardChoiceUIBootstrap : MonoBehaviour
{
    private const string CanvasName = "Mining UI";
    private const string UiRootName = "RunCardChoiceUI";
    private const string DrawPromptPanelName = "DrawPromptPanel";
    private const string ChoicePanelName = "RunCardChoicePanel";
    private const string StatusTextName = "RuntimeStatusText";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureCardChoiceUi();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureCardChoiceUi();
    }

    private static void EnsureCardChoiceUi()
    {
        // The card UI is only relevant inside mining scenes.
        if (FindFirstObjectByType<MiningController>() == null)
        {
            return;
        }

        RunCardChoiceUI existingUi = FindFirstObjectByType<RunCardChoiceUI>();
        if (existingUi != null)
        {
            EnsureEventSystem();
            existingUi.BindSceneHierarchy();
            return;
        }

        GameObject manualUiRoot = FindSceneObject(UiRootName);
        if (manualUiRoot != null)
        {
            EnsureEventSystem();
            manualUiRoot.AddComponent<RunCardChoiceUI>().BindSceneHierarchy();
            return;
        }

        EnsureEventSystem();
        Canvas canvas = EnsureCanvas();

        GameObject uiRoot = new GameObject(UiRootName);
        uiRoot.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI statusText = CreateText(StatusTextName, uiRoot.transform, string.Empty, 20f, TextAlignmentOptions.Top, new Color(1f, 0.9f, 0.55f, 1f));
        RectTransform statusRect = statusText.rectTransform;
        statusRect.anchorMin = new Vector2(0.5f, 1f);
        statusRect.anchorMax = new Vector2(0.5f, 1f);
        statusRect.pivot = new Vector2(0.5f, 1f);
        statusRect.anchoredPosition = new Vector2(0f, -18f);
        statusRect.sizeDelta = new Vector2(420f, 34f);

        GameObject promptPanel = CreatePanel(DrawPromptPanelName, uiRoot.transform, new Color(0.02f, 0.015f, 0.01f, 0.86f));
        RectTransform promptPanelRect = promptPanel.GetComponent<RectTransform>();
        promptPanelRect.anchorMin = Vector2.zero;
        promptPanelRect.anchorMax = Vector2.one;
        promptPanelRect.offsetMin = Vector2.zero;
        promptPanelRect.offsetMax = Vector2.zero;

        GameObject promptFrame = CreatePanel("PromptFrame", promptPanel.transform, new Color(0.13f, 0.09f, 0.06f, 0.96f));
        RectTransform promptFrameRect = promptFrame.GetComponent<RectTransform>();
        promptFrameRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptFrameRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptFrameRect.pivot = new Vector2(0.5f, 0.5f);
        promptFrameRect.anchoredPosition = Vector2.zero;
        promptFrameRect.sizeDelta = new Vector2(520f, 250f);

        TextMeshProUGUI promptTitle = CreateText("PromptTitle", promptFrame.transform, "Draw a card set?", 32f, TextAlignmentOptions.Center, new Color(1f, 0.84f, 0.42f, 1f));
        RectTransform promptTitleRect = promptTitle.rectTransform;
        promptTitleRect.anchorMin = new Vector2(0f, 1f);
        promptTitleRect.anchorMax = new Vector2(1f, 1f);
        promptTitleRect.pivot = new Vector2(0.5f, 1f);
        promptTitleRect.anchoredPosition = new Vector2(0f, -24f);
        promptTitleRect.sizeDelta = new Vector2(-48f, 50f);

        TextMeshProUGUI promptDescription = CreateText("PromptDescription", promptFrame.transform, "The set may contain rewards or curses. If you draw, you must choose one card.", 20f, TextAlignmentOptions.Center, Color.white);
        RectTransform promptDescriptionRect = promptDescription.rectTransform;
        promptDescriptionRect.anchorMin = new Vector2(0f, 0.5f);
        promptDescriptionRect.anchorMax = new Vector2(1f, 0.5f);
        promptDescriptionRect.pivot = new Vector2(0.5f, 0.5f);
        promptDescriptionRect.anchoredPosition = new Vector2(0f, 10f);
        promptDescriptionRect.sizeDelta = new Vector2(-70f, 82f);

        Button drawButton = CreateButton(promptFrame.transform, "DrawButton", "Draw", new Vector2(-95f, -78f), new Vector2(150f, 46f), new Color(0.68f, 0.24f, 0.18f, 1f));
        Button skipButton = CreateButton(promptFrame.transform, "SkipButton", "Skip", new Vector2(95f, -78f), new Vector2(150f, 46f), new Color(0.24f, 0.32f, 0.55f, 1f));

        GameObject panel = CreatePanel(ChoicePanelName, uiRoot.transform, new Color(0.02f, 0.015f, 0.01f, 0.86f));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject frame = CreatePanel("ChoiceFrame", panel.transform, new Color(0.13f, 0.09f, 0.06f, 0.96f));
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.anchoredPosition = Vector2.zero;
        frameRect.sizeDelta = new Vector2(780f, 340f);

        TextMeshProUGUI title = CreateText("Title", frame.transform, "Choose a Run Card", 34f, TextAlignmentOptions.Center, new Color(1f, 0.84f, 0.42f, 1f));
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -22f);
        titleRect.sizeDelta = new Vector2(0f, 54f);

        Button[] buttons = new Button[3];
        Image[] backgroundImages = new Image[3];
        TMP_Text[] titles = new TMP_Text[3];
        TMP_Text[] descriptions = new TMP_Text[3];
        for (int i = 0; i < buttons.Length; i++)
        {
            float x = -250f + (250f * i);
            buttons[i] = CreateCardButton(frame.transform, $"CardButton{i + 1}", new Vector2(x, -62f), out backgroundImages[i], out titles[i], out descriptions[i]);
        }

        promptPanel.SetActive(false);
        panel.SetActive(false);
        uiRoot.AddComponent<RunCardChoiceUI>().Initialize(promptPanel, panel, drawButton, skipButton, buttons, backgroundImages, titles, descriptions, statusText);
    }

    private static Canvas EnsureCanvas()
    {
        // Reuse an existing canvas so generated overlays share the same screen-space setup.
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        GameObject canvasObject = new GameObject(CanvasName);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
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

    private static GameObject FindSceneObject(string objectName)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject candidate = objects[i];
            if (candidate == null || candidate.name != objectName)
            {
                continue;
            }

            Scene scene = candidate.scene;
            if (scene.IsValid() && scene.isLoaded)
            {
                return candidate;
            }
        }

        return null;
    }

    private static Button CreateCardButton(Transform parent, string objectName, Vector2 anchoredPosition, out Image backgroundImage, out TMP_Text title, out TMP_Text description)
    {
        // Cards are fixed-size so different descriptions do not shift the panel layout.
        GameObject buttonObject = CreatePanel(objectName, parent, new Color(1f, 1f, 1f, 0f));
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(220f, 210f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        GameObject backgroundObject = CreatePanel("CardBackgroundImage", buttonObject.transform, new Color(0.72f, 0.47f, 0.2f, 1f));
        backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.raycastTarget = false;
        RectTransform backgroundRect = backgroundImage.rectTransform;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        title = CreateText("CardTitle", buttonObject.transform, string.Empty, 25f, TextAlignmentOptions.Top, new Color(0.12f, 0.07f, 0.03f, 1f));
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -18f);
        titleRect.sizeDelta = new Vector2(-24f, 52f);

        description = CreateText("CardDescription", buttonObject.transform, string.Empty, 18f, TextAlignmentOptions.Center, new Color(0.12f, 0.07f, 0.03f, 1f));
        RectTransform descriptionRect = description.rectTransform;
        descriptionRect.anchorMin = new Vector2(0f, 0f);
        descriptionRect.anchorMax = new Vector2(1f, 1f);
        descriptionRect.offsetMin = new Vector2(18f, 18f);
        descriptionRect.offsetMax = new Vector2(-18f, -74f);

        return button;
    }

    private static Button CreateButton(Transform parent, string objectName, string label, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject buttonObject = CreatePanel(objectName, parent, color);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = size;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        TextMeshProUGUI text = CreateText("Text", buttonObject.transform, label, 22f, TextAlignmentOptions.Center, Color.white);
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private static GameObject CreatePanel(string objectName, Transform parent, Color color)
    {
        GameObject panel = new GameObject(objectName);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = true;
        return panel;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent, string value, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        return text;
    }
}
