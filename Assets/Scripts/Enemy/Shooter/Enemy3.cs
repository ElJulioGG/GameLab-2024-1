using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    public GameObject player; // Referencia al jugador
    public float detectionRange = 15f; // Rango de detección del jugador
    public float moveSpeed = 3f; // Velocidad de movimiento del enemigo
    public Transform weapon; // Referencia al arma
    public GameObject bulletPrefab; // Prefab de la bala
    public Transform firePoint; // Punto de origen del disparo
    public float bulletForce = 10f; // Fuerza de la bala
    public float fireRate = 1f; // Tasa de disparo (tiempo entre disparos)

    private float nextFireTime; // Tiempo para el próximo disparo
    private bool isShooting; // Bandera para indicar si está disparando

    private void Start()
    {
        player = GameObject.Find("Player");
        nextFireTime = Time.time;
    }

    private void Update()
    {
        // Detectar si el jugador está en el rango de visión
        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            // Mirar hacia el jugador solo en el eje Y
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Disparar si ha pasado el tiempo de espera entre disparos
            if (Time.time >= nextFireTime)
            {
                isShooting = true;
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
            else
            {
                isShooting = false;
            }
        }
    }

    private void Shoot()
    {
        // Instanciar la bala en el punto de origen del disparo
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Obtener el componente Rigidbody de la bala y aplicarle fuerza hacia adelante
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);

        // Destruir la bala después de cierto tiempo
        Destroy(bullet, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el rango de detección en la escena para visualización
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
