using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountGUI : MonoBehaviour
{
    private TextMeshProUGUI tmProElement;
    public string itemName;
    public NetworkVariable<int> count = new NetworkVariable<int>(0);

    // Start is called before the first frame update
    void Start()
    {
        tmProElement = GetComponent<TextMeshProUGUI>();
        count.OnValueChanged += OnCountValueChanged;
    }

    public void UpdateCountBroadcast()
    {
        if (IsServer) // okay but why
        {
            UpdateCount();
        }
        else
        {
            UpdateCountRpc();
        }
    }

    public override OnNetworkSpawn()
    {
        UpdateText();
    }

    private void OnCountValueChanged(int previousValue, int newValue)
    {
        UpdateText();
    }

    [Rpc(SendTo.Server)]
    public void UpdateCountRpc()
    {
        UpdateCount();
    }

    public void UpdateCount()
    {
        count.Value++;
    }

    public void UpdateText()
    {
        tmProElement.text = itemName + ": " + count.Value;
    }

}
