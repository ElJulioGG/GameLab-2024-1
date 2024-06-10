using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlaySfx("shot");
    }
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);//aqui

            Destroy(gameObject);
            AudioManager.instance.PlaySfx("explode");
        }
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");

            CreateBulletImpactEffect(objectWeHit);//aqui

            Destroy(gameObject);
           
        }
        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            print("hit an enemy");

            CreateBulletBloodImpactEffect(objectWeHit);//Efecto de sang

            Destroy(gameObject);

        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
    void CreateBulletBloodImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactBloodEfectTemp, contact.point, Quaternion.LookRotation(contact.normal));

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
