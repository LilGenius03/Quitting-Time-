using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    void Interact();
}

public class Interactor_Script : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 3f;
    public InputActionReference PlayerInteract;
    public GameObject PressF;
    public GameObject KeyItemCollected;


    private bool isInteractableInRange = false;
    private IEnumerator Cor;

    void Start()
    {
        if (PlayerInteract == null)
        {
            Debug.LogError("PlayerInteract input action is not assigned!");
        }
        PressF.SetActive(false); 
        KeyItemCollected.SetActive(false);
    }

    private void Update()
    {
        Debug.DrawRay(InteractorSource.position, InteractorSource.forward * InteractRange, Color.red);

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, LayerMask.GetMask("Interactable")))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (!isInteractableInRange)
                {
                    isInteractableInRange = true;
                    PressF.SetActive(true);
                }
            }
        }
        else
        {
            
            if (isInteractableInRange) 
            {
                isInteractableInRange = false;
                PressF.SetActive(false);
            }
        }
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
        PressF.SetActive(false);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInteractableInRange) 
        {
            Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, LayerMask.GetMask("Interactable")))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                    Debug.Log($"Interacted with: {hitInfo.collider.gameObject.name}");
                    PressF.SetActive(false); 
                    isInteractableInRange = false; 
                    KeyItemCollected.SetActive(true);
                    Cor = TextDisplayTime(3.0f);
                    StartCoroutine(Cor);
                }
            }
        }
    }

    IEnumerator TextDisplayTime(float DisplayTime)
    {
        KeyItemCollected.SetActive(true);
        yield return new WaitForSecondsRealtime(DisplayTime);
        KeyItemCollected.SetActive(false);
    }
}

