using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    public GameObject dilan;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("createIlan", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void movement()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
          // Vector3

        }


    }
    void createIlan()
    {
        Instantiate(dilan);
        Invoke("createIlan", 2f);
    }
}
