using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    [SerializeField] private float forceMultiplier = 10.0f;
    [SerializeField] private ForceMode forceMode;
    public GameObject[] spawnPoints;
    
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        foreach(GameObject g in spawnPoints)
        {
            Debug.Log(g.name);
        }

        Respawn();
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        float horizontalVelocity = Input.GetAxis("Horizontal");
        float verticalVelocity = Input.GetAxis("Vertical");
        direction = new Vector3(horizontalVelocity, 0, verticalVelocity);
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if(IsServer)
        {
            Move(direction);
        }
        else
        {
            MoveRpc(direction);
        }
    }

    private void Move(Vector3 input)
    {
        rbPlayer.AddForce(input * forceMultiplier, forceMode);
        if (transform.position.z > 38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 38);
        }
        else if (transform.position.z < -38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -38);
        }
        if (transform.position.y < -10)
        {
            Respawn(); // For when the player glitches and does not detect the hazard or falls through the map
        }
    }

    [Rpc(SendTo.Server)] // "Remote Procedural Call" - Sends this request to the server to call this function
    public void MoveRpc(Vector3 input)
    {
        Move(input);
    }

    private void Respawn()
    {
        int index = 0;
        while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.0f, 1.0f, 1.0f)))
        {
            index++;
        }

        rbPlayer.MovePosition(spawnPoints[index].transform.position);
    }

    void OnTriggerExit(Collider collider)
    {
        if (!IsServer)
        {
            return;
        }

        if (collider.gameObject.CompareTag("Hazard"))
        {
            Respawn();
        }
    }
}
