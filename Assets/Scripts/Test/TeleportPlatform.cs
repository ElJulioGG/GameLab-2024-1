using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlatform : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject playerObject;
    public GameObject TpOut;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerObject.transform.position = TpOut.transform.position;
        }
    }
}
