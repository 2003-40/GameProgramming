using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiningReturnButtonBootstrap : MonoBehaviour
{
    private const string ButtonName = "Return To Map Button";
    private const string MapHallSceneName = "MapHall";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureReturnButton();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureReturnButton();
    }

    private static void EnsureReturnButton()
    {
        if (FindFirstObjectByType<MiningController>() == null)
        {
            return;
        }

        if (GameObject.Find(ButtonName) != null)
        {
            return;
        }

        EnsureEventSystem();

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Mining UI");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960f, 540f);

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject buttonObject = new GameObject(ButtonName);
        buttonObject.transform.SetParent(canvas.transform, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.48f, 0.29f, 0.14f, 0.92f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(ReturnToMapHall);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1f, 1f);
        buttonRect.anchorMax = new Vector2(1f, 1f);
        buttonRect.pivot = new Vector2(1f, 1f);
        buttonRect.anchoredPosition = new Vector2(-16f, -16f);
        buttonRect.sizeDelta = new Vector2(150f, 42f);

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = "Return Map";
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
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

    private static void ReturnToMapHall()
    {
        SceneManager.LoadScene(MapHallSceneName);
    }
}
