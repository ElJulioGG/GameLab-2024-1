using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageEnemy : MonoBehaviour
{
    public float damageAmount = 25f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Obtener el componente EnemyHealth del objeto con el que colision la bala
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            // Si el objeto tiene un componente EnemyHealth, aplicar dao
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }

    }

}
