
using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public string keyID;

    public void Interact()
    {
        KeyInventory playerInventory = FindFirstObjectByType<KeyInventory>();
        if (playerInventory != null)
        {
            playerInventory.AddKey(keyID);
            Debug.Log($"Picked up key: {keyID}");
            Destroy(gameObject);
        }
    }
}
