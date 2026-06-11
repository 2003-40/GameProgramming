using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Creates a bottom-right stamina display in mining scenes when no manual UI display exists.
/// </summary>
public class MiningStaminaUIBootstrap : MonoBehaviour
{
    private const string CanvasName = "Mining UI";
    private const string DisplayName = "Stamina Display";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureMiningStaminaDisplay();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureMiningStaminaDisplay();
    }

    private static void EnsureMiningStaminaDisplay()
    {
        // Avoid adding stamina UI to menu or map scenes.
        if (FindFirstObjectByType<MiningController>() == null)
        {
            return;
        }

        if (FindFirstObjectByType<StaminaDisplay>() != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject(CanvasName);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject displayObject = new GameObject(DisplayName);
        displayObject.transform.SetParent(canvas.transform, false);

        RectTransform displayRect = displayObject.AddComponent<RectTransform>();
        displayRect.anchorMin = new Vector2(1f, 0f);
        displayRect.anchorMax = new Vector2(1f, 0f);
        displayRect.pivot = new Vector2(1f, 0f);
        displayRect.anchoredPosition = new Vector2(-16f, 16f);
        displayRect.sizeDelta = new Vector2(240f, 64f);

        GameObject textObject = new GameObject("Stamina Text");
        textObject.transform.SetParent(displayObject.transform, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 24f;
        text.alignment = TextAlignmentOptions.BottomRight;
        text.color = Color.white;
        text.text = "Stamina: 100/100";

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = new Vector2(0f, 0.45f);
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        GameObject barBackgroundObject = new GameObject("Stamina Bar Background");
        barBackgroundObject.transform.SetParent(displayObject.transform, false);
        Image barBackground = barBackgroundObject.AddComponent<Image>();
        barBackground.color = new Color(0f, 0f, 0f, 0.55f);

        RectTransform backgroundRect = barBackground.rectTransform;
        backgroundRect.anchorMin = new Vector2(0f, 0f);
        backgroundRect.anchorMax = new Vector2(1f, 0.35f);
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        GameObject fillObject = new GameObject("Stamina Bar Fill");
        fillObject.transform.SetParent(barBackgroundObject.transform, false);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = new Color(0.3f, 0.85f, 0.35f, 1f);
        fill.fillAmount = 1f;

        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);

        displayObject.AddComponent<StaminaDisplay>().Initialize(text, fill);
    }
}
