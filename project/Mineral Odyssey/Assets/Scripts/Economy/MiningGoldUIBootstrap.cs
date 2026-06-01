using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiningGoldUIBootstrap : MonoBehaviour
{
    private const string CanvasName = "Mining UI";
    private const string DisplayName = "Gold Display";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureMiningGoldDisplay();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureMiningGoldDisplay();
    }

    private static void EnsureMiningGoldDisplay()
    {
        if (FindFirstObjectByType<MiningController>() == null)
        {
            return;
        }

        if (FindFirstObjectByType<GoldDisplay>() != null)
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

        TextMeshProUGUI text = displayObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 28f;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.text = "Gold: 0";

        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(16f, -16f);
        rectTransform.sizeDelta = new Vector2(240f, 48f);

        displayObject.AddComponent<GoldDisplay>();
    }
}
