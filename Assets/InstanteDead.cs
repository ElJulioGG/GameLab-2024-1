using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanteDead : MonoBehaviour
{
    public float damageAmount = 100f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Destroy(collision.gameObject); // Eliminar el objeto del jugador
            }
        }
    }
}
