using NUnit.Framework;
using System.Data.SqlTypes;
using UnityEngine;
using System.Collections.Generic;

public class KeyInventory : MonoBehaviour
{
    private List<string> keys = new List<string>();
    
    public void AddKey(string keyID)
    {
        if(!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log($"Key '{keyID}' added to inventory.");
        }
    }

    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }
}
