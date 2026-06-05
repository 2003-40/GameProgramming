using UnityEngine;

public enum ToolTier
{
    Copper = 1,
    Iron = 2,
    Steel = 3
}

[CreateAssetMenu(fileName = "New Tool Data", menuName = "Tools/Tool Data")]
public class ToolData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Copper Pickaxe";
    [SerializeField] private ToolTier tier = ToolTier.Copper;

    [Header("Mining Power")]
    [Min(1)]
    [SerializeField] private int miningPower = 1;

    [Header("Stamina Efficiency")]
    [Min(0.01f)]
    [SerializeField] private float staminaEfficiency = 1f;

    public string DisplayName => displayName;
    public ToolTier Tier => tier;
    public int MiningPower => Mathf.Max(1, miningPower);
    public float StaminaEfficiency => Mathf.Max(0.01f, staminaEfficiency);
}
