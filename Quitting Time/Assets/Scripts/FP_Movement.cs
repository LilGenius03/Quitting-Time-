using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
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

    private InputAction Move;
    private InputAction Look;
    private InputAction hide;
    //private InputAction interact;
    private InputAction sprint;

    private bool isHidden;
    private bool isSprinting;
    private float sprintTimer;

    private float verticalRotation = 0f;

    private void Awake()
    {
        PlayerControls = new PlayerFPSController();
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
    }

    private void FixedUpdate()
    {
        float currentSpeed = isSprinting ? SprintSpeed : moveSpeed;


        Vector3 forwardMovement = transform.forward * MoveDirection.z;
        Vector3 rightMovement = transform.right * MoveDirection.x;

        rb.linearVelocity = (forwardMovement + rightMovement) * currentSpeed + new Vector3(0, rb.linearVelocity.y, 0);
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

}
