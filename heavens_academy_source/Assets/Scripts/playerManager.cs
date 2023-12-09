using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable; //using Photon's hashtable instead of default C#

public class playerManager : MonoBehaviour
{
    // public static playerManager Instance;

    PhotonView PV;

    GameObject controller;

    int kills, deaths;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine) // if true PhotonView is owned by local player
        {
            CreateController();
        }
    }

    // instantiate playerController
    private void CreateController()
    {
        Transform spawnPoint = spawnManager.Instance.getSpawnPoint();
        Debug.Log("Player Created. ");
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
        
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void getKill()
    {
        PV.RPC(nameof(RPC_getKill), PV.Owner);
    }

    [PunRPC]
    void RPC_getKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    //side note: when you change 1 custom property in photon it calls every monobehaviorpuncallbacks on player property update
    // so when we get a kill and change custom properties for kills it's also calling playerController on player property updates
    // which expects it to be "item index", so in playerController it's checking to see if item index is property being updated

    // have other players be able to find playerManagers to keep track of kda
    public static playerManager Find(Player player)
    {
        // inefficient b/c it loops through every player manager in the scene
        return FindObjectsOfType<playerManager>().SingleOrDefault(x => x.PV.Owner == player); // playerManager's PV owner is player we're finding
    }
}
