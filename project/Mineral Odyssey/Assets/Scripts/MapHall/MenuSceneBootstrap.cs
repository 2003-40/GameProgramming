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

        Button startButton = FindButton("Start_Button");
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OpenMapHall);
            startButton.onClick.AddListener(OpenMapHall);
        }

        Button exitButton = FindButton("Exit_Button");
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(QuitGame);
            exitButton.onClick.AddListener(QuitGame);
        }
    }

    private static Button FindButton(string objectName)
    {
        GameObject buttonObject = GameObject.Find(objectName);
        return buttonObject == null ? null : buttonObject.GetComponent<Button>();
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
