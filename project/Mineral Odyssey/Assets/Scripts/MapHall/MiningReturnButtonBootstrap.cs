using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiningReturnButtonBootstrap : MonoBehaviour
{
    private const string LegacyButtonName = "Return To Map Button";
    private const string EndButtonName = "EndTurn";
    private const string RunEndPanelName = "RunEndPanel";
    private const string ReturnButtonName = "ReturnToMapButton";
    private const string GoldEarnedTextName = "GoldEarnedText";
    private const string TotalGoldTextName = "TotalGoldText";
    private const string MapHallSceneName = "MapHall";

    private static GameObject runEndPanel;
    private static Button endTurnButton;
    private static Button returnToMapButton;
    private static TMP_Text goldEarnedText;
    private static TMP_Text totalGoldText;
    private static int startingGold;
    private static bool hasRunContext;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        StaminaManager.Instance.RunEnded -= ShowRunEndPanel;
        StaminaManager.Instance.RunEnded += ShowRunEndPanel;
        BindExistingRunEndUi();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        BindExistingRunEndUi();
    }

    private static void BindExistingRunEndUi()
    {
        if (FindFirstObjectByType<MiningController>() == null)
        {
            ClearRunUiReferences();
            return;
        }

        EnsureEventSystem();
        DisableLegacyReturnButton();

        if (!hasRunContext)
        {
            startingGold = GoldManager.Instance.Gold;
            hasRunContext = true;
        }

        runEndPanel = FindSceneObject(RunEndPanelName);
        endTurnButton = FindSceneButton(EndButtonName);
        returnToMapButton = FindSceneButton(ReturnButtonName);
        goldEarnedText = FindSceneText(GoldEarnedTextName);
        totalGoldText = FindSceneText(TotalGoldTextName);

        if (runEndPanel != null)
        {
            runEndPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[MiningRunEnd] RunEndPanel was not found in the loaded mining scene. No runtime panel will be generated.");
        }

        ConfigureButton(endTurnButton, EndCurrentRun);
        ConfigureButton(returnToMapButton, ReturnToMapHall);
    }

    private static void ClearRunUiReferences()
    {
        hasRunContext = false;
        runEndPanel = null;
        endTurnButton = null;
        returnToMapButton = null;
        goldEarnedText = null;
        totalGoldText = null;
    }

    private static void ConfigureButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.interactable = true;
        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);

        Graphic targetGraphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();
        if (targetGraphic != null)
        {
            targetGraphic.raycastTarget = true;
            button.targetGraphic = targetGraphic;
        }

        TMP_Text[] labels = button.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].raycastTarget = false;
        }
    }

    private static void ShowRunEndPanel()
    {
        BindExistingRunEndUi();

        if (runEndPanel == null)
        {
            return;
        }

        UpdateRunEndText();
        runEndPanel.SetActive(true);

        if (endTurnButton != null)
        {
            endTurnButton.interactable = false;
        }

        Time.timeScale = 0f;
    }

    private static void UpdateRunEndText()
    {
        int currentGold = GoldManager.Instance.Gold;
        int earnedGold = Mathf.Max(0, currentGold - startingGold);

        if (goldEarnedText != null)
        {
            goldEarnedText.text = $"Gold earned:\n{earnedGold}";
        }

        if (totalGoldText != null)
        {
            totalGoldText.text = $"Total gold:\n{currentGold}";
        }
    }

    private static void EndCurrentRun()
    {
        StaminaManager.Instance.EndCurrentRun();
    }

    private static void ReturnToMapHall()
    {
        Time.timeScale = 1f;
        hasRunContext = false;
        SceneManager.LoadScene(MapHallSceneName);
    }

    private static void DisableLegacyReturnButton()
    {
        GameObject legacyButton = FindSceneObject(LegacyButtonName);
        if (legacyButton != null)
        {
            legacyButton.SetActive(false);
        }
    }

    private static Button FindSceneButton(string objectName)
    {
        GameObject target = FindSceneObject(objectName);
        if (target == null)
        {
            return null;
        }

        Button button = target.GetComponent<Button>();
        if (button != null)
        {
            return button;
        }

        button = target.GetComponentInChildren<Button>(true);
        if (button != null)
        {
            return button;
        }

        return target.GetComponentInParent<Button>(true);
    }

    private static TMP_Text FindSceneText(string objectName)
    {
        GameObject target = FindSceneObject(objectName);
        if (target == null)
        {
            return null;
        }

        TMP_Text text = target.GetComponent<TMP_Text>();
        if (text != null)
        {
            return text;
        }

        return target.GetComponentInChildren<TMP_Text>(true);
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

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        Debug.LogWarning("[MiningRunEnd] No EventSystem was found in the loaded mining scene. Existing UI buttons will not receive clicks until an EventSystem is present.");
    }
}
