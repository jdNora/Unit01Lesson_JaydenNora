using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Drift : NetworkBehaviour
{
    public float speed = 10.0f;
    public DriftDirection driftDirection;
    public enum DriftDirection
    {
        LEFT = -1,
        RIGHT = 1
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        transform.Translate(Vector3.right * speed * (int)driftDirection * Time.deltaTime);

        if (transform.position.x < -80 || transform.position.x > 80)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                
                NetworkObject player = transform.GetChild(i).GetComponent<NetworkObject>();
                player.TryRemoveParent();
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            NetworkObject player = collision.gameObject.GetComponent<NetworkObject>();
            player.TrySetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NetworkObject player = collision.gameObject.GetComponent<NetworkObject>();
            player.TryRemoveParent();
        }
    }
}
