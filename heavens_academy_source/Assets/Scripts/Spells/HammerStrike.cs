using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HammerStrike : MonoBehaviour
{
    [SerializeField] SpellInfo spellInfo;

    private void Update()
    {
        despawnShield();
    }

    void despawnShield()
    {
        if (!spellInfo.isSpellActive())
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    collision.gameObject.GetComponent<IDamageable>()?.takeDamage(spellInfo.damage);
    //}
}
