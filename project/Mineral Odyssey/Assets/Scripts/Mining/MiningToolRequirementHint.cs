using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows a short warning when the current mining tool is too weak for an ore.
/// </summary>
public class MiningToolRequirementHint : MonoBehaviour
{
    private const string HintObjectName = "Mining Tool Requirement Hint";
    private const float HintDuration = 2.8f;

    private TextMeshProUGUI hintText;
    private GameObject hintPanel;
    private float hideTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureHintForActiveScene();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureHintForActiveScene();
    }

    private static void EnsureHintForActiveScene()
    {
        if (FindFirstObjectByType<MiningController>() == null)
        {
            return;
        }

        if (FindFirstObjectByType<MiningToolRequirementHint>() != null)
        {
            return;
        }

        GameObject hintObject = new GameObject(HintObjectName);
        hintObject.AddComponent<MiningToolRequirementHint>();
    }

    private void Awake()
    {
        CreateHintUI();
        HideHint();
    }

    private void OnEnable()
    {
        MiningController.ToolLevelBlocked += ShowToolBlockedHint;
    }

    private void OnDisable()
    {
        MiningController.ToolLevelBlocked -= ShowToolBlockedHint;
    }

    private void Update()
    {
        if (hintPanel != null && hintPanel.activeSelf && Time.unscaledTime >= hideTime)
        {
            HideHint();
        }
    }

    private void ShowToolBlockedHint(int currentLevel, int requiredLevel, string oreName)
    {
        string oreLabel = string.IsNullOrWhiteSpace(oreName) ? "this ore" : oreName;
        SetHintText($"Tool too weak for {oreLabel}. Pickaxe level {currentLevel}/{requiredLevel} required.");

        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
        }

        hideTime = Time.unscaledTime + HintDuration;
    }

    private void SetHintText(string message)
    {
        if (hintText != null)
        {
            hintText.text = message;
        }
    }

    private void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }

    private void CreateHintUI()
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

        hintPanel = new GameObject("Tool Requirement Hint Panel");
        hintPanel.transform.SetParent(canvas.transform, false);

        Image panelImage = hintPanel.AddComponent<Image>();
        panelImage.color = new Color(0.18f, 0.04f, 0.02f, 0.78f);

        RectTransform panelRect = panelImage.rectTransform;
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 132f);
        panelRect.sizeDelta = new Vector2(920f, 52f);

        GameObject textObject = new GameObject("Tool Requirement Hint Text");
        textObject.transform.SetParent(hintPanel.transform, false);

        hintText = textObject.AddComponent<TextMeshProUGUI>();
        hintText.fontSize = 24f;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = new Color(1f, 0.86f, 0.68f, 1f);
        hintText.enableWordWrapping = false;
        hintText.raycastTarget = false;

        RectTransform textRect = hintText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 8f);
        textRect.offsetMax = new Vector2(-18f, -8f);
    }
}
