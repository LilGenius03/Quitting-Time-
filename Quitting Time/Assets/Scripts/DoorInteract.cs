using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    public string requiredKeyID;  
    private bool isOpen = false;

    public void Interact()
    {
        KeyInventory playerInventory = FindFirstObjectByType<KeyInventory>();

        if (playerInventory != null && playerInventory.HasKey(requiredKeyID))
        {
            if (!isOpen)
            {
                Debug.Log("Door unlocked and opened!");
                transform.Rotate(0, 90, 0);  
                isOpen = true;
            }
        }
        else
        {
            Debug.Log("You need a specific key to unlock this door!");
        }
    }
}
