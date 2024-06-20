using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour

{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    void Start()
    {
        health = maxHealth;
    }



    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    //   if (Input.GetKeyDown(KeyCode.P))
    //   {
    //       TakeDamage(Random.Range(5, 10));
    //   }
    //   if (Input.GetKeyDown(KeyCode.I))
    //   {
    //       RestoreHealth(Random.Range(5, 10));
    //   }
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



}