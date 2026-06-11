using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages one temporary run-card opportunity, selected card effects, and per-run card state.
/// </summary>
public class RunCardManager : MonoBehaviour
{
    private const float RequiredTimerSeconds = 20f;
    private const float StaminaReductionMultiplier = 0.75f;
    private const float StaminaIncreaseMultiplier = 1.2f;
    private const float DoubleOreDropChance = 0.35f;
    private const float FreeMiningHitChance = 0.2f;
    private const float NoOreDropChance = 0.25f;
    private const float MiningHitFailChance = 0.15f;
    private const int ImmediateGoldAmount = 20;
    private const int RestoreStaminaAmount = 20;
    private const int ImmediateGoldLossAmount = 15;
    private const int FlatStaminaCostPenalty = 1;
    private const int CardsPerChoice = 3;

    private static RunCardManager instance;
    private static RunCardData[] goodCards;
    private static RunCardData[] badCards;

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
        RunStateChanged?.Invoke();
        CardChoiceBecameAvailable?.Invoke();
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

    private static RunCardData[] PickCards(RunCardPolarity polarity)
    {
        // Pick without replacement so the three offered cards are distinct.
        RunCardData[] source = polarity == RunCardPolarity.Bad
            ? badCards ?? (badCards = CreateBadCards())
            : goodCards ?? (goodCards = CreateGoodCards());

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

    private static RunCardData[] CreateGoodCards()
    {
        return new[]
        {
            new RunCardData(
                "steady_breath",
                "Steady Breath",
                "Stamina costs are reduced by 25% for this run.",
                RunCardPolarity.Good,
                RunCardEffectType.StaminaCostReduction,
                StaminaReductionMultiplier,
                0),
            new RunCardData(
                "gold_cache",
                "Gold Cache",
                "Gain 20 gold immediately.",
                RunCardPolarity.Good,
                RunCardEffectType.GainGoldImmediately,
                0f,
                ImmediateGoldAmount),
            new RunCardData(
                "rich_vein",
                "Rich Vein",
                "Ore drops have a 35% chance to drop two fragments this run.",
                RunCardPolarity.Good,
                RunCardEffectType.DoubleOreDropChance,
                DoubleOreDropChance,
                0),
            new RunCardData(
                "second_wind",
                "Second Wind",
                "Restore 20 stamina immediately.",
                RunCardPolarity.Good,
                RunCardEffectType.RestoreStamina,
                0f,
                RestoreStaminaAmount),
            new RunCardData(
                "light_swing",
                "Light Swing",
                "Mining hits have a 20% chance to cost no stamina this run.",
                RunCardPolarity.Good,
                RunCardEffectType.FreeMiningHitChance,
                FreeMiningHitChance,
                0)
        };
    }

    private static RunCardData[] CreateBadCards()
    {
        return new[]
        {
            new RunCardData(
                "heavy_arms",
                "Heavy Arms",
                "Stamina costs are increased by 20% for this run.",
                RunCardPolarity.Bad,
                RunCardEffectType.StaminaCostIncrease,
                StaminaIncreaseMultiplier,
                0),
            new RunCardData(
                "gold_tax",
                "Gold Tax",
                "Lose 15 gold immediately.",
                RunCardPolarity.Bad,
                RunCardEffectType.LoseGoldImmediately,
                0f,
                ImmediateGoldLossAmount),
            new RunCardData(
                "poor_vein",
                "Poor Vein",
                "Ore drops have a 25% chance to produce nothing this run.",
                RunCardPolarity.Bad,
                RunCardEffectType.NoOreDropChance,
                NoOreDropChance,
                0),
            new RunCardData(
                "dull_edge",
                "Dull Edge",
                "Each mining hit costs 1 extra stamina this run.",
                RunCardPolarity.Bad,
                RunCardEffectType.FlatStaminaCostIncrease,
                0f,
                FlatStaminaCostPenalty),
            new RunCardData(
                "shaky_hands",
                "Shaky Hands",
                "Mining hits have a 15% chance to deal no tile damage this run.",
                RunCardPolarity.Bad,
                RunCardEffectType.MiningHitFailChance,
                MiningHitFailChance,
                0)
        };
    }
}
