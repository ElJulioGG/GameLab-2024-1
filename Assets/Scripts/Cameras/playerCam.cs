using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class playerCam : MonoBehaviour
{
    /// <summary>
    /// tilinTarjeta();
    /// 
    /// 
    /// public int Hola
    /// 
    /// private int lmao
    /// 
    /// </summary>


    // Start is called before the first frame update
    [SerializeField] private float senseX;
    [SerializeField] private float senseY;

    public Transform orientation;
    //public Transform weapon;

    private float rotationX;
    private float rotationY;
    private float rotationZ;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X")/** Time.deltaTime*/ * senseX;
        float mouseY = Input.GetAxisRaw("Mouse Y")/* * Time.deltaTime*/ * senseY;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        orientation.rotation = Quaternion.Euler(0f, rotationY, 0f);
        //weapon.rotation = Quaternion.Euler(rotationX , rotationY, 0f);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}
