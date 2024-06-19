using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    //public AudioSource shootingSound1; Idk it doesn't work
    public AudioSource reloadingSound1;
    public AudioSource emptyMagazineSound1911;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //  Weapon sound detector (for sound and other stuff)
    //public void PlayShootingSound(WeaponModel weapon)
    //{
    //    switch (type)
    //    {
    //        case WeaponModel.M1911Pistol:
    //            shootingSound1.Play();
    //            break;
    //        case WeaponModel.M48Rifle:
    //            //shootingSound1911.Play(); // Same but with new M48 sound
    //            break;
    //    }
    //}
    //
    //public void PlayReloadSound(WeaponModel weapon)
    //{
    //    switch (type)
    //    {
    //        case WeaponModel.M1911Pistol:
    //            reloadingSound1.Play();
    //            break;
    //        case WeaponModel.M48Rifle:
    //            //reloadingSound1.Play(); // Same but with new M48 sound
    //            break;
    //    }
    //}

}
