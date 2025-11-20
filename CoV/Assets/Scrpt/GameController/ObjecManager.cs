using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class KeySwitchElement
{
    public string keyId;
    public GameObject objectKey;
    public GameObject objectNoKey;
}

public class ObjecManager : MonoBehaviour
{
    public List<KeySwitchElement> switchElements = new List<KeySwitchElement>();
    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateAllSwitches;
            UpdateAllSwitches(); // èâä˙èÛë‘ÇîΩâf
        }
    }

    void UpdateAllSwitches()
    {
        foreach (var element in switchElements)
        {
            bool hasKey = playerInventory.HasKey(element.keyId);
            if (element.objectKey != null)
                element.objectKey.SetActive(hasKey);
            if (element.objectNoKey != null)
                element.objectNoKey.SetActive(!hasKey);
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateAllSwitches;
        }
    }
}
