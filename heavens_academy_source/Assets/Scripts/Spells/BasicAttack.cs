using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicAttack : Spell
{
    [SerializeField] Camera cam;

    public Projectile projectile;
    public Transform firingPoint;
    public float projectileSpeed = 20f;

    PhotonView PV;

    public float damage;

    Vector3 shootPos;

    // projectile
    // public static Dictionary<int, BasicAttack> projectiles = new Dictionary<int, BasicAttack>();

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        damage = ((SpellInfo)abilityInfo).damage;
    }

    public override void Activate()
    {
        Shoot();
    }

    void Shoot()
    {
        // new vector makes ray always point to center of screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            shootPos = hit.point;
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            //? means GetComponent only called if IDamageable is on object
            //hit.collider.gameObject.GetComponent<IDamageable>()?.takeDamage(damage);
        }
        else
        {
            shootPos = ray.GetPoint(1000);
        }

        Instantiate(projectile, firingPoint.position, Quaternion.identity).transform.LookAt(shootPos);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    other.gameObject.GetComponent<IDamageable>()?.takeDamage(damage);
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    collision.collider.gameObject.GetComponent<IDamageable>()?.takeDamage(damage);
    //}

    [PunRPC]
    public void InitializeBullet(Vector3 originalDirection, float lag)
    {
        transform.forward = originalDirection;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = originalDirection * 200.0f;
        rigidbody.position += rigidbody.velocity * lag;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPos, Vector3 hitNorm)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPos, 0.3f); //0.3 is radius
        if (colliders.Length != 0)
        {
            // instantiate bullet impact prefab
        //    GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPos + hitNorm * 0.001f, Quaternion.LookRotation(hitNorm, Vector3.up) * bulletImpactPrefab.transform.rotation);
        //    Destroy(bulletImpactObj, 10f); //destroy themselves after 10 sec
        //    bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        //    GameObject bullet = Instantiate((GameObject)Resources.Load("bullet"), basicAttack.firingPoint.transform.position, basicAttack.firingPoint.transform.rotation);
        //    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    }

}
