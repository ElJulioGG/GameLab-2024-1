using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isaacburh : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    private System.Random random;
    void Start()
    {
        random = new System.Random();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
            return;
        }

        StartCoroutine(ActivateTrigger());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator ActivateTrigger()
    {
        // Wait for a random time between 0 and 1 second
        float waitTime = (float)random.NextDouble();
        yield return new WaitForSeconds(waitTime);

       // Set the trigger
            animator.SetTrigger("lmao");
 
    }
}
