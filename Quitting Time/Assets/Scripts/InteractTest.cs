using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("It works!!!!!!!!!!!!!!!!!!!");
        Destroy(gameObject);
    }
}
