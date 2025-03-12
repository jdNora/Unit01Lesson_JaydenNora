using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemCollect : NetworkBehaviour
{
    public delegate void CollectItem(Item.VegetableType item);
    public static event CollectItem ItemCollected;

    private Dictionary<Item.VegetableType, int> inventory = new Dictionary<Item.VegetableType, int>();
    private Collider collidingItem;

    private void Start()
    {

        // Populate inventory with vegetable types and their counts (0 by default)
        // "Reflection" method - Uses foreach loop and references each declared VegetableType in the Item Class as "type"
        foreach (Item.VegetableType type in System.Enum.GetValues(typeof(Item.VegetableType)))
        {
            inventory.Add(type, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && collidingItem != null)
        {
            Item item = collidingItem.GetComponent<Item>();
            AddItemToInventory(item);
            ItemCollected?.Invoke(item.typeOfVeggie);
            PrintInventory();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (collider.gameObject.CompareTag("Item"))
        {
            collidingItem = collider;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (collider.gameObject.CompareTag("Item"))
        {
            collidingItem = null;
        }
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
}
