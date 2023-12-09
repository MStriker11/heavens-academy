using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20;
    public float lifetime = 5;
    public GameObject explosion;

    public Player Owner { get; private set; }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
        //if (activeTimeCD > 0)
        //{
        //    activeTimeCD -= Time.deltaTime;
        //}

        //if (activeTimeCD <= 0)
        //{
        //    Destroy(this.gameObject);
        //}

        // destroy bullet if it doesn't hit anything
        //same as
        // Destroy(this.gameObject, ((SpellInfo)abilityInfo).activeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);


    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void InitializeBullet(Player owner, Vector3 originalDirection, float lag)
    {
        Owner = owner;
        transform.forward = originalDirection;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = originalDirection * 200.0f;
        rigidbody.position += rigidbody.velocity * lag;
    }


}
