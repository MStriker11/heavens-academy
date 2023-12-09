using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// block projectile
public class Shield : MonoBehaviour
{
    [SerializeField] AbilityInfo abilityInfo;

    private void Update()
    {
        despawnShield();
    }

    void despawnShield()
    {
        if (!abilityInfo.isSpellActive())
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
