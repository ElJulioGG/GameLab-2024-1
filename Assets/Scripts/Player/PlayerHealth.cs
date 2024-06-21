using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour

{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public Canvas deathMenuCanvas;
    public AudioClip deathClip; // Clip de audio para la muerte
    private AudioSource audioSource;



    public event EventHandler PlayerDead;
    void Start()
    {
        health = maxHealth;
        audioSource = GetComponent<AudioSource>(); // Obtiene el componente AudioSource

        deathMenuCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    void Update()
    {

        // Verifica si la salud ha llegado a 0
        if (health <= 0)
        {
            PlayDeathAudio();  
            Die();
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    //   if (Input.GetKeyDown(KeyCode.P))
    //   {
    //       TakeDamage(Random.Range(5, 10));
    //   }
       if (Input.GetKeyDown(KeyCode.H))
       {
           RestoreHealth(UnityEngine.Random.Range(5, 10));
       }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
       float fillF = frontHealthBar.fillAmount;
       float fillB = backHealthBar.fillAmount;
       float hFraction = health / maxHealth;
       if (fillB > hFraction)
       {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
       if (fillF < hFraction)
       {
           backHealthBar.color = Color.green;
           backHealthBar.fillAmount = hFraction;
           lerpTimer += Time.deltaTime;
           float percentComplete = lerpTimer / chipSeed;
           percentComplete = percentComplete * percentComplete;
           frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
    
       }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
    private void PlayDeathAudio()
    {
        if (audioSource != null && deathClip != null)
        {
            audioSource.clip = deathClip;
            audioSource.Play();
        }
    }

    private void Die()
    {
        // Activa el Canvas de menú de muerte
        deathMenuCanvas.gameObject.SetActive(true);
        PlayDeathAudio();
        // Reproduce el sonido de muerte



        // Permite el uso del mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactiva el movimiento del jugador
        var movementComponents = GetComponents<MonoBehaviour>();
        foreach (var component in movementComponents)
        {
            if (component != this) // Evita desactivar este script
            {
                component.enabled = false;
            }
        }

        // Pausa otros componentes del juego (asume que tienes un método global para esto)
        Time.timeScale = 1f; // Pausa todo el juego

        // Opcional: desactivar componentes de la cámara
        var cameraComponents = Camera.main.GetComponents<MonoBehaviour>();
        foreach (var component in cameraComponents)
        {
            component.enabled = false;
        }
    }
}