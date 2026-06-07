using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    private const string GoldSaveKey = "Gold";

    private static GoldManager instance;

    [Header("Currency")]
    [SerializeField] private int gold;

    public static GoldManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<GoldManager>();

            if (instance == null)
            {
                GameObject goldObject = new GameObject(nameof(GoldManager));
                instance = goldObject.AddComponent<GoldManager>();
            }

            return instance;
        }
    }

    public int Gold => gold;
    public static bool HasInstance => instance != null;

    public event Action<int> GoldChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGold();
    }

    public int AddGold(int amount)
    {
        if (amount <= 0)
        {
            return gold;
        }

        SetGold(gold + amount);
        return gold;
    }

    public int RemoveGold(int amount)
    {
        if (amount <= 0)
        {
            return gold;
        }

        SetGold(Mathf.Max(0, gold - amount));
        return gold;
    }

    public bool CanAfford(int amount)
    {
        return amount <= 0 || gold >= amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (gold < amount)
        {
            return false;
        }

        SetGold(gold - amount);
        return true;
    }

    private void LoadGold()
    {
        gold = PlayerPrefs.GetInt(GoldSaveKey, gold);
    }

    private void SetGold(int value)
    {
        gold = value;
        PlayerPrefs.SetInt(GoldSaveKey, gold);
        PlayerPrefs.Save();
        GoldChanged?.Invoke(gold);
    }
}
