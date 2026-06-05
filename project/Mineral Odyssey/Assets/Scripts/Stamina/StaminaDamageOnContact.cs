using UnityEngine;

public class StaminaDamageOnContact : MonoBehaviour
{
    [SerializeField] private int staminaDamage = 10;
    [SerializeField] private float damageCooldown = 1f;

    public int StaminaDamage => staminaDamage;
    public float DamageCooldown => damageCooldown;
}
