using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FinalBoss : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public float moveSpeed = 3f;
    public float basicAttackCooldown = 3f;
    public float strongAttackCooldown = 6f;
    public AudioClip basicAttackSound;
    public AudioClip strongAttackSound;
    public float basicDamageAmount = 15f;
    public float strongDamageAmount = 30f;
    public int attacksBeforeStrong = 3;

    private Animator animator;
    private AudioSource audioSource;
    private float lastAttackTime;
    private int attackCount = 0;
    private bool isDead = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth script not found on player.");
        }
        lastAttackTime = -basicAttackCooldown;
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
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= basicAttackCooldown)
        {
            lastAttackTime = Time.time;
            if (attackCount < attacksBeforeStrong)
            {
                BasicAttack();
                attackCount++;
            }
            else
            {
                StrongAttack();
                attackCount = 0;
            }
        }
    }

    void BasicAttack()
    {
        animator.SetTrigger("BasicAttack");
        PlaySound(basicAttackSound);
        if (player.tag == "Player" && playerHealth != null)
        {
            playerHealth.TakeDamage(basicDamageAmount);
        }
    }

    void StrongAttack()
    {
        animator.SetTrigger("StrongAttack");
        PlaySound(strongAttackSound);
        if (player.tag == "Player" && playerHealth != null)
        {
            playerHealth.TakeDamage(strongDamageAmount);
        }
    }

    void Idle()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 5f);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

