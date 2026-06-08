using UnityEngine;

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
        if (amount <= 0)
        {
            return;
        }

        currentHealth -= amount;
        Debug.Log($"[Monster Hit] {name} took {amount} damage. HP={Mathf.Max(0, currentHealth)}/{maxHealth}");
        if (currentHealth > 0)
        {
            return;
        }

        Debug.Log($"[Monster Defeated] {name}");
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
