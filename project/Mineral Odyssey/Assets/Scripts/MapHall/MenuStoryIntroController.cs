using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Plays the menu story intro while preserving the hand-built menu button style.
/// </summary>
public class MenuStoryIntroController : MonoBehaviour
{
    private const string MapHallSceneName = "MapHall";
    private const float MenuFadeDuration = 0.45f;
    private const float TitleDriftDuration = 1.15f;
    private const float TimeEchoDuration = 0.95f;
    private const float FinalPushDuration = 1.1f;

    private static readonly Color FinalDim = new Color(0f, 0f, 0f, 0.44f);

    private Button startButton;
    private Button storyButton;
    private Button exitButton;
    private Button backButton;
    private Button skipButton;
    private Button finalStartButton;

    private Canvas rootCanvas;
    private RectTransform titleRect;
    private TMP_Text titleText;
    private SpriteRenderer backgroundRenderer;
    private Camera menuCamera;
    private Image tintOverlay;
    private CanvasGroup storyControlsGroup;
    private CanvasGroup finalGroup;
    private CanvasGroup timeEchoGroup;
    private RectTransform clockHand;
    private TMP_Text finalStoryText;
    private TMP_Text timeEchoText;

    private CanvasGroup startGroup;
    private CanvasGroup storyGroup;
    private CanvasGroup exitGroup;
    private CanvasGroup titleGroup;

    private Coroutine storyRoutine;
    private Vector2 titleStartPosition;
    private Vector3 cameraStartPosition;
    private float cameraStartSize;
    private Color backgroundStartColor;
    private Color titleStartColor;
    private bool initialized;
    private bool isPlaying;

    public void Initialize(Button start, Button story, Button exit)
    {
        startButton = start;
        storyButton = story;
        exitButton = exit;

        ResolveSceneReferences();
        EnsureStoryView();
        CacheStartState();
        ResetToMenuState();
        initialized = true;
    }

    public void PlayStory()
    {
        if (!initialized)
        {
            Initialize(startButton, storyButton, exitButton);
        }

        if (isPlaying)
        {
            return;
        }

        if (storyRoutine != null)
        {
            StopCoroutine(storyRoutine);
        }

        storyRoutine = StartCoroutine(PlayStoryRoutine());
    }

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    private IEnumerator PlayStoryRoutine()
    {
        isPlaying = true;
        ResetToMenuState();
        SetStoryControlsVisible(true);
        SetFinalVisible(false, 0f);

        float time = 0f;
        while (time < MenuFadeDuration)
        {
            float t = Ease(time / MenuFadeDuration);
            SetMenuButtonsAlpha(1f - t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetMenuButtonsAlpha(0f);
        SetMenuButtonsInteractable(false);

        Vector2 titleEnd = titleStartPosition + new Vector2(0f, -125f);
        time = 0f;
        while (time < TitleDriftDuration)
        {
            float t = Ease(time / TitleDriftDuration);
            if (titleRect != null)
            {
                titleRect.anchoredPosition = Vector2.LerpUnclamped(titleStartPosition, titleEnd, t);
            }

            SetGroupAlpha(titleGroup, 1f - t);
            SetCameraPush(0f, 0.16f * t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetGroupAlpha(titleGroup, 0f);
        SetGroupInteractable(titleGroup, false);

        yield return PlayTimeEcho("DAY 001", 0.16f, 0.29f, 0f, -95f);
        yield return PlayTimeEcho("DAY 128", 0.29f, 0.43f, -95f, -220f);
        yield return PlayTimeEcho("DAY 365", 0.43f, 0.58f, -220f, -455f);
        yield return PlayTimeEcho("DAY 1200", 0.58f, 0.76f, -455f, -815f);

        time = 0f;
        while (time < FinalPushDuration)
        {
            float t = Ease(time / FinalPushDuration);
            tintOverlay.color = Color.Lerp(Color.clear, FinalDim, t);
            SetTimeEchoAlpha(1f - t);
            SetCameraPush(Mathf.Lerp(0.76f, 1f, t), 1f);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        ShowFinalState();
    }

    private IEnumerator PlayTimeEcho(string label, float fromPush, float toPush, float fromAngle, float toAngle)
    {
        SetTimeEcho(label, 0f, fromAngle);

        float time = 0f;
        while (time < TimeEchoDuration)
        {
            float t = Ease(time / TimeEchoDuration);
            float push = Mathf.Lerp(fromPush, toPush, t);
            SetCameraPush(push, push);
            SetTimeEchoAlpha(Mathf.Sin(Mathf.Clamp01(time / TimeEchoDuration) * Mathf.PI) * 0.34f);
            SetClockHandAngle(Mathf.Lerp(fromAngle, toAngle, t));
            time += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void ShowFinalState()
    {
        SetCameraPush(1f, 1f);
        tintOverlay.color = FinalDim;
        SetTimeEchoAlpha(0f);
        SetStoryControlsVisible(true);
        SetFinalVisible(true, 1f);
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }
    }

    private void SkipToEnding()
    {
        if (!isPlaying)
        {
            return;
        }

        if (storyRoutine != null)
        {
            StopCoroutine(storyRoutine);
            storyRoutine = null;
        }

        SetMenuButtonsAlpha(0f);
        SetMenuButtonsInteractable(false);
        SetGroupAlpha(titleGroup, 0f);
        SetGroupInteractable(titleGroup, false);
        ShowFinalState();
    }

    private void ReturnToMenu()
    {
        if (storyRoutine != null)
        {
            StopCoroutine(storyRoutine);
            storyRoutine = null;
        }

        isPlaying = false;
        ResetToMenuState();
    }

    private void OpenMapHall()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MapHallSceneName);
    }

    private void ResolveSceneReferences()
    {
        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                rootCanvas = FindFirstObjectByType<Canvas>();
            }
        }

        if (titleText == null)
        {
            GameObject titleObject = GameObject.Find("TitleText");
            titleText = titleObject != null ? titleObject.GetComponent<TMP_Text>() : null;
        }

        if (titleText != null)
        {
            titleRect = titleText.GetComponent<RectTransform>();
            titleGroup = EnsureCanvasGroup(titleText.gameObject);
        }

        if (backgroundRenderer == null)
        {
            GameObject background = GameObject.Find("menu_background");
            backgroundRenderer = background != null ? background.GetComponent<SpriteRenderer>() : null;
        }

        if (menuCamera == null)
        {
            menuCamera = Camera.main;
        }

        startGroup = EnsureCanvasGroup(startButton != null ? startButton.gameObject : null);
        storyGroup = EnsureCanvasGroup(storyButton != null ? storyButton.gameObject : null);
        exitGroup = EnsureCanvasGroup(exitButton != null ? exitButton.gameObject : null);
    }

    private void EnsureStoryView()
    {
        if (rootCanvas == null || storyControlsGroup != null)
        {
            return;
        }

        GameObject tintObject = new GameObject("StoryTintOverlay", typeof(RectTransform));
        tintObject.transform.SetParent(rootCanvas.transform, false);
        tintObject.transform.SetSiblingIndex(0);
        tintOverlay = tintObject.AddComponent<Image>();
        tintOverlay.raycastTarget = false;
        tintOverlay.color = Color.clear;
        Stretch(tintOverlay.rectTransform);

        GameObject controlsObject = new GameObject("StoryIntroControls", typeof(RectTransform));
        controlsObject.transform.SetParent(rootCanvas.transform, false);
        storyControlsGroup = controlsObject.AddComponent<CanvasGroup>();
        RectTransform controlsRect = controlsObject.GetComponent<RectTransform>();
        Stretch(controlsRect);

        backButton = CreateStyledButton("StoryBackButton", "Back", new Vector2(115f, -45f), new Vector3(1.8f, 1.8f, 1.8f), false);
        skipButton = CreateStyledButton("StorySkipButton", "Skip", new Vector2(-115f, -45f), new Vector3(1.8f, 1.8f, 1.8f), false);
        finalStartButton = CreateStyledButton("StoryFinalStartButton", "Start", new Vector2(0f, -350f), Vector3.one * 4.0628123f, true);
        SetRect(backButton.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(115f, -45f), new Vector2(160f, 30f));
        SetRect(skipButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-115f, -45f), new Vector2(160f, 30f));

        GameObject timeObject = new GameObject("StoryTimeEcho", typeof(RectTransform));
        timeObject.transform.SetParent(controlsObject.transform, false);
        timeEchoGroup = timeObject.AddComponent<CanvasGroup>();
        Stretch(timeObject.GetComponent<RectTransform>());

        timeEchoText = CreateStoryText("StoryTimeEchoText", timeObject.transform, string.Empty, 56f);
        timeEchoText.alignment = TextAlignmentOptions.Center;
        timeEchoText.enableWordWrapping = false;
        timeEchoText.color = new Color(1f, 0.92f, 0.72f, 1f);
        SetRect(timeEchoText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(520f, 72f));

        GameObject handObject = new GameObject("StoryClockHand", typeof(RectTransform));
        handObject.transform.SetParent(timeObject.transform, false);
        Image handImage = handObject.AddComponent<Image>();
        handImage.color = new Color(1f, 0.86f, 0.55f, 0.9f);
        handImage.raycastTarget = false;
        clockHand = handObject.GetComponent<RectTransform>();
        SetRect(clockHand, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 46f), new Vector2(4f, 105f));
        clockHand.pivot = new Vector2(0.5f, 0f);

        GameObject finalObject = new GameObject("StoryFinalText", typeof(RectTransform));
        finalObject.transform.SetParent(controlsObject.transform, false);
        finalGroup = finalObject.AddComponent<CanvasGroup>();
        finalStoryText = finalObject.AddComponent<TextMeshProUGUI>();
        CopyTextStyle(titleText, finalStoryText);
        finalStoryText.text = "Tired of day after day of mental labor?\n\nCome to Mineral Odyssey,\nand with every strike,\nrediscover the simple joy of digging.";
        finalStoryText.fontSize = 50f;
        finalStoryText.alignment = TextAlignmentOptions.Center;
        finalStoryText.enableWordWrapping = true;
        finalStoryText.raycastTarget = false;
        finalStoryText.color = new Color(1f, 0.93f, 0.64f, 1f);
        SetRect(finalStoryText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 22f), new Vector2(850f, 300f));

        backButton.onClick.AddListener(ReturnToMenu);
        skipButton.onClick.AddListener(SkipToEnding);
        finalStartButton.onClick.AddListener(OpenMapHall);
    }

    private Button CreateStyledButton(string objectName, string label, Vector2 anchoredPosition, Vector3 localScale, bool startsHidden)
    {
        Button template = startButton != null ? startButton : storyButton;
        GameObject buttonObject;

        if (template != null)
        {
            buttonObject = Instantiate(template.gameObject, storyControlsGroup.transform);
            buttonObject.name = objectName;
        }
        else
        {
            buttonObject = new GameObject(objectName, typeof(RectTransform));
            buttonObject.transform.SetParent(storyControlsGroup.transform, false);
            buttonObject.AddComponent<Image>().color = new Color(0.66f, 0.42f, 0.18f, 1f);
            buttonObject.AddComponent<Button>();
        }

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = buttonObject.AddComponent<RectTransform>();
        }

        SetRect(rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(160f, 30f));
        rectTransform.localScale = localScale;

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.interactable = true;

        Image image = buttonObject.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
            button.targetGraphic = image;
        }

        TMP_Text text = buttonObject.GetComponentInChildren<TMP_Text>(true);
        if (text == null)
        {
            GameObject textObject = new GameObject("Label");
            textObject.transform.SetParent(buttonObject.transform, false);
            text = textObject.AddComponent<TextMeshProUGUI>();
            Stretch(text.GetComponent<RectTransform>());
        }

        text.text = label;
        text.raycastTarget = false;
        text.alignment = TextAlignmentOptions.Center;

        buttonObject.SetActive(!startsHidden);
        return button;
    }

    private void CacheStartState()
    {
        if (titleRect != null)
        {
            titleStartPosition = titleRect.anchoredPosition;
        }

        if (menuCamera != null)
        {
            cameraStartPosition = menuCamera.transform.position;
            cameraStartSize = menuCamera.orthographicSize;
        }

        if (backgroundRenderer != null)
        {
            backgroundStartColor = backgroundRenderer.color;
        }

        if (titleText != null)
        {
            titleStartColor = titleText.color;
        }
    }

    private void ResetToMenuState()
    {
        SetMenuButtonsAlpha(1f);
        SetMenuButtonsInteractable(true);
        SetGroupAlpha(titleGroup, 1f);
        SetGroupInteractable(titleGroup, true);

        if (titleRect != null)
        {
            titleRect.anchoredPosition = titleStartPosition;
        }

        if (titleText != null)
        {
            titleText.color = titleStartColor;
        }

        if (menuCamera != null)
        {
            menuCamera.transform.position = cameraStartPosition;
            menuCamera.orthographicSize = cameraStartSize;
        }

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = backgroundStartColor;
        }

        if (tintOverlay != null)
        {
            tintOverlay.color = Color.clear;
        }

        SetTimeEchoAlpha(0f);
        SetStoryControlsVisible(false);
        SetFinalVisible(false, 0f);
    }

    private void SetCameraPush(float zoomAmount, float downwardAmount)
    {
        if (menuCamera == null)
        {
            return;
        }

        menuCamera.orthographicSize = Mathf.Lerp(cameraStartSize, 2.25f, Mathf.Clamp01(zoomAmount));
        Vector3 targetPosition = cameraStartPosition + new Vector3(0f, -1.55f * Mathf.Clamp01(downwardAmount), 0f);
        menuCamera.transform.position = targetPosition;
    }

    private void SetMenuButtonsAlpha(float alpha)
    {
        SetGroupAlpha(startGroup, alpha);
        SetGroupAlpha(storyGroup, alpha);
        SetGroupAlpha(exitGroup, alpha);
    }

    private void SetMenuButtonsInteractable(bool interactable)
    {
        SetGroupInteractable(startGroup, interactable);
        SetGroupInteractable(storyGroup, interactable);
        SetGroupInteractable(exitGroup, interactable);
    }

    private void SetStoryControlsVisible(bool visible)
    {
        if (storyControlsGroup == null)
        {
            return;
        }

        storyControlsGroup.alpha = visible ? 1f : 0f;
        storyControlsGroup.interactable = visible;
        storyControlsGroup.blocksRaycasts = visible;
        storyControlsGroup.gameObject.SetActive(visible);

        if (backButton != null)
        {
            backButton.gameObject.SetActive(visible);
        }

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(visible);
        }
    }

    private void SetFinalVisible(bool visible, float alpha)
    {
        SetGroupAlpha(finalGroup, alpha);
        SetGroupInteractable(finalGroup, visible);

        if (finalStoryText != null)
        {
            finalStoryText.gameObject.SetActive(visible);
        }

        if (finalStartButton != null)
        {
            finalStartButton.gameObject.SetActive(visible);
        }
    }

    private TMP_Text CreateStoryText(string objectName, Transform parent, string text, float fontSize)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);
        TMP_Text textComponent = textObject.AddComponent<TextMeshProUGUI>();
        CopyTextStyle(titleText, textComponent);
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.raycastTarget = false;
        return textComponent;
    }

    private void SetTimeEcho(string text, float alpha, float angle)
    {
        if (timeEchoText != null)
        {
            timeEchoText.text = text;
        }

        SetClockHandAngle(angle);
        SetTimeEchoAlpha(alpha);
    }

    private void SetTimeEchoAlpha(float alpha)
    {
        if (timeEchoGroup == null)
        {
            return;
        }

        timeEchoGroup.alpha = Mathf.Clamp01(alpha);
        timeEchoGroup.interactable = false;
        timeEchoGroup.blocksRaycasts = false;
    }

    private void SetClockHandAngle(float angle)
    {
        if (clockHand == null)
        {
            return;
        }

        clockHand.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private static CanvasGroup EnsureCanvasGroup(GameObject target)
    {
        if (target == null)
        {
            return null;
        }

        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = target.AddComponent<CanvasGroup>();
        }

        return group;
    }

    private static void SetGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group == null)
        {
            return;
        }

        group.alpha = alpha;
    }

    private static void SetGroupInteractable(CanvasGroup group, bool interactable)
    {
        if (group == null)
        {
            return;
        }

        group.interactable = interactable;
        group.blocksRaycasts = interactable;
    }

    private static void CopyTextStyle(TMP_Text source, TMP_Text target)
    {
        if (source == null || target == null)
        {
            return;
        }

        target.font = source.font;
        target.fontSharedMaterial = source.fontSharedMaterial;
        target.enableVertexGradient = source.enableVertexGradient;
        target.colorGradient = source.colorGradient;
        target.fontStyle = source.fontStyle;
    }

    private static float Ease(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
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
