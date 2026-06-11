using UnityEngine;

/// <summary>
/// Minimal health component used by tool attacks to remove or disable simple monsters.
/// </summary>
public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = true;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        // Ignore invalid damage so accidental zero-value weapon settings do not kill enemies.
        if (amount <= 0)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth > 0)
        {
            return;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
