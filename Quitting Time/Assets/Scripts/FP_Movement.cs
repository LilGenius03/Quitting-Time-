using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FP_Movement : MonoBehaviour
{
    public PlayerFPSController PlayerControls;
    public int moveSpeed = 3;
    public int SprintSpeed = 6;
    public float SprintTime = 1.5f;

    private Vector3 MoveDirection = Vector3.zero;
    public Rigidbody rb;

    public Transform playerCamera;
    public float lookSensitivity = 100f;
    public float maxLookAngle = 90f;

    public AudioClip[] groundSounds;
    public AudioClip[] stairsSounds;

    private AudioSource audioSource;
    private string currentSurface;
    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.2f;

    private InputAction Move;
    private InputAction Look;
    private InputAction hide;
    //private InputAction interact;
    private InputAction sprint;

    private bool isHidden;
    private bool isSprinting;
    private float sprintTimer;

    private Transform hidingSpot;
    public float mouseMovementThreshold = 10f;
    private Vector2 lastMousePosition;


    public float verticalRotation = 0f;
    private float detectionRisk = 0f;
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.05f;
    public float SprintBobSpeed = 18f;
    public float SprintBobAmount = 0.1f;
    public float BobbingFadeOut = 5f;
    private float bobOffset = 0f;
    private bool isBobbing = false;

    private void Awake()
    {
        PlayerControls = new PlayerFPSController();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isHidden = false;
        isSprinting = false;
        sprintTimer = SprintTime;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 moveInput = Move.ReadValue<Vector2>();
        MoveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (MoveDirection.magnitude > 0)
        {
            isBobbing = true;
        }

        else
        {
            isBobbing = false;
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0)
            {
                StopSprinting();
            }
        }

        if(!isHidden)
        {
            HandleMovement();
            HandleLook();
        }
        else
        {
            HandleHidingLook();
            Debug.Log(detectionRisk);
        }

        Vector2 lookInput = Look.ReadValue<Vector2>();
        RotateCamera(lookInput);

        bobOffset = Mathf.Lerp(bobOffset, (isSprinting ? SprintBobAmount : walkBobAmount) * Mathf.Sin(Time.time * (isSprinting ? SprintBobSpeed : walkBobSpeed)), Time.deltaTime);
    }

    private void FixedUpdate()
    {
        float currentSpeed = isSprinting ? SprintSpeed : moveSpeed;


        Vector3 forwardMovement = transform.forward * MoveDirection.z;
        Vector3 rightMovement = transform.right * MoveDirection.x;

        rb.linearVelocity = (forwardMovement + rightMovement) * currentSpeed + new Vector3(0, rb.linearVelocity.y, 0);

        if(isBobbing)
        {
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, bobOffset, playerCamera.localPosition.z);
        }

        else
        {
            bobOffset = Mathf.Lerp(bobOffset, 0f, Time.deltaTime * BobbingFadeOut);
        }


        if (MoveDirection.magnitude <= 0 && !isBobbing) 
        { 
           bobOffset = 0;
        }
        string newSurface = GetCurrentSurface();
        if(newSurface != currentSurface) 
        {
            currentSurface = newSurface;
        }

        if (MoveDirection.magnitude > 0f)
        {
            PlayFootstepSound(currentSurface);
        }
    }

    private void HandleMovement()
    {
        Vector2 moveInput = Move.ReadValue<Vector2>();
        Vector3 forwardMovement = transform.forward * moveInput.y;
        Vector3 rightMovement = transform.right * moveInput.x;

        rb.linearVelocity = (forwardMovement + rightMovement) * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
    }

    void HandleLook()
    {
        Vector2 lookInput = Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleHidingLook()
    {
        Vector2 lookInput = Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        detectionRisk += (Mathf.Abs(mouseX) + Mathf.Abs(mouseY)) * Time.deltaTime;

        if (Mathf.Abs(mouseX) + Mathf.Abs(mouseY) > mouseMovementThreshold)
        {
            ExitHiding();
            
        }

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void OnEnable()
    {
        Move = PlayerControls.Player.Move;
        Move.Enable();
        Look = PlayerControls.Player.Look;
        Look.Enable();

        hide = PlayerControls.Player.Hide;
        //interact = PlayerControls.Player.ObjectInteract;
        sprint = PlayerControls.Player.Sprint;

        hide.Enable();
        hide.performed += Hide;

        //interact.Enable();
        //interact.performed += Interact;

        sprint.Enable();
        sprint.performed += Sprint;
        sprint.canceled += ctx => StopSprinting();

        isHidden = true;
    }

    private void OnDisable()
    {
        Move.Disable();
        hide.Disable();
        Look.Disable();
        //interact.Disable();
        sprint.Disable();
        isHidden = false;
        hide.performed -= Hide;
    }

    private void Hide(InputAction.CallbackContext context)
    {
        

        if (isHidden)
        {
            ExitHiding();
        }
        else
        {
            EnterHiding();
        }
    }

    /*private void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interacted");
    }*/

    private void Sprint(InputAction.CallbackContext context)
    {
        if (!isSprinting)
        {
            isSprinting = true;
            sprintTimer = SprintTime;
            Debug.Log("Sprinting started");
        }
    }

    private void StopSprinting()
    {
        if (isSprinting)
        {
            isSprinting = false;
            sprintTimer = SprintTime;
            Debug.Log("Sprinting stopped");
        }
    }

    private void RotateCamera(Vector2 lookInput)
    {
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);


        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private string GetCurrentSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return "Ground";
            }
            else if (hit.collider.gameObject.CompareTag("Stairs"))
            {
                return "Stairs";
            }        
        }
        return "Default";
    }

    void PlayFootstepSound(string surface)
    {
        if(Time.time > lastFootstepTime + footstepCooldown)
        {
            AudioClip[] sounds;

            switch (surface)
            {
                case "Ground":
                    sounds = groundSounds;
                    break;
                case "Stairs":
                    sounds = stairsSounds;
                    break;

                default:
                    return;

            }

            int footIndex = (int)(Time.time * 2) % 2;

            audioSource.clip = sounds[footIndex];
            audioSource.PlayOneShot(audioSource.clip, Random.Range(0.8f, 1.2f));
            lastFootstepTime = Time.time;
        }
        
    }

    private void EnterHiding()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("HidingSpot"));
        if (colliders.Length > 0)
        {
            hidingSpot = colliders[0].transform;
            playerCamera.position = hidingSpot.position;

            rb.isKinematic = true;

            isHidden = true;
            detectionRisk = 0f; 
            Debug.Log("Player is now hiding!");
        }
        else
        {
            Debug.Log("No hiding spot nearby!");
        }
    }

    private void ExitHiding()
    {
        if (isHidden)
        {
            playerCamera.localPosition = Vector3.zero;

            rb.isKinematic = false;

            isHidden = false;
            Debug.Log("Player exited hiding!");
        }
    }

}
