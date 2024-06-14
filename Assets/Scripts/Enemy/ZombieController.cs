using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public AudioClip attackSound;
    public Material attackMaterial;
    public Material defaultMaterial;

    //private //animator //animator;
    private AudioSource audioSource;
    private Renderer renderer;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        //animator = GetComponent<//animator>();
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<Renderer>();

        lastAttackTime = -attackCooldown;
        renderer.material = defaultMaterial;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        Debug.Log("Cambiando a estado: Perseguir");
        //animator.SetBool("isWalking", true);
        //animator.SetBool("isAttacking", false);

        renderer.material = defaultMaterial;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void AttackPlayer()
    {
        Debug.Log("Cambiando a estado: Atacar");
        //animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            //animator.SetBool("isAttacking", true);
            lastAttackTime = Time.time;
            PlayAttackSound();
            ChangeColorToAttack();
            // Aquí puedes añadir la lógica para causar daño al jugador.
        }
        else
        {
            //animator.SetBool("isAttacking", false);
        }
    }

    void Idle()
    {
        Debug.Log("Cambiando a estado: Idle");
        //animator.SetBool("isWalking", false);
        //animator.SetBool("isAttacking", false);

        renderer.material = defaultMaterial;
    }

    public void Die()
    {
        Debug.Log("Cambiando a estado: Muerte");
        isDead = true;
        //animator.SetTrigger("Die");
        // Aquí puedes añadir cualquier otra lógica necesaria cuando el zombie muere.
        Destroy(gameObject, 5f); // Destruye el zombie después de 5 segundos.
    }

    void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void ChangeColorToAttack()
    {
        if (attackMaterial != null)
        {
            renderer.material = attackMaterial;
        }
    }

    // Función para dibujar el rango de detección en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
