using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float damageAmount = 25f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener el componente PlayerHealth del objeto con el que colisiona la bala
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // Si el objeto tiene un componente PlayerHealth, aplicar daño
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

    }

}
