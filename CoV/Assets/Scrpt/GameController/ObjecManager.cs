using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class KeySwitchElement
{
    public string keyId;
    public GameObject objectWhenKeyIsPresent;
    public GameObject objectWhenKeyIsAbsent;
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
            if (element.objectWhenKeyIsPresent != null)
                element.objectWhenKeyIsPresent.SetActive(hasKey);
            if (element.objectWhenKeyIsAbsent != null)
                element.objectWhenKeyIsAbsent.SetActive(!hasKey);
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
