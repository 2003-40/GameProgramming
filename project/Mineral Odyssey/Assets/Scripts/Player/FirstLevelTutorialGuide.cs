using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows the first-run Level 1 control guide and advances it through movement and mining.
/// </summary>
public class FirstLevelTutorialGuide : MonoBehaviour
{
    private const string LevelOneSceneName = "FirstFlour";
    private const string TutorialCompleteKey = "FirstLevelTutorialComplete";
    private const string GuideObjectName = "First Level Tutorial Guide";
    private const float FinalMessageDuration = 6f;

    private enum TutorialStep
    {
        Move,
        Mine,
        CombatPreview,
        Complete
    }

    private TextMeshProUGUI guideText;
    private GameObject guidePanel;
    private TutorialStep currentStep = TutorialStep.Move;
    private float finalMessageHideTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureGuideForActiveScene();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureGuideForActiveScene();
    }

    private static void EnsureGuideForActiveScene()
    {
        if (SceneManager.GetActiveScene().name != LevelOneSceneName)
        {
            return;
        }

        if (PlayerPrefs.GetInt(TutorialCompleteKey, 0) == 1)
        {
            return;
        }

        if (FindFirstObjectByType<FirstLevelTutorialGuide>() != null)
        {
            return;
        }

        GameObject guideObject = new GameObject(GuideObjectName);
        guideObject.AddComponent<FirstLevelTutorialGuide>();
    }

    private void Awake()
    {
        CreateGuideUI();
        ShowMovePrompt();
    }

    private void OnEnable()
    {
        MiningController.OreHit += HandleOreHit;
    }

    private void OnDisable()
    {
        MiningController.OreHit -= HandleOreHit;
    }

    private void Update()
    {
        if (currentStep == TutorialStep.Move && WasdPressed())
        {
            ShowMinePrompt();
            return;
        }

        if (currentStep == TutorialStep.CombatPreview && Time.unscaledTime >= finalMessageHideTime)
        {
            CompleteTutorial();
        }
    }

    private void HandleOreHit()
    {
        if (currentStep != TutorialStep.Mine)
        {
            return;
        }

        ShowCombatPreviewPrompt();
    }

    private static bool WasdPressed()
    {
        return Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D);
    }

    private void ShowMovePrompt()
    {
        currentStep = TutorialStep.Move;
        SetGuideText("Use WASD to move");
    }

    private void ShowMinePrompt()
    {
        currentStep = TutorialStep.Mine;
        SetGuideText("Left-click to mine ore");
    }

    private void ShowCombatPreviewPrompt()
    {
        currentStep = TutorialStep.CombatPreview;
        finalMessageHideTime = Time.unscaledTime + FinalMessageDuration;
        SetGuideText("In the next two levels, right-click to attack monsters. This level has no monsters.");
    }

    private void CompleteTutorial()
    {
        currentStep = TutorialStep.Complete;
        PlayerPrefs.SetInt(TutorialCompleteKey, 1);
        PlayerPrefs.Save();

        if (guidePanel != null)
        {
            Destroy(guidePanel);
        }

        Destroy(gameObject);
    }

    private void SetGuideText(string message)
    {
        if (guideText != null)
        {
            guideText.text = message;
        }
    }

    private void CreateGuideUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Mining UI");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject panelObject = new GameObject("Tutorial Hint Panel");
        panelObject.transform.SetParent(canvas.transform, false);
        guidePanel = panelObject;

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.62f);

        RectTransform panelRect = panelImage.rectTransform;
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 132f);
        panelRect.sizeDelta = new Vector2(940f, 54f);

        GameObject textObject = new GameObject("Tutorial Hint Text");
        textObject.transform.SetParent(panelObject.transform, false);

        guideText = textObject.AddComponent<TextMeshProUGUI>();
        guideText.fontSize = 26f;
        guideText.alignment = TextAlignmentOptions.Center;
        guideText.color = Color.white;
        guideText.enableWordWrapping = false;
        guideText.raycastTarget = false;

        RectTransform textRect = guideText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 8f);
        textRect.offsetMax = new Vector2(-18f, -8f);
    }
}
