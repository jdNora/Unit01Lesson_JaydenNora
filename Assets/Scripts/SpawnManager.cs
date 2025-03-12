using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    public GameObject[] lilyPads;
    public float startDelay = 2.0f;
    public float spawnInterval = 2.0f;

    public override void OnNetworkSpawn() // Network equivalent of "Start"
    {
        if (!IsServer)
        {
            return;
        }
        InvokeRepeating("SpawnLilyPad", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnLilyPad()
    {
        foreach (GameObject lilyPad in lilyPads)
        {
            NetworkObject lilyPadObject = Instantiate(lilyPad).GetComponent<NetworkObject>();
            lilyPadObject.Spawn();
        }
    }
}
