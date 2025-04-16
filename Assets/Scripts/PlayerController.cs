using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerController : NetworkBehaviour
{
    // class-level variables
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    [SerializeField]
    private float forceMultiplier = 1.0f;
    [SerializeField]
    private ForceMode forceMode;
    public GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        Respawn();
    }

    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        // local variables - they're local to Update, but not to other methods
        float horizontalVelocity = Input.GetAxis("Horizontal");
        float verticalVelocity = Input.GetAxis("Vertical");

        direction = new Vector3(horizontalVelocity, 0, verticalVelocity);
    }

    // FixedUpdate is called once per frame, along with the Unity's physics engine
    void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (IsServer) {
            Move(direction);
        } else {
            MoveRpc(direction);
        }        
    }

    private void Move(Vector3 input)
    {
        rbPlayer.AddForce(input * forceMultiplier, forceMode); // function/method

        if (transform.position.z > 38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 38);
        }
        else if (transform.position.z < -38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -38);
        }
    }

    // Remote Procedural Calls
    // 
    [Rpc(SendTo.Server)]
    public void MoveRpc(Vector3 input) {
        Move(input);
    }

    private void Respawn()
    {
        int index = 0;
        while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.0f, 1.0f, 1.0f))) {
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

        if (collider.CompareTag("Hazard"))
        {
            Respawn();
        }
    }
}
