using UnityEngine;

/// <summary>
/// Supported temporary effects that can be applied to a single mining run.
/// </summary>
public enum RunCardEffectType
{
    StaminaCostReduction,
    GainGoldImmediately,
    DoubleOreDropChance,
    RestoreStamina,
    FreeMiningHitChance,
    StaminaCostIncrease,
    LoseGoldImmediately,
    NoOreDropChance,
    FlatStaminaCostIncrease,
    MiningHitFailChance
}

/// <summary>
/// Indicates whether a card set is beneficial or harmful.
/// </summary>
public enum RunCardPolarity
{
    Good,
    Bad
}

/// <summary>
/// ScriptableObject data for a temporary run card shown in the card-choice UI.
/// </summary>
[CreateAssetMenu(fileName = "New Run Card", menuName = "Cards/Run Card")]
public class RunCardData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string cardId;
    [SerializeField] private string displayName;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Effect")]
    [SerializeField] private RunCardPolarity polarity = RunCardPolarity.Good;
    [SerializeField] private RunCardEffectType effectType;
    [SerializeField] private float effectValue;
    [SerializeField] private int effectAmount;

    public string Id => cardId;
    public string DisplayName => displayName;
    public string Description => description;
    public RunCardPolarity Polarity => polarity;
    public RunCardEffectType EffectType => effectType;
    public float EffectValue => effectValue;
    public int EffectAmount => effectAmount;
    public bool IsBad => Polarity == RunCardPolarity.Bad;
}
