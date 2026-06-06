using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            return;
        }

        Button startButton = FindButton("Start_Button", "StartButton", "Start", "PlayButton");
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OpenMapHall);
            startButton.onClick.AddListener(OpenMapHall);
        }

        Button exitButton = FindButton("Exit_Button", "ExitButton", "Exit", "QuitButton");
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(QuitGame);
            exitButton.onClick.AddListener(QuitGame);
        }
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
        Application.Quit();
    }
}
