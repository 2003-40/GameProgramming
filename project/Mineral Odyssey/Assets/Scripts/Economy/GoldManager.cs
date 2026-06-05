using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
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
    }

    public int AddGold(int amount)
    {
        if (amount <= 0)
        {
            return gold;
        }

        gold += amount;
        GoldChanged?.Invoke(gold);
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

        gold -= amount;
        GoldChanged?.Invoke(gold);
        return true;
    }
}
