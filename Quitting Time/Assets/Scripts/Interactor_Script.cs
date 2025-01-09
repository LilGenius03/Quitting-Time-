using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable{
    void Interact();
}

public class Interactor_Script : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 3f;
    public InputActionReference PlayerInteract;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerInteract == null)
        {
            Debug.LogError("PlayerInteract input action is not assigned!");
        }
    }

    private void Update()
    {
        Debug.DrawRay(InteractorSource.position, InteractorSource.forward * InteractRange, Color.red);
    }



    private void OnEnable()
    {
        PlayerInteract.action.Enable();
        PlayerInteract.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        PlayerInteract.action.performed -= OnInteract;
        PlayerInteract.action.Disable();
    }

    private void OnInteract (InputAction.CallbackContext context)
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, LayerMask.GetMask("Interactable")))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
                Debug.Log($"Interacted with: {hitInfo.collider.gameObject.name}");
            }
            else
            {
                Debug.Log("No interactable object found.");
            }
        }
    }
}
