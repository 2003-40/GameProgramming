using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages one temporary run-card opportunity, selected card effects, and per-run card state.
/// </summary>
public class RunCardManager : MonoBehaviour
{
    private const float RequiredTimerSeconds = 20f;
    private const int CardsPerChoice = 3;
    private const string GoodCardsResourcePath = "Cards/Good";
    private const string BadCardsResourcePath = "Cards/Bad";

    private static RunCardManager instance;

    [Header("Card Pools")]
    [SerializeField] private RunCardData[] goodCardPool;
    [SerializeField] private RunCardData[] badCardPool;

    private bool isMiningRun;
    private bool hasStartedCardTimer;
    private bool cardOpportunityResolved;
    private bool drawOfferAvailable;
    private bool choiceAvailable;
    private float activeMiningSeconds;
    private RunCardData selectedCard;
    private RunCardData[] currentChoiceCards;
    private RunCardPolarity currentChoicePolarity;
    private float staminaCostMultiplier = 1f;
    private int staminaCostBonus;
    private float doubleOreDropChance;
    private float noOreDropChance;
    private float freeMiningHitChance;
    private float miningHitFailChance;
    private RunCardData[] loadedGoodCards;
    private RunCardData[] loadedBadCards;

    public static RunCardManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<RunCardManager>();
            if (instance == null)
            {
                GameObject managerObject = new GameObject(nameof(RunCardManager));
                instance = managerObject.AddComponent<RunCardManager>();
            }

            return instance;
        }
    }

    public static bool HasInstance => instance != null;
    public RunCardData[] CurrentChoiceCards => currentChoiceCards;
    public bool IsMiningRun => isMiningRun;
    public bool HasStartedCardTimer => hasStartedCardTimer;
    public bool IsDrawOfferAvailable => drawOfferAvailable;
    public bool IsChoiceAvailable => choiceAvailable;
    public bool IsCardOpportunityResolved => cardOpportunityResolved;
    public bool IsCurrentChoiceBad => currentChoicePolarity == RunCardPolarity.Bad;
    public bool HasSelectedCard => selectedCard != null;
    public string SelectedCardName => selectedCard != null ? selectedCard.DisplayName : string.Empty;
    public float ActiveMiningSeconds => activeMiningSeconds;
    public float RequiredSeconds => RequiredTimerSeconds;

    public event Action DrawOfferBecameAvailable;
    public event Action CardChoiceBecameAvailable;
    public event Action<RunCardData> CardSelected;
    public event Action RunStateChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        Instance.SyncWithCurrentScene();
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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        StaminaManager.Instance.RunEnded -= HandleRunEnded;
        StaminaManager.Instance.RunEnded += HandleRunEnded;
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        if (StaminaManager.HasInstance)
        {
            StaminaManager.Instance.RunEnded -= HandleRunEnded;
        }
    }

    private void Update()
    {
        // Card time only advances after the player starts mining or collecting during a mining run.
        if (!isMiningRun || !hasStartedCardTimer || cardOpportunityResolved || drawOfferAvailable || choiceAvailable)
        {
            return;
        }

        activeMiningSeconds = Mathf.Min(RequiredTimerSeconds, activeMiningSeconds + Time.deltaTime);
        RunStateChanged?.Invoke();

        if (activeMiningSeconds >= RequiredTimerSeconds)
        {
            MakeChoiceAvailable();
        }
    }

    public void RegisterCardTimerStartAction()
    {
        // This can be called by mining, attacking, or collecting so any real run action starts the timer.
        if (!isMiningRun)
        {
            if (FindFirstObjectByType<MiningController>() == null)
            {
                return;
            }

            BeginMiningRun();
        }

        if (hasStartedCardTimer || cardOpportunityResolved || drawOfferAvailable || choiceAvailable)
        {
            return;
        }

        hasStartedCardTimer = true;
        activeMiningSeconds = 0f;
        RunStateChanged?.Invoke();
    }

    public void RegisterSuccessfulMiningAction()
    {
        RegisterCardTimerStartAction();
    }

    public int ModifyStaminaCost(int baseCost)
    {
        if (baseCost <= 0)
        {
            return baseCost;
        }

        int scaledCost = Mathf.CeilToInt(baseCost * staminaCostMultiplier);
        return Mathf.Max(1, scaledCost + staminaCostBonus);
    }

    public bool ShouldConsumeStaminaForMiningHit()
    {
        return freeMiningHitChance <= 0f || UnityEngine.Random.value >= freeMiningHitChance;
    }

    public bool ShouldMiningHitFail()
    {
        return miningHitFailChance > 0f && UnityEngine.Random.value < miningHitFailChance;
    }

    public int GetOreDropCount()
    {
        if (noOreDropChance > 0f && UnityEngine.Random.value < noOreDropChance)
        {
            return 0;
        }

        if (doubleOreDropChance <= 0f)
        {
            return 1;
        }

        return UnityEngine.Random.value < doubleOreDropChance ? 2 : 1;
    }

    public bool AcceptDrawOffer()
    {
        // Drawing is risky by design: the offer can become either a reward set or a curse set.
        if (!drawOfferAvailable)
        {
            return false;
        }

        drawOfferAvailable = false;
        choiceAvailable = true;
        currentChoicePolarity = UnityEngine.Random.value < 0.5f ? RunCardPolarity.Good : RunCardPolarity.Bad;
        currentChoiceCards = PickCards(currentChoicePolarity);
        if (currentChoiceCards.Length == 0)
        {
            choiceAvailable = false;
            cardOpportunityResolved = true;
            Debug.LogWarning($"[Run Card] No {currentChoicePolarity} run cards are configured.");
        }

        RunStateChanged?.Invoke();
        if (choiceAvailable)
        {
            CardChoiceBecameAvailable?.Invoke();
        }

        return true;
    }

    public bool SkipDrawOffer()
    {
        if (!drawOfferAvailable)
        {
            return false;
        }

        drawOfferAvailable = false;
        cardOpportunityResolved = true;
        currentChoiceCards = null;
        RunStateChanged?.Invoke();
        return true;
    }

    public bool SelectCard(int index)
    {
        RunCardData[] cards = currentChoiceCards;
        if (!choiceAvailable || cards == null || index < 0 || index >= cards.Length)
        {
            return false;
        }

        RunCardData card = cards[index];
        selectedCard = card;
        choiceAvailable = false;
        cardOpportunityResolved = true;
        ApplyCard(card);
        RunStateChanged?.Invoke();
        CardSelected?.Invoke(card);
        Debug.Log($"[Run Card] Selected {card.DisplayName}: {card.Description}");
        return true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncWithCurrentScene();
    }

    private void SyncWithCurrentScene()
    {
        if (FindFirstObjectByType<MiningController>() != null)
        {
            BeginMiningRun();
            return;
        }

        EndRunScope();
    }

    private void BeginMiningRun()
    {
        isMiningRun = true;
        ClearRunState();
    }

    private void HandleRunEnded()
    {
        EndRunScope();
    }

    private void EndRunScope()
    {
        isMiningRun = false;
        ClearRunState();
    }

    private void ClearRunState()
    {
        // Every mining scene starts with neutral card modifiers.
        hasStartedCardTimer = false;
        cardOpportunityResolved = false;
        drawOfferAvailable = false;
        choiceAvailable = false;
        activeMiningSeconds = 0f;
        selectedCard = null;
        currentChoiceCards = null;
        currentChoicePolarity = RunCardPolarity.Good;
        staminaCostMultiplier = 1f;
        staminaCostBonus = 0;
        doubleOreDropChance = 0f;
        noOreDropChance = 0f;
        freeMiningHitChance = 0f;
        miningHitFailChance = 0f;
        RunStateChanged?.Invoke();
    }

    private void MakeChoiceAvailable()
    {
        drawOfferAvailable = true;
        RunStateChanged?.Invoke();
        DrawOfferBecameAvailable?.Invoke();
    }

    private void ApplyCard(RunCardData card)
    {
        // Card effects intentionally modify small numeric hooks already used by mining and rewards.
        switch (card.EffectType)
        {
            case RunCardEffectType.StaminaCostReduction:
                staminaCostMultiplier *= Mathf.Max(0.01f, card.EffectValue);
                break;

            case RunCardEffectType.GainGoldImmediately:
                GoldManager.Instance.AddGold(card.EffectAmount);
                break;

            case RunCardEffectType.DoubleOreDropChance:
                doubleOreDropChance = Mathf.Clamp01(card.EffectValue);
                break;

            case RunCardEffectType.RestoreStamina:
                StaminaManager.Instance.RestoreStamina(card.EffectAmount);
                break;

            case RunCardEffectType.FreeMiningHitChance:
                freeMiningHitChance = Mathf.Clamp01(card.EffectValue);
                break;

            case RunCardEffectType.StaminaCostIncrease:
                staminaCostMultiplier *= Mathf.Max(0.01f, card.EffectValue);
                break;

            case RunCardEffectType.LoseGoldImmediately:
                GoldManager.Instance.RemoveGold(card.EffectAmount);
                break;

            case RunCardEffectType.NoOreDropChance:
                noOreDropChance = Mathf.Clamp01(card.EffectValue);
                break;

            case RunCardEffectType.FlatStaminaCostIncrease:
                staminaCostBonus += Mathf.Max(0, card.EffectAmount);
                break;

            case RunCardEffectType.MiningHitFailChance:
                miningHitFailChance = Mathf.Clamp01(card.EffectValue);
                break;
        }
    }

    private RunCardData[] PickCards(RunCardPolarity polarity)
    {
        // Pick without replacement so the three offered cards are distinct.
        RunCardData[] source = GetCardPool(polarity);
        if (source.Length == 0)
        {
            return Array.Empty<RunCardData>();
        }

        RunCardData[] pickedCards = new RunCardData[Mathf.Min(CardsPerChoice, source.Length)];
        bool[] used = new bool[source.Length];
        for (int i = 0; i < pickedCards.Length; i++)
        {
            int index = UnityEngine.Random.Range(0, source.Length);
            while (used[index])
            {
                index = UnityEngine.Random.Range(0, source.Length);
            }

            used[index] = true;
            pickedCards[i] = source[index];
        }

        return pickedCards;
    }

    private RunCardData[] GetCardPool(RunCardPolarity polarity)
    {
        RunCardData[] configuredCards = polarity == RunCardPolarity.Bad ? badCardPool : goodCardPool;
        RunCardData[] configuredValidCards = FilterCards(configuredCards, polarity);
        if (configuredValidCards.Length > 0)
        {
            return configuredValidCards;
        }

        if (polarity == RunCardPolarity.Bad)
        {
            if (loadedBadCards == null)
            {
                loadedBadCards = FilterCards(Resources.LoadAll<RunCardData>(BadCardsResourcePath), polarity);
            }

            return loadedBadCards;
        }

        if (loadedGoodCards == null)
        {
            loadedGoodCards = FilterCards(Resources.LoadAll<RunCardData>(GoodCardsResourcePath), polarity);
        }

        return loadedGoodCards;
    }

    private static RunCardData[] FilterCards(RunCardData[] cards, RunCardPolarity polarity)
    {
        if (cards == null || cards.Length == 0)
        {
            return Array.Empty<RunCardData>();
        }

        int validCount = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null && cards[i].Polarity == polarity)
            {
                validCount++;
            }
        }

        RunCardData[] filteredCards = new RunCardData[validCount];
        int filteredIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null || cards[i].Polarity != polarity)
            {
                continue;
            }

            filteredCards[filteredIndex] = cards[i];
            filteredIndex++;
        }

        return filteredCards;
    }
}
