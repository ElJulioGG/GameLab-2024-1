using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuGameOver : MonoBehaviour
{
    [SerializeField] private GameObject menuGameOver;
    private PlayerHealth player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        player.PlayerDead += ActivarMenu;
    }

    private void ActivarMenu(object render, EventArgs e)
    {
        menuGameOver.SetActive(true);
    }


    // Start is called before the first frame update
    public void Reiniciar() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Funciona");
    }

    public void MenuInicial(string nombre) {
        SceneManager.LoadScene(nombre);
        print("Funciona");
    }

}
