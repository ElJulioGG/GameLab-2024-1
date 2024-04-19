using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; //bullet lasts 3secs max in the air


    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    private void Start()
    {
       // player = GameObject.Find("player");
    }
    // Update is called once per frame
    void Update()
    {
        // Left mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FixedUpdate()
    {
        //Vector3 newPos = new Vector3(playerCamera.transform.position.x + offsetX, playerCamera.transform.position.y + offsetY, playerCamera.transform.position.z+ offsetZ);

        //transform.position = playerCamera.transform.position;
       // transform.rotation = playerCamera.transform.rotation;
        
    }
    // Will update as we advance
    private void FireWeapon()
    {
        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        // Destroy bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
