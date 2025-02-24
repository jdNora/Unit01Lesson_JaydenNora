using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    [SerializeField] private float forceMultiplier = 10.0f;
    [SerializeField] private ForceMode forceMode;
    public GameObject spawnPoint;
    private Dictionary<Item.VegetableType, int> inventory = new Dictionary<Item.VegetableType, int>();

    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();

        // Populate inventory with vegetable types and their counts (0 by default)
        // "Reflection" method - Uses foreach loop and references each declared VegetableType in the Item Class as "type"
        foreach (Item.VegetableType type in System.Enum.GetValues(typeof(Item.VegetableType)))
        {
            inventory.Add(type, 0);
        }
    }
    
    private void Update()
    {
        float horizontalVelocity = Input.GetAxis("Horizontal");
        float verticalVelocity = Input.GetAxis("Vertical");
        direction = new Vector3(horizontalVelocity, 0, verticalVelocity);
    }

    void FixedUpdate()
    {
        rbPlayer.AddForce(direction * forceMultiplier, forceMode);
        if(transform.position.z > 38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 38);
        }
        else if (transform.position.z < -38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -38);
        }
    }

    private void Respawn()
    {
        rbPlayer.MovePosition(spawnPoint.transform.position);
    }

    private void AddItemToInventory(Item item)
    {
        inventory[item.typeOfVeggie]++;
        PrintInventory();
    }

    private void PrintInventory()
    {
        string output = "";

        // okay, you lost me
        foreach (KeyValuePair<Item.VegetableType, int> pair in inventory)
        {
            output += string.Format("{0}: {1}; ", pair.Key, pair.Value);
        }

        Debug.Log(output);
    }

    void OnTriggerEnter(Collider collider)
    {
        Item item = collider.GetComponent<Item>();
        if (collider.gameObject.CompareTag("Item"))
        {
            AddItemToInventory(item);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Hazard"))
        {
            Respawn();
        }
    }
}
