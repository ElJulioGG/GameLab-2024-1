using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sounds[] musicSounds, sfxSounds, playerSounds, UISounds;
    public AudioSource musicSource, sfxSource, playerSource, UISource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //musica del menu
    }
    public void PlayMusic(string name)
    {
        Sounds s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySfx(string name)
    {
        Sounds s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip); //hay varios metodos para controlar audio
            //ver la documentacion de unity
        }
    }
    public void PlayerSteps(string name)
    {
        Sounds s = Array.Find(playerSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            playerSource.PlayOneShot(s.clip);
        }
    }

    public void PlayBark(string name)
    {
        Sounds s = Array.Find(UISounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        else
        {
            UISource.PlayOneShot(s.clip);
        }
    }
}