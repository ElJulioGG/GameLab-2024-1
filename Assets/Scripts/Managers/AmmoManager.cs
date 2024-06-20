using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; set; }
    
    // UI HUD
    public TMPro.TextMeshProUGUI ammoDisplay; //Use TMPro. before the use of TMPGUI
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this ;
        }
    }
}
