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
    private Rigidbody rb;

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

    public float verticalRotation = 0f;
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
        MoveDirection = Move.ReadValue<Vector3>();
        if(MoveDirection.magnitude > 0)
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
    }

    private void Hide(InputAction.CallbackContext context)
    {
        Debug.Log("Hidden");

        if (!isHidden)
        {
            Debug.Log("is not Hidden");
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

}
