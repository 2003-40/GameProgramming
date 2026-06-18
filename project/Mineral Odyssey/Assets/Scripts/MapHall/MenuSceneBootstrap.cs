using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Wires the main menu buttons to the map hall and quit actions after scene load.
/// </summary>
public class MenuSceneBootstrap : MonoBehaviour
{
    private const string MenuSceneName = "_Menu";
    private const string MapHallSceneName = "MapHall";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        WireMenuButtons();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireMenuButtons();
    }

    private static void WireMenuButtons()
    {
        // Name alternatives make the bootstrap tolerant of small UI naming changes.
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            return;
        }

        Button startButton = FindButton("Start_Button", "StartButton", "Start", "PlayButton");
        if (startButton != null)
        {
            ConfigureButton(startButton);
            startButton.onClick.RemoveListener(OpenMapHall);
            startButton.onClick.AddListener(OpenMapHall);
        }

        Button storyButton = FindButton("Story_Button", "StoryButton", "Story");
        if (storyButton != null)
        {
            ConfigureButton(storyButton);
        }

        Button exitButton = FindButton("Exit_Button", "ExitButton", "Exit", "QuitButton");
        if (exitButton != null)
        {
            ConfigureButton(exitButton);
            exitButton.onClick.RemoveListener(QuitGame);
            exitButton.onClick.AddListener(QuitGame);
        }

        MenuStoryIntroController storyController = EnsureStoryController();
        if (storyButton != null && storyController != null)
        {
            storyController.Initialize(startButton, storyButton, exitButton);
            storyButton.onClick.RemoveListener(storyController.PlayStory);
            storyButton.onClick.AddListener(storyController.PlayStory);
        }
    }

    private static void ConfigureButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        button.interactable = true;

        Graphic graphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
            button.targetGraphic = graphic;
        }
    }

    private static MenuStoryIntroController EnsureStoryController()
    {
        MenuStoryIntroController controller = FindFirstObjectByType<MenuStoryIntroController>();
        if (controller != null)
        {
            return controller;
        }

        GameObject menuUi = GameObject.Find("MenuUI");
        if (menuUi == null)
        {
            return null;
        }

        return menuUi.AddComponent<MenuStoryIntroController>();
    }

    private static Button FindButton(params string[] objectNames)
    {
        for (int i = 0; i < objectNames.Length; i++)
        {
            GameObject buttonObject = GameObject.Find(objectNames[i]);
            if (buttonObject == null)
            {
                continue;
            }

            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                return button;
            }

            button = buttonObject.GetComponentInParent<Button>();
            if (button != null)
            {
                return button;
            }

            button = buttonObject.GetComponentInChildren<Button>();
            if (button != null)
            {
                return button;
            }
        }

        return null;
    }

    private static void OpenMapHall()
    {
        SceneManager.LoadScene(MapHallSceneName);
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
