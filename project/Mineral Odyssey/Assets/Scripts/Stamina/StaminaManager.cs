using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent run-budget manager that tracks stamina, notifies UI, and ends runs when depleted.
/// </summary>
public class StaminaManager : MonoBehaviour
{
    private static StaminaManager instance;

    [Header("Stamina")]
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private int currentStamina = 100;

    [Header("Run End")]
    [SerializeField] private bool resetWhenEnteringMiningScene = true;

    private bool isEndingRun;
    private Coroutine endRunCoroutine;

    public static StaminaManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<StaminaManager>();

            if (instance == null)
            {
                GameObject staminaObject = new GameObject(nameof(StaminaManager));
                instance = staminaObject.AddComponent<StaminaManager>();
            }

            return instance;
        }
    }

    public static bool HasInstance => instance != null;
    public int CurrentStamina => currentStamina;
    public int MaxStamina => maxStamina;
    public bool HasStamina => currentStamina > 0;

    public event Action<int, int> StaminaChanged;
    public event Action StaminaDepleted;
    public event Action RunEnded;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        StaminaManager manager = Instance;
        manager.ResetForCurrentSceneIfNeeded();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ResetRunStamina()
    {
        // Entering a mining scene starts a fresh run budget.
        if (endRunCoroutine != null)
        {
            StopCoroutine(endRunCoroutine);
            endRunCoroutine = null;
        }

        isEndingRun = false;
        currentStamina = maxStamina;
        StaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public bool ConsumeStamina(int amount)
    {
        // Mining and monster damage both use this path so run-ending behavior is consistent.
        if (amount <= 0)
        {
            return currentStamina > 0;
        }

        if (currentStamina <= 0)
        {
            return false;
        }

        currentStamina = Mathf.Max(0, currentStamina - amount);
        StaminaChanged?.Invoke(currentStamina, maxStamina);

        if (currentStamina <= 0)
        {
            ScheduleEndCurrentRun();
        }

        return true;
    }

    public int RestoreStamina(int amount)
    {
        if (amount <= 0)
        {
            return currentStamina;
        }

        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        StaminaChanged?.Invoke(currentStamina, maxStamina);
        return currentStamina;
    }

    public void EndCurrentRun()
    {
        ScheduleEndCurrentRun();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetForCurrentSceneIfNeeded();
    }

    private void ResetForCurrentSceneIfNeeded()
    {
        if (!resetWhenEnteringMiningScene)
        {
            return;
        }

        if (FindFirstObjectByType<MiningController>() != null)
        {
            ResetRunStamina();
        }
    }

    private void ScheduleEndCurrentRun()
    {
        // Defer run end by one frame so the current hit/collection can finish cleanly.
        if (isEndingRun)
        {
            return;
        }

        isEndingRun = true;
        if (currentStamina <= 0)
        {
            StaminaDepleted?.Invoke();
        }

        endRunCoroutine = StartCoroutine(EndCurrentRunAfterFrame());
    }

    private IEnumerator EndCurrentRunAfterFrame()
    {
        yield return null;

        RunEnded?.Invoke();
        endRunCoroutine = null;
    }
}
