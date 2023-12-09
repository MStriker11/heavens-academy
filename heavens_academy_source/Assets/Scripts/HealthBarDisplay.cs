using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthBarDisplay : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    private void Start()
    {
        if (PV.IsMine)
        {
            gameObject.SetActive(false);
        }
    }
}
