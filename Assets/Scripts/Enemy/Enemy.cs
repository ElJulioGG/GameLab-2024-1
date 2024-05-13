using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop = 2f;
    public float keepChasingTime = 5f;
    public float velEnemy = 5f;
    public Rigidbody theRB;
    public MovementPlayer movementPlayer;

    private Vector3 startPoint;
    private float chaseCounter;
    private bool chasing;
    private Vector3 currentTarget;


    void Start()
    {
        startPoint = transform.position;
        currentTarget = startPoint;



    }

    void Update()
    {
        Vector3 playerPosition = movementPlayer.transform.position;
        playerPosition.y = transform.position.y;  // Asegurarse que están en el mismo plano horizontal

        if (chasing)
        {
            if (Vector3.Distance(transform.position, playerPosition) > distanceToLose)
            {
                // Pierde al jugador
                chasing = false;
                chaseCounter = keepChasingTime;
                currentTarget = startPoint;  // Regresar al punto de inicio
            }
            else if (Vector3.Distance(transform.position, playerPosition) <= distanceToStop)
            {
                // Detenerse si está muy cerca del jugador
                currentTarget = transform.position;
            }
            else
            {
                currentTarget = playerPosition;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, startPoint) > 0.5f && chaseCounter <= 0)
            {
                // Volver a la posición de inicio si el jugador no ha sido visto
                currentTarget = startPoint;
            }
            else if (Vector3.Distance(transform.position, playerPosition) < distanceToChase)
            {
                // Comenzar a perseguir al jugador
                chasing = true;
                currentTarget = playerPosition;
            }

            if (chaseCounter > 0)
            {

                chaseCounter -= Time.deltaTime;
            }
        }



        // Movimiento hacia el punto actual
        Vector3 direction = (currentTarget - transform.position).normalized;
        theRB.velocity = direction * velEnemy;

        // Asegurar que el enemigo siempre mire hacia el jugador cuando esté persiguiendo
        if (chasing && currentTarget != transform.position)
        {
            transform.LookAt(new Vector3(playerPosition.x, transform.position.y, playerPosition.z));
        }
    }



}