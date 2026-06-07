using System;

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

public enum RunCardPolarity
{
    Good,
    Bad
}

[Serializable]
public class RunCardData
{
    public RunCardData(
        string id,
        string displayName,
        string description,
        RunCardPolarity polarity,
        RunCardEffectType effectType,
        float effectValue,
        int effectAmount)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Polarity = polarity;
        EffectType = effectType;
        EffectValue = effectValue;
        EffectAmount = effectAmount;
    }

    public string Id { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public RunCardPolarity Polarity { get; private set; }
    public RunCardEffectType EffectType { get; private set; }
    public float EffectValue { get; private set; }
    public int EffectAmount { get; private set; }
    public bool IsBad => Polarity == RunCardPolarity.Bad;
}
