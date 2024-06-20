using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Aqu� puedes a�adir cualquier l�gica adicional que necesites cuando el enemigo muere
        Destroy(gameObject); // Destruye el enemigo inmediatamente
    }
}
