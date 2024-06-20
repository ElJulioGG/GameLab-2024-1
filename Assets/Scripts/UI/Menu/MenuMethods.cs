using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMethods : MonoBehaviour
{
    public string nombreEscena;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BotonJugar()
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void PlayHoverSound()
    {
        AudioManager.instance.PlaySfx("ButtonHover");
    }
    public void PlayClickSound()
    {
        AudioManager.instance.PlaySfx("ButtonClick");
    }


}
