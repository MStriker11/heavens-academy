using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

// detect if we switched scenes (started game), if y instantiate player prefab
public class lobbyManager : MonoBehaviourPunCallbacks
{
    public static lobbyManager Instance; // make singleton

    private void Awake()
    {
        if (Instance) // check if another lobbyManager is in scene
        {
            Destroy(gameObject); // if y destroy itself and return
            return;
        }
        // if n (it's the only one), don't destroy itself when scene switches and makes itself a scene instance
        DontDestroyOnLoad(gameObject); 
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable(); // base call necessary for on/off enable overrides; rest not so much
        SceneManager.sceneLoaded += OnSceneLoaded; // when we switch scenes OnSceneLoaded() called
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // TODO: STORE PHOTON PREFABS IN RESOURCES OR ELSE UNITY WILL IGNORE IT IN THE FINAL BUILD
        // View IDs on Photon View must be unique; to avoid conflicts we set lobbyManager to the max value (999)
        // instantiate photon prefabs w strings

        if (scene.buildIndex == 2) // game scene
        {
            // playerManager is going to be spawned on an empty object so can just set it at (0,0,0)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }

    }

    
}
