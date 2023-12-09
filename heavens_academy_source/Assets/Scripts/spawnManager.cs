using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    public static spawnManager Instance;

    // set where players will respawn manually
    public spawnPoint[] spawnPointList;

    private void Awake()
    {
        Instance = this;
        spawnPointList = GetComponentsInChildren<spawnPoint>();
    }

    // manually set which player to spawn at which location
    public Transform getSpawnPoint(int playerNum)
    {
        return spawnPointList[playerNum].transform;
    }

    public Transform getSpawnPoint()
    {
        //TODO: spawn points will need to be set later based on team
        return spawnPointList[Random.Range(0, spawnPointList.Length)].transform;
    }
}
