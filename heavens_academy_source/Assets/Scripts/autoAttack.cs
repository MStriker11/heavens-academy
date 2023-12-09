using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class autoAttack : MonoBehaviour
{
    [SerializeField] Transform autoSpawnPoint;
    [SerializeField] GameObject autoPrefab;
    [SerializeField] float autoSpeed = 10f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var autoAtk = Instantiate(autoPrefab, autoSpawnPoint.position, autoSpawnPoint.rotation);
            autoAtk.GetComponent<Rigidbody>().velocity = autoSpawnPoint.forward * autoSpeed;
        }
    }
}
